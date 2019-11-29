using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Creates a Gizmo as spawn point for Pedestrians
/// </summary>
public class SetAsPedestrianDestination : MonoBehaviour {
    public Color iconColor = Color.magenta;
    public float radius = 0.5f;
    private void OnDrawGizmos()
    {
        Gizmos.color = iconColor;
        Gizmos.DrawSphere(this.transform.position, radius);
    }
}
