using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;
/// <summary>
/// Resets the camera position
/// </summary>
public class CameraReset : MonoBehaviour
{
 
    // Update is called once per frame
    void Update()
    {
        //transform.position = transform.root.position;
        InputTracking.Recenter();
    }
}
