using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarProfile : ScriptableObject
{
    public string profileName = "New Car Profile";
    public string tag = "Active Car";
    [Space]
    [Header("Debug - Avoidance Sensor")]
    public bool debugAlwaysShowCast = false;
    public bool debugShowCastOnHit = false;

    public bool debugGUI = false;
    public bool debugScene = false;
    [Space]
    [Header("Setup parameters")]
    [Tooltip("if enabled, cars will not follow physical simulation and will use Kinematic simulation")]
    public bool nonPhysicalSimulation = true;
    public bool useRuntimeColor = false;
    public Color paint;
    [Space]
    [Header("Path settings")]
    public bool usePathDefaultDuration = true;
    [Tooltip("Cars head follow the direction of the path")]
    public bool lookForward = true;
    [Tooltip("Sets if the car is going forward in the path or going back from the end of the path")]
    public bool goingForward = true;
    [Space]
    [Header("Car Body")]
    public GameObject CarBody;

    [Space]
    [Header("Car Sensors")]
    public string avoidingLayerName = "avoidable";
    public float avoidanceSenrosLength = 30f;
    public float avoidanceCriticalDistance = 5f;
    public float turningAvoidanceTreshold = 70;
    public float debugHitDistance = 10f;
    [Space]
    [Header("Engine sound")]
    public AudioClip engineSound;
}
