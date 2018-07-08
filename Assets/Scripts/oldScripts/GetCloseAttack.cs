using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetCloseAttack : MonoBehaviour {
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    [SerializeField] private List<BoxCollider> fistsColliders;
    [SerializeField] private GameObject player;
    private Animator anim;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform idleViewOrientation;
    private bool hasToRotateBackwards;
    private bool hasToRotateForwards;
    private int rotationSpeed = 10;
    private bool isReturning;
    

    // Use this for initialization
    void Start () {
        playerTransform = player.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        isReturning = false;
        hasToRotateForwards = false;
        hasToRotateBackwards = false;

        // warrior set to attack from start
        anim.SetBool("isRunning", true);
        navMeshAgent.SetDestination(playerTransform.position);
       
    }
	
	// Update is called once per frame
	void Update () {
        if (hasToRotateBackwards)
        {
            //Debug.Log(Time.deltaTime);
            Vector3 targetDir = initialTransform.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            if (newDir == transform.forward)
            {
                hasToRotateBackwards = false;
                anim.SetBool("isRunning", true);
                navMeshAgent.SetDestination(initialTransform.position);
                isReturning = true;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        if(isReturning && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            hasToRotateForwards = true;
        }
        if (hasToRotateForwards)
        {
            //Debug.Log(Time.deltaTime);
            Vector3 targetDir = idleViewOrientation.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            if (newDir == transform.forward)
            {
                hasToRotateForwards = false;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    void OnTriggerEnter(Collider c)
    {
        Debug.Log(c.gameObject.tag + " - Inside GetCloseAttack OnTriggerEnter");
        if (c.gameObject.tag == "EnemyAttackRange")
        {
            Time.timeScale = 0.3f;
            navMeshAgent.isStopped = true;
            anim.SetBool("isRunning", false);
            //foreach(BoxCollider collider in fistsColliders)
            //{
            //    collider.enabled = true;
            //}
            anim.Play("Attack1");
        }
    }

    public void EndHit()
    {
        //foreach (BoxCollider collider in fistsColliders)
        //{
        //    collider.enabled = false;
        //}
        hasToRotateBackwards = true;
        Time.timeScale = 1;
    }

    public void Hit()
    {
        Debug.Log("Hit function-event properly called"); 
    }
}
