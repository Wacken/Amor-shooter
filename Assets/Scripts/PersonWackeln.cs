using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonWackeln : MonoBehaviour {

    public float speed;
    public float ausschlag;

	// Use this for initialization
	void Start ()
    {
        speed = Random.Range(3, 8f);
        ausschlag = Random.Range(4, 7f);
    }
	
	// Update is called once per frame
	void Update () {
        transform.localRotation = Quaternion.Euler(0,0,Mathf.Sin(Time.time * speed) * ausschlag);
	}
}
