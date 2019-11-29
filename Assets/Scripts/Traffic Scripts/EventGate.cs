using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///Run the event using collision on each modes. 
/// </summary>
public class EventGate : MonoBehaviour {
    // Interface to set the scenario and insert descriptions. 
    [Space]
    [Header("Control")]
    public EventHandler scenario;
    
    [TextArea]
    public string scenarioDescription;
    [Space]
    [Header("AVAS Script")]
    [TextArea]
    public string AVASEnglishText = "";
    [TextArea]
    public string AVASGermanText = "";
    [Space] [Header("AVAS Script non TTS audio")]
    public AudioClip AVASEnglishAudio;

    public AudioClip AVASGermanAudio;
    [Space] [Header("Taxi Driver Audio Reactions")]
    public AudioClip GermanReactionAudio;

    public AudioClip EnglishReactionAudio;

    public float reactionDelay = 0;
    // Use this for initialization
    void Start () {
        scenario = this.transform.GetComponentInParent<EventHandler>();
        
	}

    // Run the corresponding event when any colliders enter the trigger.
    
    private void OnTriggerEnter(Collider other)
    {
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngine>().path == scenario.ADVPath)
            {
                scenario.RunEvent();
            }
        }
        else if (WestdriveSettings.SimulationMode == mode.simulate)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngineReplay>().path == scenario.ADVPath)
            {
                scenario.RunEvent();
            }
        }
        else if (WestdriveSettings.SimulationMode == mode.visualize)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngineVisualise>().path == scenario.ADVPath)
            {
                scenario.RunEvent();
            }
        }
    }
    void Update () {
		
	}
}
