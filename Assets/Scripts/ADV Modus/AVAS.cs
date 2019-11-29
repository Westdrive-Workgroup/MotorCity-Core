#if UNITY_STANDALONE_LINUX
#define NOTTS
#elif UNITY_STANDALONE_WIN
#define NOTTS
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechLib;
using System.Xml;
using System.IO;
using UnityEngine.Audio;

///<summary>
/// Translates text to speech by using the microsoft Text-to-Speech API
///</summary>
public class AVAS : MonoBehaviour {
    [Tooltip("please make sure that Microsoft's Hedda and Zira voices are installed on the machine")]
    public string language;
    private bool ended = false;
    public float feedbackDelay = 2;
    [Tooltip("Should be true if you want the feedback to be trigger on ADV halt. check false if you want the feedback to be triggered after event ended")]
    public bool feedbackOnHalt = false;
    private string feedbackText;
    private bool introDone = false;
#if NOTTS
    [Header("no TTS components")] 
    private AudioClip AVASAudio;
    public AudioMixerGroup AVASMixerOutput;
    private bool triggerLock;
#endif
    
#if TTS
    private SpVoice voice;
#endif
    // Selects the voice corresponding to the language setting
	void Awake ()
    {
        
        language = WestdriveSettings.language;
#if TTS
        SpObjectTokenCategory tokenCat = new SpObjectTokenCategory();
        tokenCat.SetId(SpeechLib.SpeechStringConstants.SpeechCategoryVoices, false);
        ISpeechObjectTokens tokens = tokenCat.EnumerateTokens(null, null);
        int n = 0;
        int languageIndex = 0;
        foreach (SpObjectToken item in tokens)
        {   if (language == "DE")
            {
                if (item.GetDescription(0).Contains("Hedda") )
                {
                    languageIndex = n;
                }
                
            }
            if (language == "ENG")
            {
                if (item.GetDescription(0).Contains("Zira"))
                {
                    languageIndex = n;
                }
                
            }
            
            n++;
        }
        
        voice = new SpVoice();
        voice.Voice = tokens.Item(languageIndex);
#endif
    }
	
