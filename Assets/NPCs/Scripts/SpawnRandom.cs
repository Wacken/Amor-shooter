using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandom : MonoBehaviour {

    public static int number = 35;
    public static int t = number;
    public GameObject[] NPCs;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < number; i++)
        {
            int r = Random.Range(0, NPCs.Length);
            NPC n = Instantiate(NPCs[r]).GetComponent<NPC>();
            n.pathNumber = Random.Range(0, n.path.Length);
            n.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            n.item = new Vector2Int(Random.Range(0, 3), Random.Range(0, 2));
            n.attraction = new Vector2Int(Random.Range(0, 3), Random.Range(0, 2));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
