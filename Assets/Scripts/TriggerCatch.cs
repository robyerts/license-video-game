using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCatch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        Debug.Log(c.gameObject.tag + " - Inside catcher - SHOULDN'T GET HERE");
    }
}
