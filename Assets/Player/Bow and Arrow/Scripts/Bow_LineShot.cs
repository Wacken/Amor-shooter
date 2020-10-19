using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BowState
{
    NoArrow,
    ArrowLoaded,
    Drawing,
    Relaxing,
    ReadyToFire,
    IsFiring,
    HasFired
}

public class Bow_LineShot : MonoBehaviour {

    public BowState State = BowState.NoArrow;

    [Header("Arrow")]
    public GameObject arrowPrefab;
    private Arrow currentArrow;

    [Header("Bow Components")]
    public LineRenderer BowstringTop;
    public LineRenderer BowstringBot;

    public Transform BowstringTopPosition;
    public Transform BowstringBotPosition;

    public Transform FirePosition;
    public Transform BowStringMidPoint;
    private Vector3 defaultBowStringMidPoint;

    [Header("Shot Path Visualization")]
    public LineRenderer ShotPathLineRenderer;

    private float defaultArrowSpeed;

    public bool shouldShowShot;

    [Header("Config")]
    [Tooltip("time in seconds you need to draw the bow before you can fire")]
    public float MinDrawTime;
    [Tooltip("time is seconds you need to draw to gain max strength. Can still draw afterwards but it doesn't do anything.")]
    public float MaxDrawTime;
    public float MaxDrawDistance;
    private float DrawSpeed;

    public float MaxShotStrenght;
    public float CooldownAfterShot;

    public float relaxTime = 0.01f;
    public float relaxWaitTime = 0;

    private float currentDrawTime = 0;
    private float currentDrawLength = 0;


	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;

        defaultArrowSpeed = (arrowPrefab.GetComponent(typeof(Arrow)) as Arrow).speed;

