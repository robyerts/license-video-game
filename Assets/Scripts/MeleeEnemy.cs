using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private Transform playerTransform;
    [SerializeField] private GameObject player;
    private Animator anim;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform idleViewOrientation;
    private bool attackAnimPlayed;
    private bool hasToRotateBackwards;
    private bool hasToRotateForwards;
    private int rotationSpeed = 10;
    private bool isReturning;
    private bool isAtacking;
    private bool isRepositioningFromHit;
    private bool isDead;

    [SerializeField] private int maxhealth = 60;
    private int currenthealth;

    [SerializeField] private SimpleHealthBar healthBar;

    // Use this for initialization
    void Start()
    {
        playerTransform = player.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        isReturning = false;
        hasToRotateForwards = false;
        hasToRotateBackwards = false;
        attackAnimPlayed = false;
        isAtacking = false;
        isRepositioningFromHit = false;
        isDead = false;

        currenthealth = maxhealth;
        healthBar.UpdateBar(currenthealth, maxhealth);

    }

    public void StartBasicAttack()
    {
        isAtacking = true;
        anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(playerTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAtacking && !attackAnimPlayed && Vector3.Distance(transform.position, playerTransform.position) < 4)
        {
            navMeshAgent.isStopped = true;
            anim.SetBool("isRunning", false);
            anim.Play("Attack1");
            attackAnimPlayed = true;
        }

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
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(initialTransform.position);
                isReturning = true;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        if (isReturning && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            hasToRotateForwards = true;
            isReturning = false;
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
                attackAnimPlayed = false;
                GameManager.instance.enemyFinishedAttack = true;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        if (isRepositioningFromHit && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
        }
        if (isDead)
        {
            // lerp the rotation such that the enemy is dea on the floor
        }
    }


    public void EndHit()
    {
        hasToRotateBackwards = true;
        isAtacking = false;
    }

    public void GetHit(int dmg)
    {
        Debug.Log("Inside Brute Warrior GetHit");
        rb.AddForce(-transform.forward * 8000, ForceMode.Impulse);

        currenthealth -= dmg;
        healthBar.UpdateBar(currenthealth, maxhealth);

        if (currenthealth <= 0)
        {
            anim.enabled = false;
            GameManager.instance.RemoveDeadEnemy(this.gameObject);
            isDead = true;
            StartCoroutine(SelfDisable());

        } else
        {
            StartCoroutine(RepositionFromHit());
        }
    }

    IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(0.8f);
        //ugly code dependand on the simple Health bar framework
        healthBar.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        
    }

    IEnumerator RepositionFromHit()
    {
        yield return new WaitForSeconds(0.8f);
        isRepositioningFromHit = true;
        //anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
    }

    public void Hit(int dmg)
    {
        player.GetComponent<Player>().GetHit(dmg);
    }
}
