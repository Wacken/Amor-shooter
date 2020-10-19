using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour {

    public GameObject arrow;
    public LineRenderer[] lines;
    public Transform mid;
    private bool aiming;
    private Transform arrowBack;

	// Use this for initialization
	void Start () {
        StartCoroutine(loop());
	}

    IEnumerator loop()
    {
        while (true)
        {
            Arrow currentArrow = Instantiate(arrow, transform).GetComponent<Arrow>();
            currentArrow.transform.localPosition = Vector3.zero;
            currentArrow.transform.localRotation = Quaternion.identity;
            currentArrow.GetComponent<Rigidbody>().isKinematic = true;
            arrowBack = currentArrow.backPoint;
            aiming = true;
            while (!Input.GetMouseButton(0))
            {
                yield return new WaitForSeconds(0.02f);
            }
            float value = 0.01f;
            while (Input.GetMouseButton(0))
            {
                value += Time.deltaTime;
                if (value > 1)
                {
                    value = 1;
                }
                currentArrow.transform.localPosition = new Vector3(0,0,-value*0.35f);
                yield return new WaitForSeconds(0.02f);
            }
            currentArrow.Shoot(value);
            aiming = false;
            yield return new WaitForSeconds(1);
        }
    }

    void LateUpdate()
    {
        if (aiming)
        {
            LineAssign(arrowBack);
        }
        else
        {
            LineAssign(mid);
        }
    }

    void LineAssign(Transform t)
    {
        Vector3[] pos = new Vector3[2];
        pos[0] = lines[0].transform.position;
        pos[1] = t.position;
        lines[0].SetPositions(pos);
        pos[0] = lines[1].transform.position;
        lines[1].SetPositions(pos);
    }
}
