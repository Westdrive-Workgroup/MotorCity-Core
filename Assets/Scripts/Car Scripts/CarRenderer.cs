using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CarRenderer : MonoBehaviour {

    // Use this for initialization
    public CarEngine car;
    void awake()
    {
        if (car == null)
        {
            Debug.LogError("carEngine should be set for the script to work correctly");
        }
    }
    private void OnBecameVisible()
    {
        Debug.Log("Visible");
        car.Visible = true;
    }
    private void OnBecameInvisible()
    {
        Debug.Log("invisible");
        car.Visible = false;
    }
}
