using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : MonoBehaviour {
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform fireboltInstatiateTransfrom;
    [SerializeField] private GameObject flametrowerPrefab;
    [SerializeField] private Transform flametrowerInstatiateTransfrom;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private SimpleHealthBar healthBar;
    [SerializeField] private int maxhealth = 3; // min 2
    [SerializeField] private int flameThrowerUnlikeabilityChance = 30; //TBC
                                                                       // add tooltip
    [SerializeField] private int flameThrowerDmg = 22;
    [SerializeField] private GameObject player;
    private bool isDead;
    private int currenthealth;
    private bool isRepositioningFromHit;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        isDead = false;
        isRepositioningFromHit = false;
        currenthealth = maxhealth;
        healthBar.UpdateBar(currenthealth, maxhealth);
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
            Debug.Log("reposition disabled - sorceress");
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
        }
        if (isDead)
        {
            // lerp the rotation such that the enemy is dea on the floor
        }
    }

    public void Hit()
    {
        System.Random rnd = new System.Random();
        int chance = rnd.Next(1, flameThrowerUnlikeabilityChance);
        if(chance == 1)
        {
            Instantiate(flametrowerPrefab, flametrowerInstatiateTransfrom.position, flametrowerInstatiateTransfrom.rotation);
            player.GetComponent<Player>().GetHit(flameThrowerDmg);
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

    public void GetHit(int dmg)
    {
        Debug.Log("Inside Sorceress GetHit");
        rb.AddForce(-transform.forward * 8000, ForceMode.Impulse);

        currenthealth -= dmg;
        healthBar.UpdateBar(currenthealth, maxhealth);
        if (currenthealth <= 0)
        {
            anim.enabled = false;
            GameManager.instance.RemoveDeadEnemy(this.gameObject);
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
        yield return new WaitForSeconds(1);
        GameManager.instance.enemyFinishedAttack = true;
    }

    IEnumerator RepositionFromHit()
    {
        yield return new WaitForSeconds(0.8f);
        //anim.SetBool("isRunning", true);
        isRepositioningFromHit = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
        yield return null;
    }

}
