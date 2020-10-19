using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeAway : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(fade());
	}

    IEnumerator fade()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        for (float a = 1; a > 0; a-=0.02f)
        {
            cg.alpha = a;
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(transform.root.gameObject);
    }
}
