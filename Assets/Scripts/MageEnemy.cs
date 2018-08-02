using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MageEnemy : MonoBehaviour {
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform fireboltInstatiateTransfrom;
    [SerializeField] private GameObject flametrowerPrefab;
    [SerializeField] private Transform flametrowerInstatiateTransfrom;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform idleViewOrientation;
    [SerializeField] private SimpleHealthBar healthBar;
    [SerializeField] private int maxHealth = 40; 
    [SerializeField] private int flameThrowerUnlikeabilityChance = 30; //TBC
                                                                       // add tooltip
    [SerializeField] private int flameThrowerDmg = 22;
    [SerializeField] private GameObject player;
    private int rotationSpeed = 10;
    private bool hasToRotateForwards = false;
    private bool isDead;
    private int currentHealth;
    private bool isRepositioningFromHit;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        isDead = false;
        isRepositioningFromHit = false;
        hasToRotateForwards = false;
        currentHealth = maxHealth;
        healthBar.UpdateBar(currentHealth, maxHealth);
    }

    public void StartBasicAttack()
    {
        if (GameManager.instance.gameOver)
        {
            return;
        }
        if (anim.enabled == true)
        {
            anim.Play("Attack1");
        }
        //else
        //{
        //    GameManager.instance.enemyFinishedAttack = true;
        //}
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log(Vector3.Distance(transform.position, initialTransform.position));
        if (isRepositioningFromHit && (Vector3.Distance(transform.position, initialTransform.position) < 0.1f))
        {
            //Debug.Log("reposition disabled - sorceress");
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
            hasToRotateForwards = true;
            GameManager.instance.enemiesAttacksOnGoing = true;
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
        if (isDead)
        {
            // lerp the rotation such that the enemy is dead on the floor
        }
    }

    public void Hit()
    {
        System.Random rnd = new System.Random();
        int chance = rnd.Next(1, flameThrowerUnlikeabilityChance);
        // use comulative probability algorithm
        // https://stackoverflow.com/questions/9330394/how-to-pick-an-item-by-its-probability
        if (chance == 1)
        {
            Instantiate(flametrowerPrefab, flametrowerInstatiateTransfrom.position, flametrowerInstatiateTransfrom.rotation);
            player.GetComponent<MagePlayer>().GetHit(flameThrowerDmg);
            // implement small procedure for delaying the dmg from the flamethrower
        }
        else
        {
            Instantiate(fireboltPrefab, fireboltInstatiateTransfrom.position, fireboltInstatiateTransfrom.rotation);
        }
    }

    public void EndHit()
    {
        StartCoroutine(AttackCompleted());
    }

    void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside Sorceress OnCollisionEnter");
        int projectileIndex = GameManager.instance.enemyProjectilesTags.FindIndex(proj => proj.CompareTo(c.gameObject.tag) == 0);
        if (projectileIndex != -1)
        {
            currentHealth -= GameManager.instance.enemyProjectilesDmg[projectileIndex];
            healthBar.UpdateBar(currentHealth, maxHealth);
            StartCoroutine(RepositionFromHit());
        }
    }

    public void GetHit(int dmg, int force)
    {
        Debug.Log("Inside Sorceress GetHit");
        rb.AddForce(-transform.forward * force, ForceMode.Impulse);

        currentHealth -= dmg;
        healthBar.UpdateBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
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

    IEnumerator AttackCompleted()
    {
        yield return new WaitForSeconds(1); //hardcoded number -> expose it to the editor
                                            // add an array with time to complete 
        GameManager.instance.enemyFinishedAttack = true;
    }

    IEnumerator RepositionFromHit()
    {
        yield return new WaitForSeconds(0.8f);
        //anim.SetBool("isRunning", true);
        isRepositioningFromHit = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
    }

}
