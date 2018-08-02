using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {
    public int rotationSpeed = 10;
    public SimpleHealthBar healthBar;
    public Transform initialTransform;
    public Transform idleViewOrientation;

    protected Animator anim;
    protected NavMeshAgent navMeshAgent;
    protected Rigidbody rb;

    protected int currentHealth;

    //used after getting hit
    protected bool hasToRotateForwards = false; // used also when melee character is returning from hitting the enemy
    protected bool isRepositioningFromHit = false;


    // Use this for initialization
    protected virtual void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        isRepositioningFromHit = false;
        hasToRotateForwards = false;
    }

    // Update is called once per frame
    protected virtual void Update () {
		
	}

    protected IEnumerator RepositionFromHit()
    {
        yield return new WaitForSeconds(0.5f);
        isRepositioningFromHit = true;
        //anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
    }

}
