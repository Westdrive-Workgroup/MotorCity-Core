using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Regulates the movement of the cars in the city
/// k'**' are kinematic forms of movement
/// </summary>
public class CarEngine : MonoBehaviour
{
    [Header("Internals")]
    private string objID;
    public string objectID
    {
        get { return objID; }
        set { objID = value;  }
    }


    private Dictionary<int, PositionRotationType> dataPoints;

    private float angleBetween;
    [TextArea]
    public string version = "2.0";
    private string originalTag;
    //Initiates the Car
    [Space]
    [Header("Car Profile")]
    [SerializeField]
    private CarProfile carProfile;
    public CarProfile Profile
    {
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
    //Sets the path for the car using bezier splines for a smooth transition
    [Space]
    [Header("Path Settings")]
    public BezierSplines path;
    public bool isLoop = false;
    [Tooltip("initial duration for the car to complete the path")]
    
    public float duration = 0;
    private float increamentUnit = 0;
    private Vector3 positionToBe;
    private Vector3 directionToHave;
    public float currentIncreament
    {
        get
        {
            return increamentUnit;
        }
    }
    public float startPecentage = 0;
    private float pastIncreamentUnit;
    public float initialIncreament
    {
        get
        {
            return pastIncreamentUnit;
        }
    }
    // Cars progress on the path and speed
    private float progress;
    public float Progress
    {
        get
        {
            return (progress * 100f);
        }

    }
    //  Declares Booleans
    private Rigidbody engine;
    private int layerID;
    
    private bool isEngineOn = false;

    private bool isRunningAnEvent = false;
    
    private bool stopped = false;
    private bool accelerated = false;
    //Declares Wheels
    [Space]
    [Header("Car Wheels")]
    public GameObject wheelFR;
    public GameObject wheelFL;
    public GameObject wheelRR;
    public GameObject wheelRL;
    public GameObject caliperWheelFR;
    public GameObject caliperWheelFL;




//    check on collision
    private string hittedObject;
    public string hittedObjectName
    {
        get
        {
            return hittedObject;
        }
    }
    public float debugHitDistance = 10f;
    private float CollisionDistance = 0;
    public float hitDistance
    {
        get
        {
            return debugHitDistance;
        }
    }
    private bool isAvoiding = false;
    public bool Avoiding
    {
        get
        {
            return isAvoiding;
        }
    }
    //check on Traffic lights
    [Space]
    [Header("Traffic Light")]
    private bool triggerLock = false;
    public bool StoppedByTrafficLight
    {
        get
        {
            return triggerLock;
        }
    }
    //If the Taxidriver Condition is used, initiate the steeringwheel and steeringwheel rotation
    [Space]
    [Header("Taxi Driver")]
    public bool isTaxiDriver = false;
    public GameObject SteeringWheel;
    public float turnMultiplier = 100;
    private Quaternion steerWheelOriginalRotation;
    //Enables the enginesound
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
    //ueses Gizmo to debug in scene drawing of the sensors
    [Space]
    [Header("Velocity")]
    public float velocity = 0;
    public bool waitingForInitialization;
    private void OnDrawGizmos()
    {
        if (carProfile.debugAlwaysShowCast)
        {
            Gizmos.color = Color.green;
            Vector3 startPos = transform.position;
            //draw middle sensor
            startPos += transform.forward * this.GetComponent<BoxCollider>().bounds.size.z/2;
            startPos += transform.up * this.GetComponent<BoxCollider>().bounds.size.y / 2;
            Gizmos.DrawLine(startPos, startPos + (transform.forward * carProfile.avoidanceSenrosLength));
            //Draw right corner sensor
            startPos = transform.position;
            startPos += transform.forward * this.GetComponent<BoxCollider>().bounds.size.z / 2;
            startPos += transform.up * this.GetComponent<BoxCollider>().bounds.size.y / 2;
            startPos += transform.right * this.GetComponent<BoxCollider>().bounds.size.x / 2;
            Gizmos.DrawLine(startPos, startPos + (transform.forward * carProfile.avoidanceSenrosLength));
            //Draw left corner sensor
            startPos = transform.position;
            startPos += transform.forward * this.GetComponent<BoxCollider>().bounds.size.z / 2;
            startPos += transform.up * this.GetComponent<BoxCollider>().bounds.size.y / 2;
            startPos += transform.right * -1 * this.GetComponent<BoxCollider>().bounds.size.x / 2;
            Gizmos.DrawLine(startPos, startPos + (transform.forward * carProfile.avoidanceSenrosLength));
            
        }

    }
    // sets the cars color
    public void setPaintColor(Color color)
    {
        
        
        List<Material> targets = new List<Material>();
        Renderer [] objectRenderers = GetComponentsInChildren<Renderer>();
        foreach( Renderer objectRenderer in objectRenderers)
        {
            Material[] mats = objectRenderer.materials;
            foreach (Material mat in mats)
            {
                if(mat.name.Contains("carPaint"))
                {
                    
                    targets.Add(mat);
                }
            }
        }
        if(targets.Count != 0)
        {
            foreach (Material target in targets)
            {
                target.SetColor("_Color", color);
            }
        }
    }
    //checks if car is ready to start and starts it
    public void Awake()
    {
        if (this.CompareTag("Event Object"))
        {
            this.GetComponent<AudioSource>().Stop();
        }
    }
    public void Start()
    {
        dataPoints = new Dictionary<int, PositionRotationType>();
        
        originalTag = this.tag;
        engineNoisePitch = this.GetComponent<AudioSource>().pitch;
        if (carProfile.useRuntimeColor)
            setPaintColor(carProfile.paint);
        if (this.GetComponent<Rigidbody>() != null)
        {
            engine = GetComponent<Rigidbody>();
            if (carProfile.nonPhysicalSimulation)
                engine.isKinematic = true;
            else
                engine.isKinematic = false;
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            engine = GetComponent<Rigidbody>();
            engine.isKinematic = true;
            engine.interpolation = RigidbodyInterpolation.Interpolate;
            engine.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            if (carProfile.nonPhysicalSimulation)
                engine.isKinematic = true;
            else
                engine.isKinematic = false;

        }
        if (carProfile.nonPhysicalSimulation)
        {
            
            if (carProfile.usePathDefaultDuration)
                this.duration = path.duration;

        }
        else
        {
            if (this.gameObject.GetComponent<MeshCollider>() != null)
            {
                Destroy(this.gameObject.GetComponent<MeshCollider>());
            }
            if (this.gameObject.GetComponent<BoxCollider>() == null)
            {
                this.gameObject.AddComponent<BoxCollider>();
            }
        }
        _initialize();
       
    }

    //Sets the speed and direction of the car
    private void _initialize()
    {
        
        
        layerID = UnityEngine.LayerMask.NameToLayer(carProfile.avoidingLayerName);
        if(CompareTag("ADV"))
        {
            WestdriveSettings.Progress = progress;
        }
        increamentUnit = (Time.fixedDeltaTime) / duration;
        progress = startPecentage;
        positionToBe = path.GetPoint(startPecentage);
        directionToHave = positionToBe + path.GetDirection(startPecentage);
        KSaveMotorValues();

        transform.localPosition = positionToBe;
        transform.LookAt(directionToHave);
        
        if (gameObject.tag != "ADV")
            initiateRenderers();
        else
            isVisible = true;
        // improve
        if(this.GetComponent<AVAS>() != null)
        {
            waitingForInitialization = true;
            isEngineOn = false;
 
        }
        else
        {
            waitingForInitialization = false;
            isEngineOn = true;
        }
        if (this.CompareTag("Event Object"))
        {
            this.GetComponent<AudioSource>().PlayDelayed(0.5f);
        }
        if(WestdriveSettings.SimulationMode == mode.record)
            StartCoroutine(recorder());
        if (WestdriveSettings.SimulationMode == mode.simulate && this.CompareTag("Event Object"))
        {
            StartCoroutine(EventRecorder());
        }
        

    }
   // disables the mesh of non ADV and non Event Objects making them invisible
    private void OnBecameInvisible()
    {
        
        if (originalTag != "ADV" && originalTag != "Event Object" )
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

    
    // disables the mesh of non Event Objects making them invisible
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
    // enables the mesh of non Event Objects making them visible
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
        
        if (!waitingForInitialization)
        {
            //creats engine noise if car is visible
            if (isVisible)
                EngineNoise();
            //adjusts the steeringwheel position corresponding to the path
            if (isTaxiDriver && SteeringWheel != null)
            {
                //float newSteer = Vector3.SignedAngle(transform.forward, path.GetDirection(progress + 3*increamentUnit), Vector3.up);
                Vector3 turnVector = path.GetDirection(progress + increamentUnit) - engine.position;
                Vector3 localTurnVector = transform.InverseTransformPoint(positionToBe);
                float angle =  Mathf.Atan2(localTurnVector.x, localTurnVector.z) * Mathf.Rad2Deg;
                if ((angle * turnMultiplier) > 1 || (angle * turnMultiplier) < -1)
                {
                    Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, 0, -angle) * turnMultiplier * Time.deltaTime);
                    SteeringWheel.transform.rotation *= deltaRotation;
                }
                else
                    SteeringWheel.transform.rotation *= (SteeringWheel.transform.rotation * Quaternion.Inverse(steerWheelOriginalRotation));
                // need work 
                //SteeringWheel.transform.localRotation = Quaternion.Lerp(SteeringWheel.transform.localRotation, Quaternion.Euler(0, 0, -angle * 50), Time.deltaTime);

            }
            
        }
    }
    public void FixedUpdate()
    {
        

        if (carProfile.nonPhysicalSimulation)
        {
            KAvoidanceSensor();
            //Interpolates the distance variance
            if (!Avoiding && !isRunningAnEvent && increamentUnit < pastIncreamentUnit)
            {
                increamentUnit = Mathf.Lerp(increamentUnit, increamentUnit + pastIncreamentUnit / 20, Time.deltaTime);
            }
            if (!Avoiding && !isRunningAnEvent && increamentUnit >= pastIncreamentUnit)
            {
                increamentUnit = Mathf.Lerp(increamentUnit, pastIncreamentUnit, Time.deltaTime);
            }

            ///////////////// Need work - related to Rock force//////////////////
            //if ( !this.CompareTag("Stopped By Traffic Light") && !isRunningAnEvent)
            //{

            //    if (!isAvoiding && increamentUnit != pastIncreamentUnit)
            //    {
            //        accelerated = true;
            //        stopped = false;
            //        increamentUnit = Mathf.Lerp(increamentUnit, increamentUnit + pastIncreamentUnit / 20, Time.deltaTime);
                    
            //        isEngineOn = true;
            //    }

            //}
            //if (increamentUnit == 0 && !stopped)
            //{
            //    stopped = true;
            //}
            if (!isAvoiding && !transform.CompareTag("Stopped By Traffic Light") && !isEngineOn && !isRunningAnEvent)
            {
                isEngineOn = true;
            }
            ////////////////////////////
            // dictates behaviour if engine is running
            if (isEngineOn)
            {
                
                KDrive();
                Vector3 direction = positionToBe - engine.position;
                Vector3 localDirction = transform.InverseTransformPoint(positionToBe);
                float angle =  Mathf.Atan2(localDirction.x, localDirction.z) * Mathf.Rad2Deg;
                Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, angle, 0) );

                //float distance = Vector3.Distance(positionToBe, engine.position);
                if (this.CompareTag("Event Object"))
                {
                    //engine.MovePosition(positionToBe);

                    transform.position = positionToBe;
                    transform.LookAt(directionToHave);
                }
                else
                {
                    
                    engine.MovePosition(engine.position + (direction * Time.deltaTime));
                    
                    engine.MoveRotation(engine.rotation * deltaRotation);
                }
                velocity = engine.velocity.magnitude;
                //spinns the wheel
                if (increamentUnit != 0 && isVisible)
                {
                    Vector3 wheelLocalDirction = transform.InverseTransformPoint(path.GetPoint(progress + increamentUnit));
                    float wheelAngle = Mathf.Atan2(wheelLocalDirction.x, wheelLocalDirction.z) * Mathf.Rad2Deg;
                    Quaternion wheelDeltaRotation = Quaternion.Euler(new Vector3(0, wheelAngle, 0) * Time.deltaTime);
                    wheelFR.transform.rotation *= wheelDeltaRotation;
                    wheelFL.transform.rotation *= wheelDeltaRotation;
                    caliperWheelFR.transform.rotation *= wheelDeltaRotation;
                    caliperWheelFL.transform.rotation *= wheelDeltaRotation;
                    wheelFL.transform.Rotate(Vector3.right, velocity);
                    wheelFR.transform.Rotate(Vector3.right, velocity);
                    wheelRL.transform.Rotate(Vector3.right, velocity);
                    wheelRR.transform.Rotate(Vector3.right, velocity); 
                    //wheelFL.transform.Rotate(Vector3.right, path.GetVelocity(progress).magnitude);
                    //wheelFR.transform.Rotate(Vector3.right, path.GetVelocity(progress).magnitude);
                    //wheelRL.transform.Rotate(Vector3.right, path.GetVelocity(progress).magnitude);
                    //wheelRR.transform.Rotate(Vector3.right, path.GetVelocity(progress).magnitude);
                }
            }
            
            
            


        }

    }
    // moves the car forward
    private void KDrive()
    {
        if (carProfile.goingForward)
        {

            progress += increamentUnit;
            if (progress > 1f)
            {
                if (!isLoop)
                {
                    progress = 1f;
                }
                else
                {
                    progress -= 1f;
                }
            }
        }
        else
        {
            progress -= increamentUnit;
            if (progress < 0f)
            {
                progress = -progress;
                carProfile.goingForward = true;
            }
        }

        Vector3 position = path.GetPoint(progress);
        positionToBe = position;
        
        if (carProfile.lookForward)
        {      
            directionToHave = position + path.GetDirection(progress);

        }
        if (CompareTag("ADV"))
        {
            WestdriveSettings.Progress = progress;
        }

    }
    //checks if an object is avoidable 
    // runs avoidance protocoll if yes
    void KAvoidanceSensor()
    {
        isAvoiding = false;
        RaycastHit hit;
        UnityEngine.LayerMask.NameToLayer("avoidable");
        if (engine.SweepTest(transform.forward,out hit,carProfile.avoidanceSenrosLength,QueryTriggerInteraction.Ignore) && (hit.collider.gameObject.layer == layerID))
        {
            if (carProfile.debugShowCastOnHit)
            {
                Debug.DrawLine(engine.position, hit.point, Color.red);
            }
            hittedObject = hit.collider.name;
            isAvoiding = true;
        }
        else
            isAvoiding = false;

        if ( isAvoiding)
        {
            KRunAvoidanceProtocol(hit);
        }
        
        
    }

    //Stops the car when another car is approaching
    void KRunAvoidanceProtocol(RaycastHit hit)
    {

        
            debugHitDistance = hit.distance;
            angleBetween = Vector3.SignedAngle(transform.forward, hit.transform.forward, Vector3.up);
            
            
            if (hit.distance > carProfile.avoidanceCriticalDistance)
            {
                debugHitDistance = hit.distance;
                increamentUnit = Mathf.Lerp(increamentUnit, increamentUnit / 5, Time.deltaTime);
                //if (!hit.collider.CompareTag("Blockade"))
                //    increamentUnit = hit.transform.gameObject.GetComponent<CarEngine>().increamentUnit;
                //else
                //    increamentUnit = Mathf.Lerp(increamentUnit , increamentUnit / 10,Time.deltaTime);
            }
            if (hit.distance <= carProfile.avoidanceCriticalDistance && 
                (angleBetween <= carProfile.turningAvoidanceTreshold && angleBetween >= (-1 * carProfile.turningAvoidanceTreshold)) || hit.collider.CompareTag("ADV") || this.CompareTag("ADV")) 
                    
            {

                debugHitDistance = hit.distance;
                increamentUnit = 0f;
                isEngineOn = false;
                
            }
            



    }
   // stops the car
    private void KHalt()
    {
        isEngineOn = false;
        increamentUnit = 0;
    }
    //saves the increament unit once per frame
    void KSaveMotorValues()
    {
        pastIncreamentUnit = increamentUnit;

    }
    //restarts the car and resets the increament
    public void KRestoreMotorValues()
    {
        isEngineOn = true;
        increamentUnit = pastIncreamentUnit;
    }
    /// <summary>
    /// Event System
    /// </summary>
    /// <param name="other"></param>

    private void OnTriggerEnter(Collider other)
    {

        //Event Gate initiate an event and ends it again
        if (true)
        {
            if (this.originalTag == "Event Object")
            {
                //ends the event
                if (other.CompareTag("End Event"))
                {
                    this.transform.parent.GetComponent<EventHandler>().needMotorControl = false;
                }
                // destroys event related objects
                if (other.CompareTag("Destroy Object"))
                {
                    if (WestdriveSettings.SimulationMode == mode.simulate)
                    {
                        WestdriveSettings.EventData.Data.Add(this.transform.parent.gameObject.name,dataPoints);
                    }
                    this.transform.parent.gameObject.SetActive(false);
                    Destroy(this.gameObject);
                }
            }
            //starts the event and stops the car
            if (other.GetComponent<EventHandler>() != null && this.originalTag == "ADV" && other.GetComponent<EventHandler>().ADVPath == this.path)
            {
                if (other.GetComponent<EventHandler>().needMotorControl )
                {
 
                    isRunningAnEvent = true;
                    increamentUnit = 0;
                    isEngineOn = false;
                }
            }
            // Trafic Light controls of the car
            if (other.GetComponent<TrafficLight>() != null && !this.CompareTag("Stopped By Traffic Light") && this.originalTag != "Event Object")
            {
                if (Vector3.Angle(transform.forward, other.transform.forward) < 10)
                {

                    float newSteer = Vector3.SignedAngle(transform.forward, path.GetPoint(progress + 0.1f) + path.GetDirection(progress + 0.1f), Vector3.up);
                    if ((!other.GetComponent<TrafficLight>().straighGreen || !other.GetComponent<TrafficLight>().straighYellow) && newSteer >= 0)
                    {

                        increamentUnit = 0;
                        isEngineOn = false;
                        this.tag = "Stopped By Traffic Light";
                    }
                    if ((!other.GetComponent<TrafficLight>().turnLeftGreen || !other.GetComponent<TrafficLight>().turnLeftYellow) && newSteer < 0)
                    {

                        increamentUnit = 0;
                        isEngineOn = false;
                        this.tag = "Stopped By Traffic Light";
                    }

                }


            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (true)
        {
            //checks if the motor should stop while event is running
            if (other.GetComponent<EventHandler>() != null && this.originalTag == "ADV" && other.GetComponent<EventHandler>().ADVPath == this.path)
            {
                if (other.GetComponent<EventHandler>().needMotorControl)
                {
                    
                    isRunningAnEvent = true;
                    increamentUnit = 0;
                    isEngineOn = false;
                }
                if (!other.GetComponent<EventHandler>().needMotorControl)
                {
                    isRunningAnEvent = false;
                    KRestoreMotorValues();
                    isEngineOn = true;
                }
            }
            //checks for change of trafficlights
            if (other.GetComponent<TrafficLight>() != null  && Vector3.Angle(transform.forward, other.transform.forward) < 10 && this.originalTag != "Event Object")
            {


                float newSteer = Vector3.SignedAngle(transform.forward, path.GetPoint(progress + 0.1f) + path.GetDirection(progress + 0.1f), Vector3.up);
                if ((!other.GetComponent<TrafficLight>().straighGreen) && (newSteer >= 0) && !this.CompareTag("Stopped By Traffic Light"))
                {
                    increamentUnit = 0;
                    isEngineOn = false;
                    this.tag = "Stopped By Traffic Light";
                }
                else if ((other.GetComponent<TrafficLight>().straighGreen) && (newSteer >= 0) && this.CompareTag("Stopped By Traffic Light"))
                {
                    isEngineOn = true;
                    KRestoreMotorValues();
                }
                if ((!other.GetComponent<TrafficLight>().turnLeftGreen) && (newSteer < 0) && !this.CompareTag("Stopped By Traffic Light"))
                {
                    increamentUnit = 0;
                    isEngineOn = false;
                    this.tag = "Stopped By Traffic Light";
                }
                else if ((other.GetComponent<TrafficLight>().turnLeftGreen) && (newSteer < 0) && this.CompareTag("Stopped By Traffic Light"))
                {
                    isEngineOn = true;
                    KRestoreMotorValues();
                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (true)
        {
            //Trafficlight sets the triggerlock to true if its an event related object
            if (other.GetComponent<TrafficLight>() != null && this.originalTag != "Event Object")
            {

                if (triggerLock)
                {

                    triggerLock = false;
                    this.tag = originalTag;

                }
                else
                {
                    triggerLock = true;

                }


            }
            // continuos on the defined path
            if (other.GetComponent<EventHandler>() != null && this.originalTag == "ADV" && other.GetComponent<EventHandler>().ADVPath == this.path)
            {

                isRunningAnEvent = false;
                KRestoreMotorValues();
                isEngineOn = true;

            }
        }
    }
    // regulates the engine music
    void EngineNoise()
    {
        if (increamentUnit != 0)
        {
            float tempPitch = increamentUnit / pastIncreamentUnit;
            if (tempPitch > engineNoisePitch)
                this.GetComponent<AudioSource>().pitch = tempPitch;
            else
                this.GetComponent<AudioSource>().pitch = engineNoisePitch;
        }
    }

    /// this section is the code related to recording positions
    private IEnumerator recorder()
    {
        while (true)
        {
            int frameCount = Time.frameCount;
            PositionRotationType dataPoint = new PositionRotationType();
            dataPoint.position = transform.position;
            dataPoint.rotaion = transform.rotation;
            if (!dataPoints.ContainsKey(frameCount))
                dataPoints.Add(Time.frameCount, dataPoint);
            else
                dataPoints[frameCount] = dataPoint;
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator EventRecorder()
    {
        while (true)
        {
            int frameCount = Time.frameCount - WestdriveSettings.frameCorrection;
            PositionRotationType dataPoint = new PositionRotationType();
            dataPoint.position = transform.position;
            dataPoint.rotaion = transform.rotation;
            if (!dataPoints.ContainsKey(frameCount))
                dataPoints.Add(frameCount, dataPoint);
            else
                dataPoints[frameCount] = dataPoint;
            yield return new WaitForEndOfFrame();
        }
    }
    public Dictionary<int, PositionRotationType> getData()
    {
        return dataPoints;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
        
    }
}
