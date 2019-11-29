using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Gives the Eventrelated Object a path and other parameters
/// </summary>
public class EventHandler : MonoBehaviour {

    // Use this for initialization
    public bool needMotorControl = true;
    [Header("Control Object")]
    public GameObject controlObject;
    public GameObject sourceObject;
    public BezierSplines eventPath;
    public GameObject [] blockades;
    public BezierSplines ADVPath;
    [Space]
    [Header("Vocal feedback")]
    public string AVASEnglishFeedbackText = "";
    public string AVASGermanFeedbackText = "";

    [Header("Vocal feedback non TTS audio")]
    public AudioClip AVASEnglishFeedbackAudio;
    public AudioClip AVASGermanFeedbackAudio;
    [Space]
    [Header("Event Delay")]
    public float delay = 0;
    [Space]
    [Header("Debug")]
    public string objectInfo = "";
    //checks if the eventrelated sourceObject is a car or pedestrian and applies respective parameters 
	void Start () {
        if (sourceObject == null)
        {
            objectInfo = "Simple Control";
        }
        else
        {
            //checks if the sourceObject is a car
            if (sourceObject.GetComponent<CarEngine>() != null)
            {
                if (eventPath != null)
                {
                    if (WestdriveSettings.SimulationMode == mode.record ||
                        WestdriveSettings.SimulationMode == mode.simulate)
                    {
                        controlObject = Instantiate<GameObject>(sourceObject);
                        controlObject.GetComponent<CarEngine>().isLoop = false;
                        controlObject.GetComponent<CarEngine>().path = eventPath;
                        controlObject.GetComponent<CarEngine>().startPecentage = 0;
                        controlObject.tag = "Event Object";
                        controlObject.transform.parent = this.transform;
                        controlObject.SetActive(false);
                    }
                    else if (WestdriveSettings.SimulationMode == mode.visualize)
                    {
                        controlObject = Instantiate<GameObject>(sourceObject);
                        controlObject.AddComponent<CarEngineVisualise>();
                        controlObject.GetComponent<CarEngineVisualise>().path = eventPath;
                        
                        controlObject.GetComponent<CarEngineVisualise>().Profile =
                            controlObject.GetComponent<CarEngine>().Profile;
                        Destroy(controlObject.GetComponent<CarEngine>());
                        controlObject.tag = "Event Object";
                        controlObject.transform.parent = this.transform;
                        controlObject.SetActive(false);
                    }
                }
                objectInfo = "Car control";
            }
            //checks if the sourceObject is a pedestrian
            if (sourceObject.GetComponent<Animator>() != null)
            {
                if (eventPath != null)
                {
                    if (WestdriveSettings.SimulationMode == mode.record ||
                        WestdriveSettings.SimulationMode == mode.simulate || (WestdriveSettings.SimulationMode == mode.visualize && WestdriveSettings.visualisationMode == visualisationMode.combinedHit))
                    {
                        controlObject = Instantiate<GameObject>(sourceObject);
                        controlObject.GetComponent<CharacterManager>().isLoop = false;
                        controlObject.GetComponent<CharacterManager>().usePathDefaultDuration = true;
                        controlObject.GetComponent<CharacterManager>().path = eventPath;
                        controlObject.GetComponent<CharacterManager>().startPecentage = 0;
                        controlObject.GetComponent<CharacterManager>().goingForward = true;
                        controlObject.GetComponent<CharacterManager>().lookForward = true;
                        controlObject.tag = "Event Object";
                        controlObject.transform.parent = this.transform;
                        controlObject.SetActive(false);
                    }
                    else if (WestdriveSettings.SimulationMode == mode.visualize && WestdriveSettings.visualisationMode != visualisationMode.combinedHit)
                    {
                        controlObject = Instantiate<GameObject>(sourceObject);
                        controlObject.AddComponent<CharacterManagerVisualise>();
                        controlObject.GetComponent<CharacterManagerVisualise>().path = eventPath;
                        Destroy(controlObject.GetComponent<CharacterManager>());
                        controlObject.tag = "Event Object";
                        controlObject.transform.parent = this.transform;
                        controlObject.SetActive(false);
                    }
                }
                objectInfo = "Pedestrian Control";
            }
        }
        
        if(blockades != null)
        {
            foreach(GameObject blockade in blockades)
            {
                blockade.SetActive(false);
            }
        }
    }

    
	//starts the Event after a delay
    public void RunEvent()
    {
        Invoke("Event", delay);
    }
    //starts the Event and initiates blockades
    private void Event()
    {
        Debug.Log("Running an Event!");
        if (blockades != null)
        {
            foreach (GameObject blockade in blockades)
            {
                blockade.SetActive(true);
            }
        }
        if(controlObject != null)
            controlObject.SetActive(true);
    }




}
