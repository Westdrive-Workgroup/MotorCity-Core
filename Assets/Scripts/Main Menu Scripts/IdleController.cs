using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
using UnityEngine.Video;
using UnityEngine.XR;

/// <summary>
/// Controlls the announcement and Scene fading
/// </summary>
public class IdleController : MonoBehaviour
{
    [Header("Components")] public CanvasGroup announcement;
    public CanvasGroup screen;
    public VideoClip instructionClip;
    public Texture logo;
    public RawImage instrcution;
    public GameObject mainController;
    public GameObject monoCamera;
    public GameObject stereoCamera;
    private GameObject camera;

    [Space] [Header("parameters")] [Range(0, 1)]
    public float fadeSpeed = 0.05f;

    public float frameWaitSpeed = 0.01f;
//    [Space] [Header("internal")] private bool userPresent;

    public float presenceThreshold = 5f;
    private bool userCheckLock;


    //Sets the alpha values= 0 , that are regulating the fading times and starts the fade in Annoucement
    void Awake()
    {
//        userPresent = false;
        userCheckLock = false;
        if (!Directory.Exists("Assets/Resources/Settings"))
        {
            Debug.Log("settings folder doesn't exist. creating one.");
            Directory.CreateDirectory("Assets/Resources/Settings");
        }


        if (!File.Exists("Assets/Resources/Settings/Config.ini"))
        {
            Debug.Log("config file doesn't exist. downloading latest one from source");
            TextAsset configTemplate = (TextAsset) Resources.Load("ConfigTemplates/Config-Template", typeof(TextAsset));
            TextWriter configWriter = new StreamWriter("Assets/Resources/Settings/Config.ini");
            configWriter.Write(configTemplate.text);
            configWriter.Close();
        }

        if (!File.Exists("Assets/Resources/Settings/Cars.manifest"))
        {
            Debug.Log("car manifest file doesn't exist. downloading latest one from source");
            TextAsset manifestTemplate =
                (TextAsset) Resources.Load("ConfigTemplates/Cars-Manifest-Template", typeof(TextAsset));
            TextWriter configWriter = new StreamWriter("Assets/Resources/Settings/Cars.manifest");
            configWriter.Write(manifestTemplate.text);
            configWriter.Close();
        }

        if (!File.Exists("Assets/Resources/Settings/firstRun"))
        {
            WestdriveSettings.firstRun = true;
            TextWriter configWriter = new StreamWriter("Assets/Resources/Settings/firstRun");
            configWriter.Write("DO NOT DELETE THIS");
            configWriter.Close();
        }
        else
        {
            WestdriveSettings.firstRun = false;
        }

        Westdrive.Settings.SetConfigFile("Assets/Resources/Settings/Config.ini");
        switch (Westdrive.Settings.ReadValue("engine", "mode"))
        {
            case "record":
                WestdriveSettings.SimulationMode = mode.record;
                break;
            case "simulate":
                WestdriveSettings.SimulationMode = mode.simulate;
                break;
            case "visualise":
                WestdriveSettings.SimulationMode = mode.visualize;
                break;
            default:
                WestdriveSettings.SimulationMode = mode.simulate;
                break;
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("input", "simulationData")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("input", "simulationData") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("input", "simulationData"));
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("output", "subPath") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath"));
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("output", "events")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("output", "events") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("output", "events"));
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("output", "csv")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("output", "csv") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("output", "csv"));
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("output", "hit")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("output", "hit") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("output", "hit"));
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("output", "combined")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("output", "combined") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("output", "combined"));
        }

        if (!Directory.Exists(Application.dataPath + Westdrive.Settings.ReadValue("output", "screenshots")))
        {
            Debug.Log(Westdrive.Settings.ReadValue("output", "screenshots") + " doesn't exist. creating one.");
            Directory.CreateDirectory(Application.dataPath + Westdrive.Settings.ReadValue("output", "screenshots"));
        }

        if (WestdriveSettings.SimulationMode != mode.simulate || WestdriveSettings.firstRun)
        {
            stereoCamera.SetActive(false);
            camera = monoCamera;
            camera.SetActive(true);
        }
        else
        {
#if UNITY_EDITOR
            stereoCamera.SetActive(false);
            camera = stereoCamera;
            camera.SetActive(true);
#else
            monoCamera.SetActive(false);
            camera = stereoCamera;
            camera.SetActive(true);
#endif
        }

        if (WestdriveSettings.SimulationMode != mode.simulate || WestdriveSettings.firstRun)
        {
            Debug.Log("Loading Scene");
            if (WestdriveSettings.firstRun)
                Debug.Log("first run");
            SceneManager.LoadScene("Stadt");
        }
        else
        {
            screen.alpha = 0;
            screen.gameObject.SetActive(false);
            announcement.alpha = 0;
            StartCoroutine(fadeInAnnouncement());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!mainController.activeSelf)
            {
                Debug.Log("preparing load");
                StartCoroutine(fadeOutAnnouncement());
            }
        }
        
 
    }


