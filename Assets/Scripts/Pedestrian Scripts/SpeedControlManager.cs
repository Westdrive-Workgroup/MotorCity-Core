using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// Can slow down the Car depending on th state
/// </summary>
public class SpeedControlManager : MonoBehaviour {
    public string state = "pass";
    public float newSpeed = 5;
    public float newBrakingTorque = 500;
    public float newIncreamentUnit = 0.0001f;
    // Sets all Gizmos to green at the start
    void Start () {
        Gizmos.color = Color.green;
	}
    // Draws all Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + new Vector3(0, 1, 0), 0.2f);
        
    }
    //Changes the Color to red if state is "halt" and to green if it is "pass"
    void Update () {
        if (state == "pass")
        {
            Gizmos.color = Color.green;
        }
        else if (state == "halt")
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Debug.LogWarning("State is not defined");
        }
    }
    //allows the car to keep its speed if state == pass
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponentInParent<CarEngine>()!= null)
        {

            if (state == "pass")
            {
                transform.parent.GetComponent<NavMeshObstacle>().enabled = true;
            }

        }
    }
    //deactivates obsticle if car is already through
    private void OnTriggerExit(Collider other)
    {
        
        if (other.GetComponentInParent<CarEngine>() != null)
        {
            //Debug.Log("Car exited");
            if (state == "pass")
            {
                transform.parent.GetComponent<NavMeshObstacle>().enabled = false;

            }
        }
    }
}
