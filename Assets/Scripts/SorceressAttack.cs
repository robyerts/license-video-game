using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SorceressAttack : MonoBehaviour {
    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform fireboltInstatiateTransfrom;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private SimpleHealthBar healthBar;
    [SerializeField] private int maxhealth = 40;
    private int currenthealth;
    private bool isRepositioningFromHit;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        isRepositioningFromHit = false;
        currenthealth = maxhealth;
        healthBar.UpdateBar(currenthealth, maxhealth);
    }

    public void StartBasicAttack()
    {
        if(anim.enabled == true)
        {
            anim.Play("Attack1");
        } else
        {
            GameManager.instance.enemyFinishedAttack = true;
        }
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
    }

    public void Hit()
    {
        Instantiate(fireboltPrefab, fireboltInstatiateTransfrom.position, fireboltInstatiateTransfrom.rotation);
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
        } else
        {
            StartCoroutine(RepostionFromHit());
        }
    }

    IEnumerator AttackCompleted()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.enemyFinishedAttack = true;
    }

    IEnumerator RepostionFromHit()
    {
        yield return new WaitForSeconds(0.5f);
        //anim.SetBool("isRunning", true);
        isRepositioningFromHit = true;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
        yield return null;
    }

}
