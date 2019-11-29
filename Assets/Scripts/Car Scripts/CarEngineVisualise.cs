using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarEngineVisualise : MonoBehaviour
{
    
    [Header("Internals")]
    private string objID;
    public string objectID
    {
        get { return objID; }
        set { objID = value; }
    }

    public BezierSplines path;
    private int firstFrame;
    private int lastFrame;
    public Dictionary<int, PositionRotationType> dataPoints;
    
    [TextArea]
    public string version = "2.0";
    private string originalTag;

    [Space]
    [Header("Car Profile")]
    [SerializeField]
    private CarProfile carProfile;
    public CarProfile Profile
    {
        set
        {
            carProfile = value;

        }
        get
        {
            return carProfile;
        }
    }
    [Space]
    [Header("Car Components")]
    public Transform cameraPos;
    public GameObject radio;
    public GameObject taxiDriver;
    private Vector3 positionToBe;
    private Vector3 directionToHave;
    private Rigidbody engine;
    private bool isPlaying = false;
    private bool isRunningAnEvent = false;

    [Space]
    [Header("Taxi Driver")]
    public bool isTaxiDriver = false;
    
    [Space]
    [Header("Velocity")]
    public float velocity = 0;
    public bool waitingForInitialization;
   
    public void setPaintColor(Color color)
    {


        List<Material> targets = new List<Material>();
        Renderer[] objectRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer objectRenderer in objectRenderers)
        {
            Material[] mats = objectRenderer.materials;
            foreach (Material mat in mats)
            {
                if (mat.name.Contains("carPaint"))
                {

                    targets.Add(mat);
                }
            }
        }
        if (targets.Count != 0)
        {
            foreach (Material target in targets)
            {
                target.SetColor("_Color", color);
            }
        }
    }
    public void Start()
    {
        
        
        originalTag = this.tag;
        if (carProfile.useRuntimeColor)
            setPaintColor(carProfile.paint);
        if (this.GetComponent<Rigidbody>() != null)
        {
            engine = GetComponent<Rigidbody>();
            engine.isKinematic = true;
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            engine = GetComponent<Rigidbody>();
            engine.isKinematic = true;
            engine.interpolation = RigidbodyInterpolation.Interpolate;
            engine.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
         }
        if (this.gameObject.GetComponent<MeshCollider>() != null)
        {
            Destroy(this.gameObject.GetComponent<MeshCollider>());
        }
        if (this.gameObject.GetComponent<BoxCollider>() == null)
        {
            this.gameObject.AddComponent<BoxCollider>();
        }  
        _initialize();

    }


    private void _initialize()
    {
        if (this.CompareTag("Event Object"))
        {
            if( WestdriveSettings.EventData.Data.ContainsKey(this.transform.parent.gameObject.name))
                dataPoints = WestdriveSettings.EventData.Data[this.transform.parent.gameObject.name];
            else
            {
                Destroy(this.gameObject);
                Destroy(this.transform.parent.gameObject);
            }
        }

        if (this.CompareTag("ADV"))
        {
            if(isTaxiDriver)
                EventManager.TriggerEvent("stop audio");
        }

        firstFrame = dataPoints.Keys.First();
        lastFrame = dataPoints.Keys.Last();
        //positionToBe = dataPoints[firstFrame].position;
        engine.position = dataPoints[firstFrame].position;
        engine.rotation = dataPoints[firstFrame].rotaion;
        
        

        isPlaying = true;
        

    }

    
    public void Update()
    {

            if (WestdriveSettings.isPlaying)
            {
                int currentFrame = TimeGaurd.getCurrentFrame();
                if (currentFrame > lastFrame)
                {
                    Destroy(this.gameObject);
                    if (this.CompareTag("Event Object"))
                    {
                        Destroy(this.transform.parent.gameObject);
                    }
                }
                else if (dataPoints.ContainsKey(currentFrame))
                {
                    if (WestdriveSettings.useInterpolate == true)
                    {
                        positionToBe = dataPoints[currentFrame].position;
                        Vector3 direction = positionToBe - engine.position;
                        Vector3 localDirection = transform.InverseTransformPoint(positionToBe);
                        float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
                        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, angle, 0));
                        engine.MovePosition(engine.position + (direction * Time.deltaTime));
                        engine.MoveRotation(engine.rotation * deltaRotation);
                    }
                    else
                    {
                        engine.position = dataPoints[currentFrame].position;
                        engine.rotation = dataPoints[currentFrame].rotaion;
                    }
                }
                

            }
    }

    

    private void OnTriggerEnter(Collider other)
    {

        //Event Gate
        if (other.GetComponent<EventHandler>() != null && this.originalTag == "ADV"
                                                       && other.GetComponent<EventHandler>().ADVPath == this.path 
                                                       && other.GetComponent<EventHandler>().needMotorControl)
        {
            isRunningAnEvent = true;       
            isPlaying = false;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<EventHandler>() != null && this.originalTag == "ADV" && other.GetComponent<EventHandler>().ADVPath == this.path)
        {
            if (other.GetComponent<EventHandler>().needMotorControl)
            {

                isRunningAnEvent = true;
                    
                isPlaying = false;
            }
            else if (!other.GetComponent<EventHandler>().needMotorControl)
            {
                isRunningAnEvent = false;
                    
                isPlaying = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EventHandler>() != null && this.originalTag == "ADV" && other.GetComponent<EventHandler>().ADVPath == this.path)
        {

            isRunningAnEvent = false;
                
            isPlaying = true;

        }
        
    }
    

    
    private void OnDestroy()
    {
        this.GetComponent<AudioSource>().Stop();
        StopAllCoroutines();
    }

    private void Pause()
    {
        isPlaying = false;
    }

    private void Play()
    {
        isPlaying = true;
    }
}
