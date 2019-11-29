using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechLib;
using System.Xml;
using System.IO;
///<summary>
///Controlls the Vehicle Radio Modul
///</summary>
public class RadioTalk : MonoBehaviour
{

    public string language;
    public AudioClip radioTalkEnglish;
    public AudioClip radioTalkGerman;
    void Start()
    {
        //Sets the Audiosource corresponding to the programm language
        EventManager.StartListening("stop audio",StopRadio);
        if(language == "ENG")
            gameObject.GetComponent<AudioSource>().clip = radioTalkEnglish;
        if(language == "DE")
            gameObject.GetComponent<AudioSource>().clip = radioTalkGerman;
        gameObject.GetComponent<AudioSource>().Play();

    }
    //Initalization of class parameters
    private void OnEnable()
    {
        language = WestdriveSettings.language;
        radioTalkEnglish = WestdriveSettings.radioTalkEnglish;
        radioTalkGerman = WestdriveSettings.radioTalkGerman;
    }
    //Stops the Audio playback
    private void OnDisable()
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }

    private void StopRadio()
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }
    private void OnDestroy()
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }
}