#if UNITY_STANDALONE_LINUX
#define NOTTS
#elif UNITY_STANDALONE_WIN
#define NOTTS
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AVASEvent : MonoBehaviour
{
    [Space] [Header("Parameters")]
    public float delay = 0f;

    public BezierSplines relatedPath;
    [Space]
    [Header("AVAS Script")]
    [TextArea]
    public string AVASEnglishText = "";
    [TextArea]
    public string AVASGermanText = "";

    [Space] [Header("AVAS Script non TTS audio")]
    public AudioClip AVASEnglishAudio;
    public AudioClip AVASGermanAudio;
    
    #if TTS
    private void OnTriggerEnter(Collider other)
    {
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngine>().path == relatedPath &&
                other.transform.gameObject.GetComponent<AVAS>() != null)
            {
                if (WestdriveSettings.language == "ENG")
                    other.transform.gameObject.GetComponent<AVAS>().ReadText(AVASEnglishText);
                if (WestdriveSettings.language == "DE")
                    other.transform.gameObject.GetComponent<AVAS>().ReadText(AVASGermanText);
            }
        }
        else if (WestdriveSettings.SimulationMode == mode.simulate)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngineReplay>().path == relatedPath &&
                other.transform.gameObject.GetComponent<AVAS>() != null)
            {
                if (WestdriveSettings.language == "ENG")
                    other.transform.gameObject.GetComponent<AVAS>().ReadText(AVASEnglishText);
                if (WestdriveSettings.language == "DE")
                    other.transform.gameObject.GetComponent<AVAS>().ReadText(AVASGermanText);
            }
        }
    }
    #elif NOTTS
    private void OnTriggerEnter(Collider other)
    {
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngine>().path == relatedPath &&
                other.transform.gameObject.GetComponent<AVAS>() != null)
            {
                if (WestdriveSettings.language == "ENG")
                    other.transform.gameObject.GetComponent<AVAS>().PlayMessege(AVASEnglishAudio);
                if (WestdriveSettings.language == "DE")
                    other.transform.gameObject.GetComponent<AVAS>().PlayMessege(AVASGermanAudio);
            }
        }
        else if (WestdriveSettings.SimulationMode == mode.simulate)
        {
            if (other.transform.CompareTag("ADV") &&
                other.transform.gameObject.GetComponent<CarEngineReplay>().path == relatedPath &&
                other.transform.gameObject.GetComponent<AVAS>() != null)
            {
                if (WestdriveSettings.language == "ENG")
                    other.transform.gameObject.GetComponent<AVAS>().PlayMessege(AVASEnglishAudio);
                if (WestdriveSettings.language == "DE")
                    other.transform.gameObject.GetComponent<AVAS>().PlayMessege(AVASGermanAudio);
            }
        }
    }
    #endif
}
