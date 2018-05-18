using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorceressAttack : MonoBehaviour {
    private Animator anim;
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform fireboltInstatiateTransfrom;
    [SerializeField] private int health = 40;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }

    public void StartBasicAttack()
    {
        anim.Play("Attack1");
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

    IEnumerator AttackCompleted()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.enemyFinishedAttack = true;
    }
}
