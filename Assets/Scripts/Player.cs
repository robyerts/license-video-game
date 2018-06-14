using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Character character;
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
    private bool isRepositioningFromHit;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform idleViewOrientation;
    [SerializeField] private Text nameLabel;
    private int attackNr = 0;

    [SerializeField] private SimpleHealthBar healthBar;
    [SerializeField] private int maxhealth = 100;
    private int currentHealth;
    [SerializeField] private SimpleHealthBar manaBar;
    [SerializeField] private int maxMana = 80;
    private int currentMana;

    [SerializeField] private List<string> abilitiesAnimNames;
    [SerializeField] private List<float> abilitiesAttackRanges;
    [SerializeField] private List<int> abilitiesManaCosts;
    [SerializeField] private List<Sprite> abilitiesIcons;
    [SerializeField] private List<int> abilitiesXpCosts;


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
        isRepositioningFromHit = false;
        currentHealth = maxhealth;
        currentMana = maxMana;
        manaBar.UpdateBar(currentMana, maxMana);
    }

    public void StartAttack(int attackNr)
    {
        GameManager.instance.ActivateGrayButtons();
        this.attackNr = attackNr;
        isAtacking = true;

        currentMana -= abilitiesManaCosts[attackNr];
        manaBar.UpdateBar(currentMana, maxMana);
        

        anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentEnemyTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAtacking && !attackAnimPlayed && Vector3.Distance(transform.position, currentEnemyTransform.position) < abilitiesAttackRanges[attackNr])
        {
            //Time.timeScale = 0.3f;
            navMeshAgent.isStopped = true;
            anim.SetBool("isRunning", false);
            anim.Play(abilitiesAnimNames[attackNr]);
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

        if (isRepositioningFromHit && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
        }
    }

    //for sorceress attack
    void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside Player OnCollisionEnter");
        int projectileIndex = GameManager.instance.enemyProjectilesTags.FindIndex(proj => proj.CompareTo(c.gameObject.tag) == 0);
        if (projectileIndex != -1)
        {
            anim.Play("Unarmed-GetHit-B1");
            currentHealth -= GameManager.instance.enemyProjectilesDmg[projectileIndex];
            healthBar.UpdateBar(currentHealth, maxhealth);
        }
    }

    public void Hit(int dmg)
    {
        RangedEnemy sorceressScript = currentEnemy.GetComponent<RangedEnemy>();
        MeleeEnemy bruteWarriorScript = currentEnemy.GetComponent<MeleeEnemy>();

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

    public void GetHit(int dmg)
    {
        Debug.Log("Inside RPG Character GetHit");
        rb.AddForce(-transform.forward * 10000, ForceMode.Impulse);

        currentHealth -= dmg; 
        healthBar.UpdateBar(currentHealth, maxhealth);
        if(currentHealth <= 0)
        {
            //anim.enabled = false;
            anim.Play("Unarmed-Death1");
            GameManager.instance.setGameOver();
        }
        else
        {
            anim.Play("Unarmed-GetHit-B1");
            StartCoroutine(RepositionFromHit());
        }
    }

    IEnumerator RepositionFromHit()
    {
        yield return new WaitForSeconds(0.5f);
        isRepositioningFromHit = true;
        //anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
    }

}
    