        defaultBowStringMidPoint = BowStringMidPoint.localPosition;
	}

    private void Update()
    {
        switch (State)
        {
            case BowState.NoArrow:
                // load arrow animation
                StartCoroutine(loadArrow());
                break;
            case BowState.ArrowLoaded:
                // maybe idle movement
                // wait for fire
                if (isFireButtonPressed())
                    StartCoroutine(drawBow());
                break;
            case BowState.ReadyToFire:
                // advance to firing state
                if (!isFireButtonPressed())
                    StartCoroutine(fireBow());
                // right mouse button -> relax bow
                else if (Input.GetMouseButtonDown(1))
                    StartCoroutine(relaxBow());
                else currentDrawTime += Time.deltaTime;
                break;
            case BowState.Drawing:
                // in coroutine
                break;
            case BowState.Relaxing:
                // in coroutine
                break;
            case BowState.IsFiring:
                // in coroutine
                break;
            case BowState.HasFired:
                // in fire coroutine
                break;
        }

        UpdateBowString();

        shouldShowShot = State == BowState.ReadyToFire || State == BowState.Drawing;

        VisualizeShotPath();
    }

    #region coroutines for the states

    IEnumerator loadArrow()
    {
        // create arrow at fireposition as child of the bow
        currentArrow = GameObject.Instantiate(arrowPrefab, FirePosition.position, FirePosition.rotation, transform).GetComponent(typeof(Arrow)) as Arrow;

        // set state
        State = BowState.ArrowLoaded;

        // load animation (?)

        yield return null;
    }

    IEnumerator drawBow()
    {
        // setup in current frame
        State = BowState.Drawing;
        currentDrawTime = 0;
        currentDrawLength = 0;

        DrawSpeed = MaxDrawDistance / MinDrawTime;

        yield return null;

        // draw loop
        while(currentDrawTime < MinDrawTime)
        {
            if(!isFireButtonPressed())
            {
                currentArrow.transform.position = FirePosition.position;
                currentArrow.transform.rotation = FirePosition.rotation;
                State = BowState.ArrowLoaded;
                // abort
                yield break;
            }

            if (currentDrawLength < MaxDrawDistance)
            {
                float dist = DrawSpeed * Time.deltaTime;
                Vector3 pos = currentArrow.transform.localPosition;
                pos.z -= dist;
                currentArrow.transform.localPosition = pos;

                currentDrawLength += dist;
            }
            currentDrawTime += Time.deltaTime;
            yield return null;
        }
        // advance to next state depending on whether button is pressed
        if (isFireButtonPressed())
            State = BowState.ReadyToFire;
        else StartCoroutine(fireBow());
    }

    IEnumerator relaxBow()
    {
        State = BowState.Relaxing;

        Vector3 arrowOffset = FirePosition.localPosition - currentArrow.transform.localPosition;

        currentDrawLength = arrowOffset.magnitude;

        float speed = (currentDrawLength / relaxTime);
        arrowOffset.Normalize();

        while(currentDrawLength > 0)
        {
            float dist = speed * Time.deltaTime;

            Vector3 pos = currentArrow.transform.localPosition;
            pos += dist * arrowOffset;
            currentArrow.transform.localPosition = pos;

            currentDrawLength -= dist;
            yield return null;
        }
        yield return new WaitForSeconds(relaxWaitTime);
        State = BowState.ArrowLoaded;
    }

    IEnumerator fireBow()
    {
        State = BowState.IsFiring;
        
        // shoot arrow
        currentArrow.transform.SetParent(null, true);
        GetComponent<AudioSource>().Play();
        currentArrow.Shoot(getShotStrength());
        
        // reset current arrow
        currentArrow = null;
        BowStringMidPoint.localPosition = defaultBowStringMidPoint;

        yield return null;

        StartCoroutine(hasFired());
    }

    IEnumerator hasFired()
    {
        State = BowState.HasFired;
        yield return new WaitForSeconds(CooldownAfterShot);
        State = BowState.NoArrow;
    }

    #endregion coroutines for the states

    #region visualization

    private void UpdateBowString()
    {
        if (currentArrow != null)
            BowStringMidPoint.position = currentArrow.backPoint.position;

        Vector3[] pos = new Vector3[2];
        pos[1] = BowStringMidPoint.localPosition;

        pos[0] = BowstringTopPosition.localPosition;
        BowstringTop.SetPositions(pos);
        
        pos[0] = BowstringBotPosition.localPosition;
        BowstringBot.SetPositions(pos);
    }

    private void VisualizeShotPath()
    {
        if (shouldShowShot)
        {
            ShotPathLineRenderer.enabled = true;

            Vector3[] points = new Vector3[ShotPathLineRenderer.positionCount];
            ShotPathLineRenderer.GetPositions(points);
            GetParabolaPoints(points);
            ShotPathLineRenderer.SetPositions(points);
        }
        else ShotPathLineRenderer.enabled = false;
    }

    private static float[] timesteps =
        {   0, 0.01f, 0.02f, 0.03f, 0.05f,
            0.075f, 0.1f, 0.15f, 0.2f, 0.3f,
            0.4f, 0.5f, 0.75f, 1f, 1.5f,
            2f, 2.5f, 3f, 4f, 5f,
            6f, 7f, 10f, 15f, 30f
        };

    private void GetParabolaPoints(Vector3[] pathPoints)
    {
        if (pathPoints == null || pathPoints.Length == 0)
            return;

        Vector3 fireDirection = FirePosition.forward.normalized;

        Vector3 fireDirPlane = fireDirection;
        fireDirPlane.y = 0;
        fireDirPlane.Normalize();

        float InitSpeed = getShotStrength() * defaultArrowSpeed;


        // cosine of look direction with y-axis
        float initAngle = Vector3.Dot(Vector3.up, fireDirection);

        initAngle = (Mathf.PI /2f) - Mathf.Acos(initAngle);
        // initAngle now is angle between the projectile direction and the x-axis

        float initVelY = InitSpeed * Mathf.Sin(initAngle);
        float vel_plane = InitSpeed * Mathf.Cos(initAngle);

        initVelY = fireDirection.y * InitSpeed;
        vel_plane = InitSpeed;

        if(Mathf.Rad2Deg * initAngle > 75f || Mathf.Rad2Deg * initAngle < -75f || vel_plane == 0)
        {
        //*
            // don't show anything
            ShotPathLineRenderer.enabled = false;
            return;
        /*/
            // return a straight line
            for (int i = 0; i < pathPoints.Length; i++)
                pathPoints[i].y = 0;
            return;
        //*/
        }

        //float accX = 0;
        float accY = Physics.gravity.y;

        //Debug.DrawRay(FirePosition.position, fireDirPlane * 10f, Color.grey);

        for(int i = 0; i < pathPoints.Length; i++)
        {
            float time = timesteps[i];
            float world_plane_distance = vel_plane * time;
            float world_height = initVelY * time + accY * Mathf.Pow(time, 2);

            // this won't make sense unless you've got my sketches there sry
            float d = world_plane_distance / Mathf.Cos(initAngle);
            float star = world_height - world_plane_distance * Mathf.Tan(initAngle);
            float h = star * Mathf.Cos(initAngle);
            float dplus = h * Mathf.Tan(initAngle);


            pathPoints[i].y = h;
            pathPoints[i].z = d + dplus;
        }
    }

    #endregion

    private bool isFireButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    private float getShotStrength()
    {
        return Mathf.Clamp01(currentDrawTime / MaxDrawTime) * MaxShotStrenght;
    }
}
