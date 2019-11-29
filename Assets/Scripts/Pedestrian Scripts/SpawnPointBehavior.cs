using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Creates a Green Gizmo as a spawnPoint
/// </summary>
public class SpawnPointBehavior : MonoBehaviour {
    public float radious = 3f;
    public Color pointColor = Color.green;
    // Use this for initialization
    public void OnDrawGizmos()
    {
        Gizmos.color = pointColor;
        Gizmos.DrawWireSphere(this.transform.position, radious);
        
    }
   
}
