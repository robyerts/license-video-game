using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorceressAttack : MonoBehaviour {
    private Animator anim;
    [SerializeField] private GameObject fireboltPrefab;
    [SerializeField] private Transform fireboltInstatiateTransfrom;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        //anim.Play("Attack1");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hit()
    {
        Instantiate(fireboltPrefab, fireboltInstatiateTransfrom.position, fireboltInstatiateTransfrom.rotation);
    }
}
