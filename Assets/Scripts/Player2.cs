using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Player2 : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private Transform currentEnemyTransform;
    public GameObject currentEnemy;
    private bool attackAnimPlayed;
    private bool hasToRotateBackwards;
    private bool hasToRotateForwards;
    private int rotationSpeed = 10;
    private bool isReturning;
    private bool isAtacking;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform idleViewOrientation;
    private int attackNr = 0;

    [SerializeField] private SimpleHealthBar healthBar;
    [SerializeField] private int maxhealth = 100;
    private int currentHealth;
    [SerializeField] private SimpleHealthBar manaBar;
    [SerializeField] private int maxMana = 80;
    private int currentMana;

    // 0 - basic attack
    // 1 - ability1
    // 2 - ability2 - not yet implemented
    //...

    // Use this for initialization
    void Start()
    {
        currentEnemyTransform = currentEnemy.transform;
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        isReturning = false;
        hasToRotateForwards = false;
        hasToRotateBackwards = false;
        attackAnimPlayed = false;
        isAtacking = false;
        currentHealth = maxhealth;
        currentMana = maxMana;
        manaBar.UpdateBar(currentMana, maxMana);
    }

    public void StartAttack(int attackNr)
    {
        GameManager.instance.ActivateGrayButtons();
        this.attackNr = attackNr;
        isAtacking = true;

        //should be replaced with a switch for mutiple attack abilities
        if(attackNr == 1)
        {
            currentMana -= 15;
            manaBar.UpdateBar(currentMana, maxMana);
        }

        anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentEnemyTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAtacking && !attackAnimPlayed && Vector3.Distance(transform.position, currentEnemyTransform.position) < 2)
        {
            navMeshAgent.isStopped = true;
            anim.SetBool("isRunning", false);
            switch(attackNr)
            {
                case 0:
                    anim.Play("Unarmed-Attack-R3");
                    break;
                case 1:
                    anim.Play("Unarmed-Attack-Kick-L1");
                    break;
            }
            
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
                GameManager.instance.enemiesAttacksStarted = true;
                // attack finished - back to idle position
                // reset the remaining variables
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    //for sorceress attack
    void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside Player OnCollisionEnter");
        if (c.gameObject.tag == "Firebolt")
        {
            anim.Play("Unarmed-GetHit-B1");
            currentHealth -= 10;
            healthBar.UpdateBar(currentHealth, maxhealth);
        }
    }

    public void Hit(int dmg)
    {
        SorceressAttack sorceressScript = currentEnemy.GetComponent<SorceressAttack>();
        GetCloseAttack2 bruteWarriorScript = currentEnemy.GetComponent<GetCloseAttack2>();

        if (sorceressScript)
        {
            sorceressScript.GetHit(dmg);
        } else
        {
            bruteWarriorScript.GetHit(dmg);
        }
    }

    public void EndHit()
    {
        hasToRotateBackwards = true;
        isAtacking = false;
    }

    public void GetHit()
    {
        Debug.Log("Inside RPG Character GetHit");
        rb.AddForce(-transform.forward * 10000, ForceMode.Impulse);
        anim.Play("Unarmed-GetHit-B1");

        currentHealth -= 30;
        healthBar.UpdateBar(currentHealth, maxhealth);
        if(currentHealth <= 0)
        {
            GameManager.instance.setGameOver();
        }
    }

}
    