	// Makes sure that the car is not running while the introduction is still taking place
	void Update () {
        #if TTS
        if(!introDone)
        {
            if (voice.Status.RunningState == SpeechRunState.SRSEDone)
            {
                
                //this.gameObject.GetComponent<CarEngine>().waitingForInitialization = false;
                introDone = true;
            }
        }
        #endif
    }
    //Play's pre defined Audioplayback in case of Event
    //First in Event of Entering a trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EventGate>() != null && this.CompareTag("ADV"))
        {
            #if TTS
            if (language == "DE")
            {
                voice.Speak(other.GetComponent<EventGate>().AVASGermanText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }
            else if (language == "ENG")
            {
                voice.Speak(other.GetComponent<EventGate>().AVASEnglishText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }
            #elif NOTTS
            if (language == "DE")
            {
                AVASAudio = other.GetComponent<EventGate>().AVASGermanAudio;
                StartCoroutine(PlayReaction(0));
            }
            else if (language == "ENG")
            {
                AVASAudio = other.GetComponent<EventGate>().AVASEnglishAudio;
                StartCoroutine(PlayReaction(0));
            }
            #endif
        }
        if (other.GetComponent<EventHandler>() != null && this.CompareTag("ADV") && feedbackOnHalt)
        {
            
            #if TTS
            if (language == "DE" && other.GetComponent<EventHandler>().AVASGermanFeedbackText != "")
            {
                feedbackText = other.GetComponent<EventHandler>().AVASGermanFeedbackText;
                Invoke("ReadFeedBackEnglish", feedbackDelay);
                
            }
            else if (language == "ENG" && other.GetComponent<EventHandler>().AVASEnglishFeedbackText != "")
            {
                feedbackText = other.GetComponent<EventHandler>().AVASEnglishFeedbackText;
                Invoke("ReadFeedBackEnglish", feedbackDelay);
               
            }
            #elif NOTTS
            if (language == "DE" && other.GetComponent<EventHandler>().AVASGermanFeedbackAudio != null)
            {
                AVASAudio = other.GetComponent<EventHandler>().AVASGermanFeedbackAudio;
                StartCoroutine(PlayReaction(feedbackDelay));
     
            }
            else if (language == "ENG" && other.GetComponent<EventHandler>().AVASEnglishFeedbackAudio != null)
            {
                AVASAudio = other.GetComponent<EventHandler>().AVASEnglishFeedbackAudio;
                StartCoroutine(PlayReaction(feedbackDelay));
               
            }
            #endif
        }
        


    }
    // Also for exiting a Trigger
    private void OnTriggerExit(Collider other)
    {
        
        #if TTS
        if (other.GetComponent<EventHandler>() != null && this.CompareTag("ADV") && !feedbackOnHalt)
        {
            if (language == "DE" && other.GetComponent<EventHandler>().AVASGermanFeedbackText != "")
            {
                feedbackText = other.GetComponent<EventHandler>().AVASGermanFeedbackText;
                Invoke("ReadFeedBackEnglish", feedbackDelay);
             
            }
            else if (language == "ENG" && other.GetComponent<EventHandler>().AVASEnglishFeedbackText != "")
            {
                feedbackText = other.GetComponent<EventHandler>().AVASEnglishFeedbackText;
                Invoke("ReadFeedBackEnglish", feedbackDelay);
          
            }
        }
        #elif NOTTS
        if (other.GetComponent<EventHandler>() != null && this.CompareTag("ADV") && !feedbackOnHalt)
        {
            if (language == "DE" && other.GetComponent<EventHandler>().AVASGermanFeedbackAudio != null)
            {
                AVASAudio = other.GetComponent<EventHandler>().AVASGermanFeedbackAudio;
                StartCoroutine(PlayReaction(feedbackDelay));
     
            }
            else if (language == "ENG" && other.GetComponent<EventHandler>().AVASEnglishFeedbackAudio != null)
            {
                AVASAudio = other.GetComponent<EventHandler>().AVASEnglishFeedbackAudio;
                StartCoroutine(PlayReaction(feedbackDelay));
               
            }
        }
        #endif
    }
    #if TTS
    public bool IntroduntionDone
    {
        
        get
        {
            return introDone;
        }
    }
    public void ReadFeedBackEnglish()
    {
        Debug.Log("English feedback Invoked");
        voice.Speak(feedbackText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }
    public void ReadFeedBackGerman()
    {
        Debug.Log("German feedback Invoked");
        voice.Speak(feedbackText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }
    public void ReadText(string message)
    {
        Debug.Log("TTS Invoked");
        voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }
    string loadXMLStandalone(string fileName)
    {

        string path = Path.Combine("Resources", fileName);
        path = Path.Combine(Application.dataPath, path);
        Debug.Log("Path:  " + path);
        StreamReader streamReader = new StreamReader(path);
        string streamString = streamReader.ReadToEnd();
        Debug.Log("STREAM XML STRING: " + streamString);
        return streamString;
    }
    #elif NOTTS
    private IEnumerator PlayReaction(float audioDelay)
    {
        triggerLock = true;
        
        AudioSource AVASAudioSource = this.gameObject.AddComponent<AudioSource>() as AudioSource;
        AVASAudioSource.playOnAwake = false;
        AVASAudioSource.clip = AVASAudio;
        AVASAudioSource.outputAudioMixerGroup = AVASMixerOutput;
        yield return new WaitForSeconds(audioDelay);
        AVASAudioSource.Play();
        while (AVASAudioSource.isPlaying)
        {
            yield return null;
        }
        Destroy(AVASAudioSource);
        triggerLock = false;
        yield return null;
    }
    public void PlayMessege(AudioClip audioMessege)
    {
        Debug.Log("audio Invoked");
        AVASAudio = audioMessege;
        StartCoroutine(PlayReaction(0));
    }
    #endif
    //Loads the path from an XML file and returns it
    
}
