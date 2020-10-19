using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutMenu : MonoBehaviour {

    public GameObject g;

    public void Switch()
    {
        g.SetActive(true);
    }

    public void LoadLevel(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
