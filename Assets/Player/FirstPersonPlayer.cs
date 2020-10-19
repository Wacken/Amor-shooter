using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum MovementState
{
    Walking,
    JumpingUp,
    Fluttering,
    Falling
}

public class FirstPersonPlayer : MonoBehaviour {

    public Transform cam;
    public LayerMask ground;
    public static Transform instance;

    private float rotX, rotY;
    [Header("Direct Controls")]
    public float walkSpeed;
    public float lookSpeed, upSpeed, downSpeed;

    [Header("Managed State")]
    public MovementState State = MovementState.Walking;
    public float jumpImpulseUpwards;
    public float jumpImpulseForward;
    [Tooltip("Gravity factor used when jumping high (upwards only). e.g.: \n  0 -> no gravity = infinite jump \n  1 -> normal gravity")]
    public float reducedGravityFactor;
    [Tooltip("Gravity factor used when doing just a small jump (upwards only). e.g.: \n  1 -> normal gravity \n  3 -> tripled gravity")]
    public float increasedGravityFactor;
    [Tooltip("strength of the downwards force that's applied when the player stops fluttering")]
    public float fallImpulse;

    [Header("Audio")]
    public AudioSource flaploop;
    public AudioSource jump;


    // components
    private Rigidbody RB;
    private Bow_LineShot bow;

	void Start () {
        instance = transform;
        RB = GetComponent<Rigidbody>();
        bow = GetComponentInChildren<Bow_LineShot>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        flaploop = GetComponent<AudioSource>();
    }
	
	void FixedUpdate () {

        bool isJumpKeyPressed = Input.GetKey(KeyCode.Space);
        //*
        #region Managed State Jumping
        switch (State)
        {
            case MovementState.Walking:
            {
                    if (flaploop.isPlaying) { flaploop.Stop(); }
                    if (transform.position.y > 50)
                        State = MovementState.Falling;
                    else        
                    // check for jump
                    if (Input.GetKeyDown(KeyCode.Space) && isStandingOnGround())
                    {
                        State = MovementState.JumpingUp;
                        // jump up
                        RB.AddForce(transform.up * jumpImpulseUpwards, ForceMode.Impulse);
                        // jump forwards
                        Vector3 local_forward = transform.forward;
                        // local_forward.y = 0; // given since player capsule is only rotated around y-axis
                        local_forward *= Mathf.Clamp01(Vector3.Dot(local_forward, RB.velocity.normalized));

                        RB.AddForce(local_forward * jumpImpulseForward, ForceMode.Impulse);
                        jump.Play();
                    }
                    break;
            }
            case MovementState.JumpingUp:
            {
                    if (flaploop.isPlaying) { flaploop.Stop(); }
                    // check if peak is reached (-> fall / flutter)
                    if (RB.velocity.y <= downSpeed)
                    {
                        // change state: pressed -> flutter | not pressed -> fall
                        State = isJumpKeyPressed ? MovementState.Fluttering : MovementState.Falling;
                    }
                    else if (isJumpKeyPressed)
                    {
                            // jump higher if space is pressed
                            // -> reduce gravity
                            Vector3 f = Physics.gravity * (reducedGravityFactor - 1f);
                            RB.AddForce(f);
                    }
                    else
                    {
                            // shallow jump if not pressed
                            // -> increase gravity
                            Vector3 f = Physics.gravity * (increasedGravityFactor - 1);
                            RB.AddForce(f);
                    }
                    break;
            }
            case MovementState.Falling:
            {
                    if (flaploop.isPlaying) { flaploop.Stop(); }
                    // check if on ground (-> walking)
                    if (isStandingOnGround())
                        State = MovementState.Walking;
                    else if (isJumpKeyPressed)
                    {
                        // check if space pressed (-> fluttering)
                        State = MovementState.Fluttering;
                        // reduce downwards momentum
                        RB.AddForce(new Vector3(0, -RB.velocity.y, 0) * RB.mass, ForceMode.Impulse);
                    }
                    else
                    {
                        // -> increase gravity to fall back down faster
                        Vector3 f = Physics.gravity * (increasedGravityFactor - 1);
                        RB.AddForce(f);
                    }
                break;
            }
            case MovementState.Fluttering:
            {
                    if (!flaploop.isPlaying) { flaploop.Play(); }
                    // check if on ground (-> walking)
                    if (isStandingOnGround())
                        State = MovementState.Walking;
                    else if (!isJumpKeyPressed)
                    {
                        // check if space released (-> falling)
                        State = MovementState.Falling;
                        // add force to get down faster
                        RB.AddForce(Vector3.down * fallImpulse * RB.mass, ForceMode.Impulse);
                    }
                    else
                    {
                        // -> reduce gravity
                        Vector3 f = Physics.gravity * (reducedGravityFactor - 1f);
                        RB.AddForce(f);
                    }
                    break;
            }
        }
        #endregion
        /*/
        //Flying
        if (Input.GetKey(KeyCode.Space))
        {
            if (isStandingOnGround())
            {
                // jump up
                Vector3 v = RB.velocity;
                v.y = upSpeed;
                RB.velocity = v;

            }
            if (RB.velocity.y < downSpeed)
            {
                Vector3 v = RB.velocity;
                v.y = downSpeed;
                RB.velocity = v;
            }
        }
        //*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HighScoreMenu.score = 0;
            HighScoreMenu.numberOfNiceMatches = 0;
            HighScoreMenu.numberOfPerfectMatches = 0;
            HighScoreMenu.miss = 0;
            SpawnRandom.t = SpawnRandom.number;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        //Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 vel = transform.forward * vertical * walkSpeed;
        vel += transform.right * horizontal * walkSpeed;

        vel.y = RB.velocity.y;

        if (Mathf.Abs(RB.velocity.x) > Mathf.Abs(vel.x))
            vel.x = RB.velocity.x;

        if (Mathf.Abs(RB.velocity.z) > Mathf.Abs(vel.z))
            vel.z = RB.velocity.z;

        RB.velocity = vel;

        rotX -= Input.GetAxis("Mouse Y") * lookSpeed * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * lookSpeed * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -90, 90);

        cam.localRotation = Quaternion.Euler(rotX,0,0);
        transform.rotation = Quaternion.Euler(0,rotY,0);


        if (State == MovementState.Walking)
        {
            float y = RB.velocity.y;
            vel = Vector3.Lerp(RB.velocity, Vector3.zero, Time.deltaTime * 5f);
            vel.y = y;
            RB.velocity = vel;
        }
	}

    private bool isStandingOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 2f, ground);
    }
}
