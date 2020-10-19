using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public bool isShot;
    public float speed;
    public GameObject explosion;
    public Transform backPoint;
    public GameObject[] uis;
    private NPC[] hit;
    public GameObject match, dead;

    private Rigidbody RB;

    private TrailRenderer trail;

	// Use this for initialization
	void OnEnable() {
        //Debug.Log("ENABLED");
        isShot = false;
        hit = new NPC[2];

        RB = GetComponent(typeof(Rigidbody)) as Rigidbody;

        trail = GetComponentInChildren(typeof(TrailRenderer)) as TrailRenderer;
        trail.enabled = false;
	}

    public void Shoot(float value)
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed * value;
        GetComponent<Rigidbody>().isKinematic = false;

        (GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem).Play();
        trail.enabled = true;

        isShot = true;
        transform.SetParent(null);
        StartCoroutine(prepareToDie(5));
    }

    IEnumerator prepareToDie(float timeLeftOnThisWorld)
    {
        yield return new WaitForSeconds(timeLeftOnThisWorld);
        destroyWithoutParticlesystem();
    }

    private void destroyWithoutParticlesystem()
    {
        Component ps = GetComponentInChildren(typeof(ParticleSystem));
        if(ps != null)
            ps.transform.SetParent(null);
        Destroy(gameObject);
    }


    private void Update()
    {
        if (!isShot) { return; }

        transform.LookAt(transform.position + RB.velocity);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isShot) { return; }
        Instantiate(explosion, transform.position, Quaternion.identity);
        if (other.GetComponent<NPC>()!=null)
        {
            if (hit[0] == null)
            {
                hit[0] = other.GetComponent<NPC>();
            }
            else if (hit[0] != other.GetComponent<NPC>())
            {
                hit[1] = other.GetComponent<NPC>();
                Debug.Log(hit[0].attraction + " "+hit[1].attraction);
                if (hit[0].attraction == hit[1].item || hit[0].item == hit[1].attraction)
                {
                    if (hit[0].attraction == hit[1].item && hit[0].item == hit[1].attraction)
                    {
                        hit[0].inLove = true;
                        hit[1].inLove = true;
                        Debug.Log("yay!!!!! :DDD");
                        HighScoreMenu.score += 100;
                        HighScoreMenu.numberOfPerfectMatches ++;
                        Instantiate(uis[0]);
                        Instantiate(match, hit[0].transform.position, Quaternion.identity);
                        Instantiate(match, hit[1].transform.position, Quaternion.identity);
                        Destroy(hit[0].gameObject);
                        Destroy(hit[1].gameObject);
                    }
                    else
                    {
                        hit[0].inLove = true;
                        hit[1].inLove = true;
                        Debug.Log("yay!!!!! :DDD");
                        HighScoreMenu.score += 10;
                        HighScoreMenu.numberOfNiceMatches++;
                        Instantiate(uis[1]);
                        Instantiate(match, hit[0].transform.position, Quaternion.identity);
                        Instantiate(match, hit[1].transform.position, Quaternion.identity);
                        Destroy(hit[0].gameObject);
                        Destroy(hit[1].gameObject);
                    }
                    SpawnRandom.t -= 2;
                }
                else
                {
                    Instantiate(uis[2]);
                    HighScoreMenu.score -= 10;
                    HighScoreMenu.miss++;
                    Debug.Log("nay  :(");
                    Instantiate(dead, hit[0].transform.position, Quaternion.identity);
                    Instantiate(dead, hit[1].transform.position, Quaternion.identity);
                    Destroy(hit[0].gameObject);
                    Destroy(hit[1].gameObject);
                    SpawnRandom.t -=2;
                }
                destroyWithoutParticlesystem();
            }
        }
        else
        {
            destroyWithoutParticlesystem();
        }
    }
}
