using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Enables the user to use a not so detailed version of the city to rewatch the car and pedestrian paths
/// </summary>
public class CharacterManagerVisualise : MonoBehaviour
{
    Animator anim;
    [Header("Internals")]
    private string objID;
    public string objectID
    {
        get { return objID; }
        set { objID = value; }
    }
    private int firstFrame;
    private int lastFrame;
    public Dictionary<int, PositionRotationType> dataPoints;    
    private Rigidbody body;    
    [Space]
    [Header("Path Settings")]
    public BezierSplines path;    
    // Use this for initialization
    private bool isPlaying;
    // gets the animator and starts the initialize method
    void Start()
    { 
        anim = GetComponent<Animator>();
        _initialize();
    }
    // updates the current positions of the objects
    void Update()
    {
        if (WestdriveSettings.isPlaying)
        {
            anim.SetBool("move", true);
            anim.enabled = true;
            int currentFrame = TimeGaurd.getCurrentFrame();
            if (currentFrame > lastFrame)
            {
                Destroy(this);
                if (this.CompareTag("Event Object"))
                {
                    Destroy(this.transform.parent.gameObject);
                }
            }
            else if (dataPoints.ContainsKey(currentFrame))
            {
                if (WestdriveSettings.useInterpolate == true)
                {
                    Vector3 positionToBe = dataPoints[currentFrame].position;
                    Vector3 direction = positionToBe - body.position;
                    Vector3 localDirction = transform.InverseTransformPoint(positionToBe);
                    float angle = Mathf.Atan2(localDirction.x, localDirction.z) * Mathf.Rad2Deg;
                    Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, angle, 0));
                    body.MovePosition(body.position + (direction * Time.deltaTime));
                    body.MoveRotation(body.rotation * deltaRotation);

                }
                else
                {
                    body.position = dataPoints[currentFrame].position;
                    body.rotation = dataPoints[currentFrame].rotaion;
                }
            }
            
        }
    }
   // initializes the Rigidbody components
    private void _initialize()
    {
        if (this.CompareTag("Event Object"))
        {
            switch (this.transform.parent.gameObject.name)
            {
                case "Event1.1":
                    anim.SetInteger("Condition", 1);
                    break;
                case "Event2.1":
                    anim.SetInteger("Condition", 1);
                    break;
                case "Event2.2":
                    anim.SetInteger("Condition", 2);
                    break;
                case "Event3.1":
                    anim.SetInteger("Condition", 3);
                    break;
                case "Event3.2":
                    anim.SetInteger("Condition", 4);
                    break;
                case "Event3.4":
                    anim.SetInteger("Condition", 1);
                    break;
                case "Event4.1 (MSW)":
                    anim.SetInteger("Condition", 1);
                    break;
                case "Event4.3 (MSW)":
                    anim.SetInteger("Condition", 3);
                    break;
                default:

                    break;
            }
            anim.enabled = true;
            if( WestdriveSettings.EventData.Data.ContainsKey(this.transform.parent.gameObject.name))
                dataPoints = WestdriveSettings.EventData.Data[this.transform.parent.gameObject.name];
            else
            {
                Destroy(this.gameObject);
                Destroy(this.transform.parent.gameObject);
            }
        }
        else
        {
            anim.SetBool("move", true);
            anim.enabled = true;
        }
        firstFrame = dataPoints.Keys.First();
        lastFrame = dataPoints.Keys.Last();
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
        
        transform.position = dataPoints[firstFrame].position;
        transform.rotation = dataPoints[firstFrame].rotaion;
           
    }
    //
    private void Pause()
    {
        isPlaying = false;
    }

    private void Play()
    {
        isPlaying = true;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
