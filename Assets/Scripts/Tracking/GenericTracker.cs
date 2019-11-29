using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// Tracks the Car position and the general raycast
/// </summary>
public abstract class GenericTracker : MonoBehaviour
{
    public virtual TrackingData recordedData { get; set; }
    public string currentContext;
    protected bool isTracking;
    protected bool isRayCasting;
    protected int firstFrame;
    protected int lastFrame;
    protected int currentFrame;
    //Still empty methods
    public GenericTracker()
    {

    }
    public virtual void Calibrate()
    {

    }
    public virtual void Validate()
    {

    }
    protected void Start()
    {
        
    }
    //sets tracking and raycasting to false on start
    protected void Awake()
    {
        this.isTracking = false;
        this.isRayCasting = false;
    }
    //Activates the tracking
    public virtual void TrackAsync()
    {
        this.isTracking = true;
    }
    // Stops the tracking
    public virtual void StopTracking()
    {
        this.isTracking = false;
    }
    //Starts the Ray casting
    public virtual void startRayCasting()
    {
        this.isRayCasting = true;
    }
    //Stops the Raycasting
    public virtual void stopRayCasting()
    {
        this.isRayCasting = false;
    }
    //Deletes the recorded data
    public void clearData()
    {
        recordedData.Data.Clear();
    }
    //replays the drive from the data points
    public void SetData(TrackingData data)
    {
        recordedData = data;
        if (WestdriveSettings.SimulationMode == mode.visualize)
        {
            firstFrame = recordedData.Data.Keys.First();
            lastFrame = recordedData.Data.Keys.Last();
            currentFrame = firstFrame;
        }
    }
    public void OnDestroy()
    {
        
    }
   
}
