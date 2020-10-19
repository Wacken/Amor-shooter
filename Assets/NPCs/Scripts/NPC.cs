using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour {
    
    public Vector2Int item;
    /*
     * x: 
     *      0 -> head      y: 0 -> ?
     *      1 -> face      y: 0 -> ?
     *      2 -> torse     y: 0 -> ?
     */
    public Vector2Int attraction;
    public MeshRenderer colorAssign;
    public Gradient colorScheme;
    public Transform headParent, faceParent, bodyParent;
    public Image matchSymbol;
    public bool inLove;

    public PathObject[] path;
    [HideInInspector]
    public int pathAkt;
    public bool loop;
    public bool backtrack;
    [HideInInspector]
    public int pathNumber;

    private bool back = false;
    private NavMeshAgent nav;
    private Rigidbody RB;

    // Use this for initialization
    void Start () {
        RB = GetComponent<Rigidbody>();

        colorAssign.materials[1].color = colorScheme.Evaluate(Random.value);
        if (item.x == 0) { SetToParent(StaticItemList.HeadItems[item.y], headParent); }
        else if (item.x == 1) { SetToParent(StaticItemList.FaceItems[item.y], faceParent); }
        else { SetToParent(StaticItemList.BodyItems[item.y], bodyParent); }

        //Nav
        int r = Random.Range(0, path[pathNumber].points.transform.childCount);
        transform.position = path[pathNumber].points.transform.GetChild(r).transform.position;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = path[pathNumber].speed;
        pathAkt = r;
        nav.Warp(path[pathNumber].points.transform.GetChild(r).transform.position);
        nav.SetDestination(path[pathNumber].points.transform.GetChild(r).transform.position);
        loop = path[pathNumber].loop;

        //TODO matchsymbol assignen
        if (attraction.x == 0) { matchSymbol.sprite = StaticItemList.SymbolsHead[attraction.y]; }
        else if (attraction.x == 1) { matchSymbol.sprite = StaticItemList.SymbolsFace[attraction.y]; }
        else { matchSymbol.sprite = StaticItemList.SymbolsBody[attraction.y]; }
    }
	
	// Update is called once per frame
	void Update () {
        //Navigation
        nav.SetDestination(path[pathNumber].points.transform.GetChild(pathAkt).transform.position);
	    if (isOnTarget(path[pathNumber].points.transform.GetChild(pathAkt).transform.position, 0.5f))
	    {
            if (!loop && pathAkt == path[pathNumber].points.transform.childCount - 1)
            {
                back = true;
            }
            else if (pathAkt == 0)
            {
                back = false;
            }
            if (!back)
            {
                pathAkt++;
            }
            else
            {
                pathAkt--;
            }
	        if (loop)
	        {
	            pathAkt %= path[pathNumber].points.transform.childCount;
            }
        }
        if (RB.velocity.magnitude > 12)
        {
            RB.velocity = RB.velocity.normalized * 12;
        }
	}

    void SetToParent(GameObject g, Transform par)
    {
        Transform t = Instantiate(g).transform;
        t.SetParent(par);
        t.localPosition = new Vector3(0, 0, 0);
        t.localRotation = Quaternion.identity;
        t.localScale = new Vector3(1, 1, 1);
    }

    private bool isOnTarget(Vector3 v, float epsilon)
    {
        if (transform.position.x < v.x + epsilon && transform.position.x > v.x - epsilon && transform.position.z < v.z + epsilon && transform.position.z > v.z - epsilon)
        {
            return true;
        }
        return false;
    }
}
