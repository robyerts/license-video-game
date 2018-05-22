using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorceressAttack : MonoBehaviour {
    private Animator anim;
    private Rigidbody rb;
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform fireboltInstatiateTransfrom;

    [SerializeField] private SimpleHealthBar healthBar;
    [SerializeField] private int maxhealth = 40;
    private int currenthealth;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

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
        }
    }

    IEnumerator AttackCompleted()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.enemyFinishedAttack = true;
    }
}
