using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class filmAvatars : MonoBehaviour {

    public Transform destination;
    // Use this for initialization
	void Start () {
		if(destination != null)
        {
            GetComponent<NavMeshAgent>().SetDestination(destination.position);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
