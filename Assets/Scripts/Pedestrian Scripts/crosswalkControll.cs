using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// controls the speed of the pedestrians nearing a corsswalk
/// </summary>
public class crosswalkControll : MonoBehaviour {

    private GameObject speedControl;
    private GameObject speedControlOtherSide;
    void Start () {
        speedControl = transform.GetChild(0).gameObject;
        speedControl.GetComponent<BoxCollider>().isTrigger = true;
        //speedControlOtherSide = transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CharacterManager>() != null)
        {
            
            if (this.GetComponent<NavMeshObstacle>().enabled == false)
            {
                speedControl.GetComponent<SpeedControlManager>().gameObject.tag = "Pedestrian";
                speedControl.GetComponent<SpeedControlManager>().state = "halt";
                speedControl.GetComponent<BoxCollider>().isTrigger = false;
            }
                //speedControl.GetComponent<BoxCollider>().isTrigger = false;
            //speedControlOtherSide.GetComponent<SpeedControlManager>().state = "halt";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterManager>() != null)
        {
            speedControl.GetComponent<SpeedControlManager>().gameObject.tag = "crossPathSpeedControll";
            speedControl.GetComponent<SpeedControlManager>().state = "pass";
            speedControl.GetComponent<BoxCollider>().isTrigger = true;
            //speedControlOtherSide.GetComponent<SpeedControlManager>().state = "pass";
        }
    }
}
