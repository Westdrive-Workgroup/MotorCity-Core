using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
/// <summary>
/// Works with the character visualizer and enables the user to follow the carpaths after the simulation
/// </summary>
public class CharacterManagerReplay : MonoBehaviour
{
    Animator anim;
    [Header("Internals")]
    private string objID;
    public string objectID
    {
        get { return objID; }
        set { objID = value; }
    }
    public Dictionary<int, PositionRotationType> dataPoints;
    private int firstFrame;
    private int lastFrame;
    private int localFrameCorrection;
    private Rigidbody body;    
    [Space]
    [Header("Path Settings")]
    public BezierSplines path;
    private int defaultLayerID;
    private int avoidableLayerID;
    // Use this for initialization
    private bool isPlaying;
    //assorts the layers and animation, also starts the initialize Method
    void Start()
    {        
        defaultLayerID = gameObject.layer;
        avoidableLayerID = UnityEngine.LayerMask.NameToLayer("avoidable");
        anim = GetComponent<Animator>(); 
        _initialize();     
    }
    // Starts the Westdrive Settings
    void Update()
    {       
        //if ((Time.frameCount - localFrameCorrection) > lastFrame)
        //    isPlaying = false;        
        if (localFrameCorrection != WestdriveSettings.frameCorrection)
        {
            localFrameCorrection = WestdriveSettings.frameCorrection;
        }
        if (WestdriveSettings.isPlaying )
        {
            if (dataPoints.ContainsKey(Time.frameCount - localFrameCorrection))
            {
                anim.SetBool("move", true);
                anim.enabled = true;
                if (WestdriveSettings.useInterpolate == true)
                {
                    Vector3 positionToBe = dataPoints[Time.frameCount - localFrameCorrection].position;
                    Vector3 direction = positionToBe - body.position;
                    Vector3 localDirction = transform.InverseTransformPoint(positionToBe);
                    float angle = Mathf.Atan2(localDirction.x, localDirction.z) * Mathf.Rad2Deg;
                    Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, angle, 0));
                    body.MovePosition(body.position + (direction * Time.deltaTime));
                    body.MoveRotation(body.rotation * deltaRotation);
                }
                else
                {
                    body.position = dataPoints[(Time.frameCount - localFrameCorrection)].position;
                    body.rotation = dataPoints[(Time.frameCount - localFrameCorrection)].rotaion;
                }
            }
        }
    }
   // gets the rigidbody components
    private void _initialize()
    {
        if (this.gameObject.GetComponent<Rigidbody>() != null)
        {
            body = this.gameObject.GetComponent<Rigidbody>();
        }
        else
        {
            this.gameObject.AddComponent<Rigidbody>();
            body = this.gameObject.GetComponent<Rigidbody>();
            body.useGravity = false;
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;
        }
        firstFrame = dataPoints.Keys.First();
        lastFrame = dataPoints.Keys.Last();
        transform.position = dataPoints[firstFrame].position;
        transform.rotation = dataPoints[firstFrame].rotaion;     
    }
    //Stops the playing
    private void Pause()
    {
        isPlaying = false;
    }
    //starts the playing
    private void Play()
    {
        isPlaying = true;
    }
    //Stops all Coroutines
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
