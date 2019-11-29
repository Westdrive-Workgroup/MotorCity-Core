using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 
/// </summary>
public class CarEngineReplay : MonoBehaviour
{
    [Header("Internals")]
    private string objID;
    public string objectID
    {
        get { return objID; }
        set { objID = value; }
    }
    
    public BezierSplines path;
    private int localFrameCorrection;
    
    public Dictionary<int, PositionRotationType> dataPoints;
    private int firstFrame;
    private int lastFrame;

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
   
    
    // Cars progress on the path
    private float progress;
    public float Progress
    {
        get
        {
            return (progress * 100f);
        }

    }
    private Rigidbody engine;
    private int layerID;

    private bool isPlaying = false;

    private bool isRunningAnEvent = false;

    
    [Space]
    [Header("Car Wheels")]
    public GameObject wheelFR;
    public GameObject wheelFL;
    public GameObject wheelRR;
    public GameObject wheelRL;
    public GameObject caliperWheelFR;
    public GameObject caliperWheelFL;


    
    [Space]
    [Header("Taxi Driver")]
    public bool isTaxiDriver = false;
    public GameObject SteeringWheel;
    public float turnMultiplier = 100;
    private Quaternion steerWheelOriginalRotation;
    [Space]
    [Header("Engine Sound")]
    private float engineNoisePitch;
    private bool isVisible;

    public bool Visible
    {
        get
        {
            return isVisible;
        }
        set
        {
            isVisible = value;
        }
    }
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
        this.GetComponent<AudioSource>().clip = carProfile.engineSound;
        this.GetComponent<AudioSource>().loop = true;
        this.GetComponent<AudioSource>().Play();
        engineNoisePitch = this.GetComponent<AudioSource>().pitch;
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
            engine.isKinematic = true;

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
        if (SteeringWheel != null)
        {
            steerWheelOriginalRotation = SteeringWheel.transform.localRotation;
        }
        firstFrame = dataPoints.Keys.First();
        lastFrame = dataPoints.Keys.Last();
        layerID = UnityEngine.LayerMask.NameToLayer(carProfile.avoidingLayerName);
        if (CompareTag("ADV"))
        {
           
            WestdriveSettings.frameCorrection = Time.frameCount - firstFrame;
            localFrameCorrection = WestdriveSettings.frameCorrection;
            WestdriveSettings.Progress = ((float)(Time.frameCount - localFrameCorrection) / (float)lastFrame);
            WestdriveSettings.isPlaying = true;
        }
        //positionToBe = dataPoints[firstFrame].position;
        engine.position = dataPoints[firstFrame].position;
        engine.rotation = dataPoints[firstFrame].rotaion;
        if (gameObject.tag != "ADV")
            initiateRenderers();
        else
            isVisible = true;
        

        isPlaying = true;
        

    }

    private void OnBecameInvisible()
    {

        if (originalTag != "ADV" && originalTag != "Event Object")
        {
            isVisible = false;
            Renderer[] objectRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer objectRenderer in objectRenderers)
            {
                if (objectRenderer.transform.name != this.transform.name)
                    objectRenderer.enabled = false;
            }
        }

    }
    private void initiateRenderers()
    {
        if (originalTag != "Event Object")
        {
            isVisible = false;
            Renderer[] objectRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer objectRenderer in objectRenderers)
            {
                if (objectRenderer.transform.name != this.transform.name)
                    objectRenderer.enabled = false;
            }
        }
        else
        {
            isVisible = true;
        }
    }
    private void OnBecameVisible()
    {

        if (originalTag != "ADV" && originalTag != "Event Object")
        {
            isVisible = true;
            Renderer[] objectRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer objectRenderer in objectRenderers)
            {
                objectRenderer.enabled = true;
            }
        }

    }
    public void Update()
    {
       
        //if ((Time.frameCount - localFrameCorrection) > lastFrame)
        //    isPlaying = false;
        if (localFrameCorrection != WestdriveSettings.frameCorrection)
        {
            localFrameCorrection = WestdriveSettings.frameCorrection;
        }
        if (!waitingForInitialization)
        {
            if (WestdriveSettings.isPlaying)
            {
                if (this.GetComponent<AudioSource>() != null)
                {
                    if (!this.GetComponent<AudioSource>().isPlaying)
                    {
                        this.GetComponent<AudioSource>().Play();
                    }
                }
                if (dataPoints.ContainsKey(Time.frameCount - localFrameCorrection))
                {
                    if (WestdriveSettings.useInterpolate == true)
                    {
                        positionToBe = dataPoints[Time.frameCount - localFrameCorrection].position;
                        Vector3 direction = positionToBe - engine.position;
                        Vector3 localDirection = transform.InverseTransformPoint(positionToBe);
                        float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
                        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, angle, 0));
                        engine.MovePosition(engine.position + (direction * Time.deltaTime));
                        engine.MoveRotation(engine.rotation * deltaRotation);
                    }
                    else
                    {
                        engine.position = dataPoints[(Time.frameCount - localFrameCorrection)].position;
                        engine.rotation = dataPoints[(Time.frameCount - localFrameCorrection)].rotaion;
                    }

                    velocity = (Vector3.Distance(dataPoints[(Time.frameCount - localFrameCorrection)].position,
                                   dataPoints[(Time.frameCount - 1 - localFrameCorrection)].position)) / Time.deltaTime;
                    if (velocity != 0 && isVisible)
                    {

                        wheelFL.transform.Rotate(Vector3.right, velocity);
                        wheelFR.transform.Rotate(Vector3.right, velocity);
                        wheelRL.transform.Rotate(Vector3.right, velocity);
                        wheelRR.transform.Rotate(Vector3.right, velocity);
                        EngineNoise();
                    }
                }

            }
            else
            {
                if (this.GetComponent<AudioSource>() != null )
                {
                    if (this.GetComponent<AudioSource>().isPlaying)
                    {
                        this.GetComponent<AudioSource>().Stop();
                    }
                }
            }

            if ( SteeringWheel != null && dataPoints.ContainsKey(Time.frameCount - localFrameCorrection))
            {

                float lastKnownAngle = 0;
                Vector3 localTurnVector = transform.InverseTransformPoint(dataPoints[((Time.frameCount - localFrameCorrection ) ) ].position);
                float angle = Mathf.Atan2(localTurnVector.x, localTurnVector.z) * Mathf.Rad2Deg;
                Quaternion deltaRotation = new Quaternion();
                
//                deltaRotation = Quaternion.Euler((new Vector3(0, 0, angle)* 20) * Time.deltaTime);
//                SteeringWheel.transform.rotation *= deltaRotation;
//                lastKnownAngle = angle;
                
                if ((angle * turnMultiplier) > 3 || (angle * turnMultiplier) < -3)
                {
                    deltaRotation = Quaternion.Euler((new Vector3(0, 0, -angle)* 20) * Time.deltaTime);
                    SteeringWheel.transform.localRotation *= deltaRotation;
                }
                else if(SteeringWheel.transform.localRotation != steerWheelOriginalRotation)
                {
                    SteeringWheel.transform.localRotation = Quaternion.RotateTowards(SteeringWheel.transform.localRotation, steerWheelOriginalRotation, 1);
                }            
                
            }
            if (CompareTag("ADV"))
            {
                WestdriveSettings.Progress = ((float)(Time.frameCount - localFrameCorrection) / (float)lastFrame);
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
    void EngineNoise()
    {
        this.GetComponent<AudioSource>().pitch = engineNoisePitch;
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