//    private IEnumerator checkUser()
//    {
//        if (userPresent)
//        {
//            yield return new WaitForSeconds(presenceThreshold);
//            
//            
//            
//        }
//        if (userPresent)
//        {
//            if (!mainController.activeSelf)
//            {
//                Debug.Log("preparing load");
//                StartCoroutine(fadeOutAnnouncement());
//            }
//        }
//        
//
//        if (!userPresent)
//        {
//            
//            if (!mainController.GetComponent<ExperimentLoader>().isLoading)
//            {
//
//                StopCoroutine(fadeInScreen());
//                StartCoroutine(fadeOutScreen());
//            }
//            else
//            {
//                Debug.Log("Loading already started, cannot halt procedure");
//            }
//        }
//    }


    //checks if the User is present 
    public void UserPresent()
    {
        
//            userPresent = true;

    }

    //if user not present and End is not reached activates the userCheckLock, If the end is reached starts the EndProcess
    public void UserNotPresent()
    {
        
//            userPresent = false;
    }

    //fades in the announcement corresponding to the alpha value
    IEnumerator fadeInAnnouncement()
    {
        announcement.gameObject.SetActive(true);
        float fadeAlpha = 0f;
        while (fadeAlpha <= 1f)
        {
            announcement.alpha = fadeAlpha;
            fadeAlpha += fadeSpeed;
            yield return new WaitForSeconds(frameWaitSpeed);
        }

        announcement.alpha = 1f;
        while (!checkIfUserPresent())
        {
            yield return null;
        }
        StartCoroutine(fadeOutAnnouncement());
        
        /*yield return WaitForSeconds(presenceThreshold);
        if (checkIfUserPresent())
        {
            
        }*/
       
        
        yield return null;
    }

    //fades out the announcemnt and starts the fade in Screen coroutine
    IEnumerator fadeOutAnnouncement()
    {
        float fadeAlpha = 1f;
        while (fadeAlpha >= 0f)
        {
            announcement.alpha = fadeAlpha;
            fadeAlpha -= fadeSpeed;
            yield return new WaitForSeconds(frameWaitSpeed);
        }

        announcement.alpha = 0f;
        announcement.gameObject.SetActive(false);
        screen.gameObject.SetActive(false);
        StartCoroutine(fadeInScreen());
        yield return null;
    }

    //Fades in the Screen
    IEnumerator fadeInScreen()
    {
        this.GetComponent<VideoPlayer>().Prepare();
        while (this.GetComponent<VideoPlayer>().isPrepared)
        {
            yield return null;
        }
        screen.gameObject.SetActive(true);
        float fadeAlpha = 0f;
        while (fadeAlpha <= 1f)
        {
            screen.alpha = fadeAlpha;
            fadeAlpha += fadeSpeed;
            yield return new WaitForSeconds(frameWaitSpeed);
        }

        screen.alpha = 1f;
        instrcution.texture = this.GetComponent<VideoPlayer>().texture;
        
        this.GetComponent<VideoPlayer>().Play();
        while (this.GetComponent<VideoPlayer>().isPlaying)
        {
            yield return null;
        }

        if (checkIfUserPresent())
        {
            instrcution.texture = logo;
            yield return new WaitForSeconds(2);
            mainController.SetActive(true);
            yield return null;
        }
        else
        {
            StartCoroutine(fadeOutScreen());
        }
        /*if (!checkIfUserPresent())
        {
            
        }
        
        if (!userPresent)
        {
            StopAllCoroutines();
            StartCoroutine(fadeInAnnouncement());
        }*/
        
    }


    private bool checkIfUserPresent()
    {
        
        return SteamVR_Actions._default.HeadsetOnHead[SteamVR_Input_Sources.Any].state;
        
    }

    
    //fades out the Screen and starts the fade in Announcement coroutine
    IEnumerator fadeOutScreen()
    {
        float fadeAlpha = 1f;
        this.GetComponent<VideoPlayer>().Stop();
        while (fadeAlpha >= 0f)
        {
            screen.alpha = fadeAlpha;
            fadeAlpha -= fadeSpeed;
            yield return new WaitForSeconds(frameWaitSpeed);
        }

        screen.alpha = 0f;
        mainController.SetActive(false);
        screen.gameObject.SetActive(false);
        StartCoroutine(fadeInAnnouncement());
        yield return null;
    }
}