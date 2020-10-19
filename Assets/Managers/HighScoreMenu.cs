using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HighScoreMenu : MonoBehaviour
{

    public static int numberOfPerfectMatches = 0,
        numberOfNiceMatches = 0,
        score = 0,
        miss = 0;

    public Text numOfPerfectMatchesDisplay;
    public Text numOfNiceMatchesDisplay;
    public Text scoreDisplay;
    public Text missDisplay;

	// Use this for initialization
	void Start ()
    {
        if (Application.loadedLevel == 2)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        numOfPerfectMatchesDisplay.text = numberOfPerfectMatches.ToString();
        numOfNiceMatchesDisplay.text = numberOfNiceMatches.ToString();
        scoreDisplay.text = score.ToString();
	    missDisplay.text = miss.ToString();
    }

    public void LoadLevel(string sceneName)
    {
        score = 0;
        numberOfNiceMatches = 0;
        numberOfPerfectMatches = 0;
        miss = 0;
        SpawnRandom.t = SpawnRandom.number;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        scoreDisplay.text = score.ToString("0");
        numOfNiceMatchesDisplay.text = numberOfNiceMatches.ToString();
        numOfPerfectMatchesDisplay.text = numberOfPerfectMatches.ToString();
        missDisplay.text = miss.ToString();
        //if (SpawnRandom.t <= 0)
        //{

        //    SceneManager.LoadScene(2);
        //}
        //if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level")
        //    score += 1;

    }
}
