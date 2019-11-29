using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Destroy the event objects after the event terminates
/// </summary>
public class EventTermination : MonoBehaviour {
    public GameObject blockObject;
    public GameObject secondBlockObject;
    public GameObject trafficLight;
    public bool selfDestructOnEventEnd = true;
	// Event has run.
	void Start () {
        if (blockObject != null)
            blockObject.SetActive(true);
        if (secondBlockObject != null)
            secondBlockObject.SetActive(true);
        if (trafficLight != null)
            trafficLight.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // When any colliders enter the trigger that are Event Object,
    // destroy the event related block objects 
    // and set the traffic light back
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Event Object"))
        {
            if (blockObject != null)
                Destroy(blockObject);
            if (secondBlockObject != null)
                Destroy(secondBlockObject);
            if (trafficLight != null)
                trafficLight.SetActive(true);
            if (selfDestructOnEventEnd)
                Destroy(this.gameObject);
        }
    }
    // Disable the event related object when it collides. 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Event Object"))
        {
            collision.gameObject.SetActive(false);
        }
    }
}
