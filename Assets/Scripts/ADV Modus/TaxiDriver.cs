using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_STANDALONE_WIN
using CrazyMinnow.SALSA;
using CrazyMinnow.SALSA.Fuse;
#endif 
///<summary>
/// Simulates the lib-movement dependeing on the audio playback
///</summary>
public class TaxiDriver : MonoBehaviour {
    public string language;
    public AudioClip taxiDriverMonolougeEnglish;
    public AudioClip taxiDriverMonolougeGerman;
    public float startDelay = 1f;
    private AudioSource audio;
    private void Awake()
    {
        EventManager.StartListening("pauseTaxiAudio",PauseSpeaking);
        EventManager.StartListening("resumeTaxiAudio",ResumeSpeaking);
    }

    // Initializes the lib movement corresponding to the language selection
	IEnumerator Start () {
        Westdrive.Settings.SetConfigFile("Assets/Resources/Settings/Config.ini");
        startDelay = float.Parse(Westdrive.Settings.ReadValue("delays", "taxiDriverDelay"));
        EventManager.StartListening("stop audio",StopSpeaking);
        audio = gameObject.GetComponent<AudioSource>();
        if (language == "ENG")
        {


            if (audio != null && taxiDriverMonolougeEnglish != null)
            {
                audio.clip = taxiDriverMonolougeEnglish;
                audio.PlayDelayed(startDelay);
            }
        }
        if (language == "DE")
        {

            if (audio != null && taxiDriverMonolougeGerman != null)
            {
                audio.clip = taxiDriverMonolougeGerman;
                audio.PlayDelayed(startDelay);
            }
        }

        yield return null;
    }
    
    //Initalization of class parameters
    private void OnDestroy()
    {

        audio.Stop();

    }

    private void StopSpeaking()
    {
        audio.Stop();
    }

    public void PauseSpeaking()
    {
        this.gameObject.GetComponent<AudioSource>().Pause();
    }

    public void ResumeSpeaking()
    {
        this.gameObject.GetComponent<AudioSource>().UnPause();
    }
    private void OnEnable()
    {
        language = WestdriveSettings.language;
        taxiDriverMonolougeEnglish = WestdriveSettings.taxiDriverMonolougeEnglish;
        taxiDriverMonolougeGerman = WestdriveSettings.taxiDriverMonolougeGerman;
    }
}
