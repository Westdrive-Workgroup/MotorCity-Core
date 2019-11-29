using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// You can define the to be analyzabel data parameters
/// </summary>
public class AnalyzableData
{
    public int frameNumber;
    public Vector3 trackerPosition;
    public Quaternion trackerRotation;
    public string centerHit;
    public Vector3 centerHitPosition;
    public string centerHitGroup;
    public string boxHit;
    public Vector3 boxHitPosition;
    public string boxHitGroup;
    public string presentObjectName;
    public string presentObjectGroup;
}
