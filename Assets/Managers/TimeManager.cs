using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    [HideInInspector]
    public int seconds;
    [HideInInspector]
    public int minutes;
    private int i;
    [HideInInspector]
    public bool paused;
    public int levelTimeInSeconds;
    public GameObject timeDisplay;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        //Time
        if (!paused)
        {
            i++;
            if (i == 60)
            {
                i = 0;
                levelTimeInSeconds--;
            }
            minutes = levelTimeInSeconds / 60;
            seconds = levelTimeInSeconds%60;
        }
        if (levelTimeInSeconds == 0)
        {
            SceneManager.LoadScene(2);
        }

        if (seconds < 10)
        {
            timeDisplay.GetComponent<Text>().text = minutes + " : 0" + seconds;
        }
        else
        {
            timeDisplay.GetComponent<Text>().text = minutes + " : " + seconds;
        }
        //Debug.Log("minutes" + minutes);
        //Debug.Log("seconds: " + seconds);
        //Debug.Log("Time in seconds: " + timeS);
    }
}
