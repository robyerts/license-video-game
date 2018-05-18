using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Transform enemyTransform;


    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        //Time.timeScale = 0.5f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside Player OnCollisionEnter");
        if (c.gameObject.tag == "BruteWarriorFist" || c.gameObject.tag == "Firebolt")
        {
            anim.Play("Unarmed-GetHit-B1");
            //anim.SetTrigger("gotHit");
        }
    }

}
