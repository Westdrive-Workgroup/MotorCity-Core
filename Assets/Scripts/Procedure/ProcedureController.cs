#if UNITY_STANDALONE_LINUX
#define NOTTS
#elif UNITY_STANDALONE_WIN
#define NOTTS
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.Audio;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.XR;
using Valve.VR;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
/// <summary>
/// Sets the languages for the texts and controlls the next Block Modul
/// </summary>
public class ProcedureController : MonoBehaviour
{
    [Space, Header("Cameras")] public GameObject monoCamera;
    public GameObject stereoCamera;
    [Space, Header("Components")] public GameObject camera;
    public ProcedureProfile experiment;
    public CarsManager carManager;
    public PedestrianManager pedestrianManager;
    public GameObject annoucementArea;
    
    [Space, Header("Record specific components")]
    public List<GameObject> disbaleOnRecord;

    [Space, Header("Internal Components")] private List<string> ADVPaths;
    private List<Object> ADVModules;
    public AudioMixerGroup reactionsOutput;
    public AudioMixerGroup AVASOutput;
    private Payload experimentData;
    private bool readyToEnd;
    private string request;
    [Space, Header("Announcement Area")] public Transform cameraStand;
    public GameObject announcementBoard;


    [FormerlySerializedAs("presenceThreshold"), Space, Header("User Presence Control"),
     Tooltip("Time in seconds which considered as user abandoning the experiment")]
    public float presenceThreshold = 5;

    [Tooltip("Time intervals in seconds to check for user presences")]
    public float controlInterval = 1;

    private bool isEndTriggered = false;
    private bool blockEndTriggered = false;
//    private bool userPresent = false;
    private bool userCheckLock = false;
    private bool useFallBack = false;
    [Space, Header("Internal parameters")] private int nextIndex;
    private bool setupDone = true;
    private int downloadProgress = 0;
    private bool firstBlock = false;
    public float startPercentage = 0f;
    private string mainServer;
    private string fallBackServer;
    private string queryString;
    private bool serverError = false;
    public float loadingDelay = 3f;
    [Space, Header("Announcement Texts")] private string consentText;
    private string welcomeText;
    private string dataSavingText;
    private string dataLoadingText;
    private string endBlockText;
    private string serverErrorText;
    private string endExperimentText;
    private string endExperimentWithCodeText;
    private string experimentLoadingText;
    private string experimentLoadingPercentageText;
    private string dataDownloadingText;
    private string transferText;
    private float timeElapsed = 0;
    //sets the language for the Texts
    private void InitiateTexts()
    {
        if (WestdriveSettings.language == "DE")
        {
            consentText = Westdrive.Settings.ReadValue("germanTexts", "consentText");
            welcomeText = Westdrive.Settings.ReadValue("germanTexts", "welcomeText");
            dataSavingText = Westdrive.Settings.ReadValue("germanTexts", "dataSavingText");
            dataLoadingText = Westdrive.Settings.ReadValue("germanTexts", "dataLoadingText");
            endBlockText = Westdrive.Settings.ReadValue("germanTexts", "endBlockText");
            serverErrorText = Westdrive.Settings.ReadValue("germanTexts", "serverErrorText");
            endExperimentText = Westdrive.Settings.ReadValue("germanTexts", "endExperimentText");
            endExperimentWithCodeText = Westdrive.Settings.ReadValue("germanTexts", "endExperimentWithCodeText");
            experimentLoadingText = Westdrive.Settings.ReadValue("germanTexts", "experimentLoadingText");
            experimentLoadingPercentageText =
                Westdrive.Settings.ReadValue("germanTexts", "experimentLoadingPercentageText");
            transferText = Westdrive.Settings.ReadValue("germanTexts", "transferText");
            dataDownloadingText = Westdrive.Settings.ReadValue("germanTexts", "dataDownloadingText");
        }
        else if (WestdriveSettings.language == "ENG")
        {
            consentText = Westdrive.Settings.ReadValue("englishTexts", "consentText");
            welcomeText = Westdrive.Settings.ReadValue("englishTexts", "welcomeText");
            dataSavingText = Westdrive.Settings.ReadValue("englishTexts", "dataSavingText");
            dataLoadingText = Westdrive.Settings.ReadValue("englishTexts", "dataLoadingText");
            endBlockText = Westdrive.Settings.ReadValue("englishTexts", "endBlockText");
            serverErrorText = Westdrive.Settings.ReadValue("englishTexts", "serverErrorText");
            endExperimentText = Westdrive.Settings.ReadValue("englishTexts", "endExperimentText");
            endExperimentWithCodeText = Westdrive.Settings.ReadValue("englishTexts", "endExperimentWithCodeText");
            experimentLoadingText = Westdrive.Settings.ReadValue("englishTexts", "experimentLoadingText");
            experimentLoadingPercentageText =
                Westdrive.Settings.ReadValue("englishTexts", "experimentLoadingPercentageText");
            transferText = Westdrive.Settings.ReadValue("englishTexts", "transferText");
            dataDownloadingText = Westdrive.Settings.ReadValue("englishTexts", "dataDownloadingText");
        }
    }

    private void Exit()
    {
        StopAllCoroutines();
        if (WestdriveSettings.experimentReload == reloadBehaviour.close)
        {
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else if (WestdriveSettings.experimentReload == reloadBehaviour.goToLobby)
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
    private void Awake()
    {
        Debug.Log("awaken");
        WestdriveSettings.ResetUID();
        WestdriveSettings.ResetParticipantCode();
        Westdrive.Settings.SetConfigFile("Assets/Resources/Settings/Config.ini");
        if (Westdrive.Settings.ReadValue("batchMode", "endBehaviour") == "goToLobby")
        {
            WestdriveSettings.experimentReload = reloadBehaviour.goToLobby;
        }
        else if (Westdrive.Settings.ReadValue("batchMode", "endBehaviour") == "close")
        {
            WestdriveSettings.experimentReload = reloadBehaviour.close;
        }
        else
        {
            WestdriveSettings.experimentReload = reloadBehaviour.goToLobby;
        }
        if (WestdriveSettings.firstRun)
        {
/*#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif*/
            Exit();
        }
        EventManager.StartListening("startTimeOut",PresenceTimeOut);
        experimentData = new Payload();

        WestdriveSettings.frameCorrection = 0;
        WestdriveSettings.isPlaying = false;
        serverError = false;
        
        mainServer = Westdrive.Settings.ReadValue("server", "main");
        fallBackServer = Westdrive.Settings.ReadValue("server", "fallback");
        useFallBack = Westdrive.Settings.ReadValue("server", "useFallBack") == "true" ? true : false;
        queryString = Westdrive.Settings.ReadValue("server", "queryString");
        
        //checks with mode is enabled

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

        WestdriveSettings.language = Westdrive.Settings.ReadValue("language", "code");
        InitiateTexts();
        //checks if the values should be interpolated or not
        if (Westdrive.Settings.ReadValue("smoothing", "interpolate") == "true")
        {
            WestdriveSettings.useInterpolate = true;
        }
        else
        {
            WestdriveSettings.useInterpolate = false;
        }

        annoucementArea.SetActive(true);
        //only activates in the unity editor
        if (WestdriveSettings.SimulationMode != mode.simulate)
        {
            stereoCamera.SetActive(false);
            camera = monoCamera;
            camera.SetActive(true);
        }
        else
        {
#if UNITY_EDITOR
            stereoCamera.SetActive(false);
            camera = monoCamera;
            camera.SetActive(true);
            
//            monoCamera.SetActive(false);
//            camera = stereoCamera;
//            camera.SetActive(true);
#else
            monoCamera.SetActive(false);
            camera = stereoCamera;
            camera.SetActive(true);
            //Valve.VR.OpenVR.System.ResetSeatedZeroPose();
            //Valve.VR.OpenVR.Compositor.SetTrackingSpace(Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated);
#endif
        }

        //monoCamera.SetActive(false);
        //camera = stereoCamera;
        //camera.SetActive(true
        isEndTriggered = false;
        blockEndTriggered = false;
//        userPresent = false;
        userCheckLock = false;
        ADVPaths = new List<string>(experiment.ADVPaths);
        ADVModules = new List<Object>(experiment.ADVModules);
        WestdriveSettings.Progress = 0;
        nextIndex = 0;
        WestdriveSettings.language = experiment.language;
        WestdriveSettings.radioTalkEnglish = experiment.radioTalkEnglish;
        WestdriveSettings.radioTalkGerman = experiment.radioTalkGerman;
        WestdriveSettings.taxiDriverMonolougeEnglish = experiment.taxiDriverMonolougeEnglish;
        WestdriveSettings.taxiDriverMonolougeGerman = experiment.taxiDriverMonolougeGerman;
        if (carManager != null)
            carManager.gameObject.SetActive(true);
        else
            Debug.LogWarning("No car manager in the scene");
        if (pedestrianManager != null)
            pedestrianManager.gameObject.SetActive(true);
        else
            Debug.LogWarning("No pedestrian manager in the scene");
        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = welcomeText;
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            foreach (GameObject target in disbaleOnRecord)
            {
                target.SetActive(false);
            }
        }

        camera.transform.position = cameraStand.position;
        camera.transform.rotation = cameraStand.rotation;
        camera.transform.parent = cameraStand;
    }

    //Enables the first block upon start and initiates the chooseNextBlock method
    private void Start()
    {
        firstBlock = true;
        if (WestdriveSettings.SimulationMode != mode.visualize)
        {
            chooseNextBlock();
        }
        else
        {
            StartCoroutine(VisualisationSetup());
        }
    }


    private IEnumerator VisualisationSetup()
    {
        GameObject onScreenCameraStand = new GameObject("onScreenCameraStand");
        onScreenCameraStand.transform.position = camera.transform.position;
        onScreenCameraStand.transform.rotation = camera.transform.rotation;
        onScreenCameraStand.transform.parent = this.transform;
        camera.transform.parent = onScreenCameraStand.transform;
        annoucementArea.SetActive(false);

        camera.AddComponent<VisualisationGUI>();

        camera.GetComponent<VisualisationGUI>().setManagers(carManager, pedestrianManager);
        camera.GetComponent<VisualisationGUI>().setProfile(experiment);
        Destroy(camera.GetComponent<FlyCam>());
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (camera.GetComponent<FlyCam>() != null)
                    Destroy(camera.GetComponent<FlyCam>());
                else
                    camera.AddComponent<FlyCam>();
            }

            yield return null;
        }

        yield return null;
    }

    //if the user is not present, wait for the elapsed time to cross the presenceThreshold to save the data and stop the application
    private IEnumerator SaveAndExit()
    {
        Debug.Log("saving and exit");
        if (WestdriveSettings.SimulationMode == global::mode.simulate)
        {
            //Tracker.saveToFileAsync(Application.dataPath + Westdrive.Settings.ReadValue("[output]", "subPath") + WestdriveSettings.ParticipantUID);

            if (experiment.usetracking)
            {
                string context = "";
                if (camera.transform.childCount != 0)
                {
                    string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                      camera.GetComponentInChildren<GenericTracker>().currentContext;
                    context = camera.GetComponentInChildren<GenericTracker>().currentContext;
                    Westdrive.IO.SaveToFileAsync(dataPath,
                        camera.GetComponentInChildren<GenericTracker>().recordedData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "trackingData"));
                }
                else
                {
                    string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                      camera.GetComponent<GenericTracker>().currentContext;
                    context = camera.GetComponent<GenericTracker>().currentContext;
                    Westdrive.IO.SaveToFileAsync(dataPath, camera.GetComponent<GenericTracker>().recordedData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "trackingData"));
                }

                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                    yield return null;
                }

                string eventDataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "events") +
                                       context;
                Westdrive.IO.SaveToFileAsync(eventDataPath, WestdriveSettings.EventData,
                    WestdriveSettings.ParticipantUID.ToString(),
                    Westdrive.Settings.ReadValue("extensions", "binary"));
                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                    yield return null;
                }
            }
        }

        Exit();
        yield return null;
    }
    /// <summary>
    /// old version
    /// </summary>
    /// <returns></returns>
    /*private IEnumerator checkUser()
    {
        userCheckLock = true;
        yield return new WaitForSeconds(presenceThreshold);
        if (checkIfUserPresent())
        {
            userCheckLock = false;
            StopCoroutine(checkUser());
            yield return null;
        }
        else
        {
            if (WestdriveSettings.SimulationMode == global::mode.simulate)
            {
                //Tracker.saveToFileAsync(Application.dataPath + Westdrive.Settings.ReadValue("[output]", "subPath") + WestdriveSettings.ParticipantUID);

                if (experiment.usetracking)
                {
                    string context = "";
                    if (camera.transform.childCount != 0)
                    {
                        string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                          camera.GetComponentInChildren<GenericTracker>().currentContext;
                        context = camera.GetComponentInChildren<GenericTracker>().currentContext;
                        Westdrive.IO.SaveToFileAsync(dataPath,
                            camera.GetComponentInChildren<GenericTracker>().recordedData,
                            WestdriveSettings.ParticipantUID.ToString(),
                            Westdrive.Settings.ReadValue("extensions", "trackingData"));
                    }
                    else
                    {
                        string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                          camera.GetComponent<GenericTracker>().currentContext;
                        context = camera.GetComponent<GenericTracker>().currentContext;
                        Westdrive.IO.SaveToFileAsync(dataPath, camera.GetComponent<GenericTracker>().recordedData,
                            WestdriveSettings.ParticipantUID.ToString(),
                            Westdrive.Settings.ReadValue("extensions", "trackingData"));
                    }

                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                        yield return null;
                    }

                    string eventDataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "events") +
                                           context;
                    Westdrive.IO.SaveToFileAsync(eventDataPath, WestdriveSettings.EventData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "binary"));
                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                        yield return null;
                    }
                }
            }

            Exit();
            yield return null;
        }

        /*float timeElapsed = 0;
    
        if (checkIfUserPresent())
        {
            userCheckLock = true;
            
            while (timeElapsed < presenceThreshold)
            {

                if (!checkIfUserPresent())
                {
                    timeElapsed += controlInterval;
                    yield return new WaitForSeconds(controlInterval);
                }
                else
                {
                    userCheckLock = false;
                    StopCoroutine(checkUser());
                    yield return null;
                }
            }

        }
//        if (!userPresent)
        else if (!checkIfUserPresent())
        {
            if (WestdriveSettings.SimulationMode == global::mode.simulate)
            {
                //Tracker.saveToFileAsync(Application.dataPath + Westdrive.Settings.ReadValue("[output]", "subPath") + WestdriveSettings.ParticipantUID);

                if (experiment.usetracking)
                {
                    string context = "";
                    if (camera.transform.childCount != 0)
                    {
                        string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                          camera.GetComponentInChildren<GenericTracker>().currentContext;
                        context = camera.GetComponentInChildren<GenericTracker>().currentContext;
                        Westdrive.IO.SaveToFileAsync(dataPath,
                            camera.GetComponentInChildren<GenericTracker>().recordedData,
                            WestdriveSettings.ParticipantUID.ToString(),
                            Westdrive.Settings.ReadValue("extensions", "trackingData"));
                    }
                    else
                    {
                        string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                          camera.GetComponent<GenericTracker>().currentContext;
                        context = camera.GetComponent<GenericTracker>().currentContext;
                        Westdrive.IO.SaveToFileAsync(dataPath, camera.GetComponent<GenericTracker>().recordedData,
                            WestdriveSettings.ParticipantUID.ToString(),
                            Westdrive.Settings.ReadValue("extensions", "trackingData"));
                    }

                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                        yield return null;
                    }

                    string eventDataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "events") +
                                           context;
                    Westdrive.IO.SaveToFileAsync(eventDataPath, WestdriveSettings.EventData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "binary"));
                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                        yield return null;
                    }
                }
            }
            /#2#/only activates in the Unity Editor
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif#2#
            Exit();
            yield return null;#1#
        //}


    }

    //checks if the User is present 
    public void UserPresent()
    {
//        userPresent = true;
    }

    //if user not present and End is not reached activates the userCheckLock, If the end is reached starts the EndProcess
    public void UserNotPresent()
    {
//        userPresent = false;
//        if (!isEndTriggered && !blockEndTriggered && !userCheckLock)
//        {
//            userCheckLock = true;
//            StartCoroutine(checkUser());
//        }
//
//        if (isEndTriggered && !blockEndTriggered)
//            StartCoroutine(EndProccess());
    }*/

/// <summary>
/// user presence check block 
/// </summary>
    
    public void RegisterChangeInPresence()
    {
        Debug.Log("sensor status:" + checkIfUserPresent());
        if (!checkIfUserPresent() && !userCheckLock)
        {
            
            Debug.Log("triggering time out");
            EventManager.TriggerEvent("startTimeOut");
        }

    }

    public void PresenceTimeOut()
    {
        Debug.Log("time out triggered, final check in "  + presenceThreshold + "s");
        userCheckLock = true;
        Invoke(nameof(FinalPresenceCheck),presenceThreshold);
    }

    private void FinalPresenceCheck()
    {
        Debug.Log("final presence check. sensor status: " + checkIfUserPresent());
        if (!checkIfUserPresent())
        {
            
            if(!isEndTriggered)
            {
                Debug.Log("breaking the experiment");
                EventManager.StopListening("startTimeOut",PresenceTimeOut);
                
                StartCoroutine(SaveAndExit());
            }
            
        }
        else
        {
            Debug.Log("sensor status:" + checkIfUserPresent());
            Debug.Log("releasing the lock");
            userCheckLock = false;
        }
    }
/// <summary>
/// end block
/// </summary>
    void Update()
    {
        
        //checking for record or simulation mode
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            //if the progress is further then the trigger on path transfer either to lobby 1 or 0
            if ((WestdriveSettings.Progress > experiment.endTriggerOnPath))
            {
                ChooseModeForStartCoroutine(ADVPaths.Count);
            }
        }
        else if (WestdriveSettings.SimulationMode == mode.simulate)
        {
            //if the progress is further then the trigger on path transfer either to lobby 1 or 0
            if ((WestdriveSettings.Progress >= experiment.endTriggerFramePercentage))
            {
                EventManager.TriggerEvent("stop audio");
                ChooseModeForStartCoroutine(ADVPaths.Count);
            }
        }

    }

    private void ChooseModeForStartCoroutine(int advPathsCount)
    {
        var mode = advPathsCount;
        if (advPathsCount != 0)
        {
            mode = 1;
        }

        StartCoroutine(transferToLobby(mode));
    }


    // saves the recorded data and starts the questionnaire   
    private IEnumerator EndProccess()
    {
        if (WestdriveSettings.SimulationMode == global::mode.simulate)
        {
            if (experiment.usetracking)
            {
                string context = "";
                if (camera.transform.childCount != 0)
                {
                    string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                      camera.GetComponentInChildren<GenericTracker>().currentContext;
                    context = camera.GetComponentInChildren<GenericTracker>().currentContext;
                    Westdrive.IO.SaveToFileAsync(dataPath, camera.GetComponentInChildren<GenericTracker>().recordedData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "trackingData"));
                }
                else
                {
                    string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                      camera.GetComponent<GenericTracker>().currentContext;
                    context = camera.GetComponent<GenericTracker>().currentContext;
                    Westdrive.IO.SaveToFileAsync(dataPath, camera.GetComponent<GenericTracker>().recordedData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "trackingData"));
                }

                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                    yield return null;
                }

                string eventDataPath =
                    Application.dataPath + Westdrive.Settings.ReadValue("output", "events") + context;
                Westdrive.IO.SaveToFileAsync(eventDataPath, WestdriveSettings.EventData,
                    WestdriveSettings.ParticipantUID.ToString(),
                    Westdrive.Settings.ReadValue("extensions", "binary"));
                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                    yield return null;
                }
            }
        }

        Debug.Log("button down");
        Debug.Log("user code =" + WestdriveSettings.ParticipantCode);
        Debug.Log("user uid = " + WestdriveSettings.ParticipantUID);
        if (experiment.openQuestionaireAfterExperiment && !serverError)
        {
            Application.OpenURL(mainServer);
        }

        readyToEnd = false;
        //*/only starts in the Unity Editor
/*#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif*/
        Exit();
        yield return null;
    }

    //Stops all Coroutines 
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // mode == 1 -> end block, mode == 0 -> end experiment
    private IEnumerator transferToLobby(int mode = 1)
    {
        annoucementArea.SetActive(true);
        WestdriveSettings.Progress = 0;
        camera.transform.position = cameraStand.position;
        camera.transform.rotation = cameraStand.rotation;
        camera.transform.parent = cameraStand;
        if (WestdriveSettings.SimulationMode == global::mode.simulate)
        {
            WestdriveSettings.isPlaying = false;
            if (experiment.usetracking)
            {
                if (camera.transform.childCount != 0)
                    camera.GetComponentInChildren<GenericTracker>().StopTracking();
                else
                    camera.GetComponentInChildren<GenericTracker>().StopTracking();
            }
            // this need to be fix for block based experiments

#if UNITY_EDITOR
            if (WestdriveSettings.SimulationMode == global::mode.simulate)
            {
                if (experiment.usetracking)
                {
                    string context = "";
                    if (camera.transform.childCount != 0)
                    {
                        string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                          camera.GetComponentInChildren<GenericTracker>().currentContext;
                        context = camera.GetComponentInChildren<GenericTracker>().currentContext;
                        Westdrive.IO.SaveToFileAsync(dataPath,
                            camera.GetComponentInChildren<GenericTracker>().recordedData,
                            WestdriveSettings.ParticipantUID.ToString(),
                            Westdrive.Settings.ReadValue("extensions", "trackingData"));
                    }
                    else
                    {
                        string dataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "subPath") +
                                          camera.GetComponent<GenericTracker>().currentContext;
                        context = camera.GetComponent<GenericTracker>().currentContext;
                        Westdrive.IO.SaveToFileAsync(dataPath, camera.GetComponent<GenericTracker>().recordedData,
                            WestdriveSettings.ParticipantUID.ToString(),
                            Westdrive.Settings.ReadValue("extensions", "trackingData"));
                    }

                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                        yield return null;
                    }

                    string eventDataPath = Application.dataPath + Westdrive.Settings.ReadValue("output", "events") +
                                           context;
                    Westdrive.IO.SaveToFileAsync(eventDataPath, WestdriveSettings.EventData,
                        WestdriveSettings.ParticipantUID.ToString(),
                        Westdrive.Settings.ReadValue("extensions", "binary"));
                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;
                        yield return null;
                    }
                }
            }
#endif
        }

        if (WestdriveSettings.SimulationMode == global::mode.record)
            experimentData.Data.Add("ADV", GameObject.FindGameObjectWithTag("ADV").GetComponent<CarEngine>().getData());
        Destroy(GameObject.FindGameObjectWithTag("ADV"));
        if (WestdriveSettings.SimulationMode == global::mode.record)
        {
            carManager.ResetSystemAsync(experimentData);
            pedestrianManager.ResetSystemAsync(experimentData);
        }
        else if (WestdriveSettings.SimulationMode == global::mode.simulate)
        {
            carManager.ResetSystemAsync();
            pedestrianManager.ResetSystemAsync();
        }

        if (WestdriveSettings.SimulationMode == global::mode.record)
        {
            Westdrive.IO.SaveToFileAsync(Application.dataPath + "/Resources/Settings/" + experiment.name ,
                experimentData,  "[rename to adv path before run]", Westdrive.Settings.ReadValue("extensions", "binary"));
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = dataSavingText;

                yield return null;
            }
        }


        if (mode == 1)
        {
            announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = endBlockText;

            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            chooseNextBlock();
        }
        else if (mode == 0)
        {
            if (experiment.useOnlineQuestionaire && WestdriveSettings.SimulationMode == global::mode.simulate)
            {
                string serverAddr = mainServer;
                serverError = false;
                do
                {
                    Westdrive.Net.AsyncNetResult netResult = ResisterationResult;
                    string url = serverAddr + "/check?id=" + WestdriveSettings.ParticipantCode +
                                 "&uid=" +
                                 WestdriveSettings.ParticipantUID;
                    Westdrive.Net.getStringRequest(url, netResult);

                    while (WestdriveSettings.CheckIO != IOState.ready)
                    {
                        yield return null;
                    }

                    Debug.Log(request);
                    if (request == "exists")
                    {
                        Debug.Log("Trying new code");
                        WestdriveSettings.ResetParticipantCode();
                    }

                    else if (request == "stored")
                    {
                        Debug.Log("Participant Insertion complete!");
                    }
                    else
                    {
                        if (serverAddr == mainServer && useFallBack)
                        {
                            serverAddr = fallBackServer;
                        }
                        else
                        {
                            Debug.Log("participant insertion failed! " + request);
                            serverError = true;
                        }

                    }

                    yield return null;
                } while (request == "exists" && !serverError);
                
                

                if (serverError)
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = serverErrorText;
                }
                else
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                        //endExperimentWithCodeText + " <" + WestdriveSettings.ParticipantCode +">";
                        string.Format(endExperimentWithCodeText, WestdriveSettings.ParticipantCode);
                    Debug.Log(WestdriveSettings.ParticipantUID);
                    Debug.Log(WestdriveSettings.ParticipantCode);
                }
            }
            else
            {
                announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = endExperimentText;
            }


            isEndTriggered = true;
            EventManager.StopListening("startTimeOut",PresenceTimeOut);
            while (checkIfUserPresent())
            {
                yield return null;
            }

            StartCoroutine(EndProccess());
            StopCoroutine(transferToLobby());
        }

        yield return null;
    }

    private void ResisterationResult(string result)
    {
        request = result;
    }

    //   chooses the next block depending on an random integer
    private void chooseNextBlock()
    {
        StopAllCoroutines();
        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = experimentLoadingText;
        //randomized the next item twice, first by shuffeling and then module random number
        var nextModuleIndex = Random.Range(0, 10000);
        if (experiment.randomizeADVPath)
        {
            Shuffle<string>(ref ADVPaths);

            nextIndex = Random.Range(0, 10000);
            if (ADVPaths.Count != 0)
                nextIndex %= ADVPaths.Count;
            else
                nextIndex = 0;
        }
        else
        {
            nextIndex = 0;
        }

        if (experiment.randomizeADVModules)
        {
            Shuffle<Object>(ref ADVModules);
            if (ADVModules.Count != 0)
                nextModuleIndex %= ADVModules.Count;
            else
                nextModuleIndex = 0;
        }
        else
        {
            nextModuleIndex = nextIndex;
        }

        StartCoroutine(runNextBlock(nextIndex, nextModuleIndex));
    }

    //saves the data
    private void setData(Payload dataFromFile)
    {
        this.experimentData = dataFromFile;
    }

    // starts the Block choosen in chooseNextBlock
    private IEnumerator runNextBlock(int index, int moduleIndex)
    {
        //activates all paths
        for (int path_index = 0; path_index < GameObject.Find("Paths").transform.childCount; path_index++)
        {
            GameObject.Find("Paths").transform.GetChild(path_index).gameObject.SetActive(true);
        }

        //activates all Events
        for (int path_index = 0; path_index < GameObject.Find("Events").transform.childCount; path_index++)
        {
            GameObject.Find("Events").transform.GetChild(path_index).gameObject.SetActive(true);
        }

        //deactivates the Events if the ADV path wasn't smoothed out with the BezierSplines
        for (int path_index = 0; path_index < GameObject.Find("Events").transform.childCount; path_index++)
        {
            if (GameObject.Find("Events").transform.GetChild(path_index).gameObject.GetComponent<EventHandler>()
                    .ADVPath != GameObject.Find(ADVPaths[index]).GetComponent<BezierSplines>())
                GameObject.Find("Events").transform.GetChild(path_index).gameObject.SetActive(false);
        }

        Debug.Log("Disabling car paths");
        // Disables the Car paths
        foreach (StringListDict dict in experiment.disabledCarPaths)
        {
            if (dict.key == ADVPaths[index])
            {
                foreach (string pathName in dict.value)
                {
                    GameObject.Find(pathName).SetActive(false);
                }
            }
        }

        Debug.Log("Disabling pedestrian paths");
        //Disables the pedestrian paths
        foreach (StringListDict dict in experiment.disabledPedestrianPaths)
        {
            if (dict.key == ADVPaths[index])
            {
                foreach (string pathName in dict.value)
                {
                    GameObject.Find(pathName).SetActive(false);
                }
            }
        }

        if (firstBlock && WestdriveSettings.SimulationMode != mode.record)
        {
            if (WestdriveSettings.language == "DE")
            {
                this.GetComponent<AudioSource>().clip = experiment.consetTextGerman;
            }
            else if (WestdriveSettings.language == "ENG")
            {
                this.GetComponent<AudioSource>().clip = experiment.consetTextEnglish;
            }
            
            this.GetComponent<AudioSource>()
                .PlayDelayed(float.Parse(Westdrive.Settings.ReadValue("delays", "consentAudio")));
            
        }

        if (WestdriveSettings.SimulationMode == mode.simulate)
        {
            WestdriveSettings.EventData = new Payload();
            WestdriveSettings.EventData.Data.Clear();


            if (!File.Exists(Application.dataPath + Westdrive.Settings.ReadValue("input", "simulationData") +
                             experiment.profileName + "-" + ADVPaths[index] +
                             Westdrive.Settings.ReadValue("extensions", "binary")))
            {
                setupDone = false;
                Debug.Log("experiment data doesn't exist. downloading latest one from source");
                WebClient client = new WebClient();
                client.DownloadFileCompleted +=
                    new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadFileProgress);
                client.DownloadFileAsync(new Uri(Westdrive.Settings.ReadValue("dataServer", "serverUrl") +
                                                 experiment.profileName + "-" + ADVPaths[index] +
                                                 Westdrive.Settings.ReadValue("extensions", "binary")),
                    Application.dataPath + Westdrive.Settings.ReadValue("input", "simulationData") +
                    experiment.profileName + "-" + ADVPaths[index] +
                    Westdrive.Settings.ReadValue("extensions", "binary"));
                while (!setupDone)
                {
                    if (firstBlock)
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                            consentText + "\n" + string.Format(dataDownloadingText,
                                experiment.profileName + "-" + ADVPaths[index] +
                                Westdrive.Settings.ReadValue("extensions", "binary"), downloadProgress) + "\n";
                        
                    }
                    else
                    {
                        announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                            string.Format(dataDownloadingText, experiment.profileName + "-" + ADVPaths[index] +
                                                               Westdrive.Settings.ReadValue("extensions", "binary"),
                                downloadProgress) + "\n";
                    }

                    yield return null;
                }

                client.DownloadFileCompleted -=
                    new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompleted);
                client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(DownloadFileProgress);
            }


            Westdrive.IO.AsyncReadResult<Payload> readResult = setData;
            Westdrive.IO.LoadAsync<Payload>(
                Application.dataPath + Westdrive.Settings.ReadValue("input", "simulationData") +
                experiment.profileName + "-" + ADVPaths[index] +
                Westdrive.Settings.ReadValue("extensions", "binary"), readResult);
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                if (firstBlock)
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                        consentText + "\n" + dataLoadingText + "\n";
                }
                else
                {
                    announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                        dataLoadingText + "\n";
                }

                yield return new WaitForSeconds(loadingDelay);
            }
        }

        // creates the percentage of the loading bar
        carManager.Init(experimentData);
        pedestrianManager.Init(experimentData);
        blockEndTriggered = false;
        float assetsToLoad = carManager.assetPopulation + pedestrianManager.assetPopulation;
        while (!carManager.loadingDone || !pedestrianManager.loadingDone)
        {
            if (firstBlock)
            {
                announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                    consentText + "\n" + string.Format(experimentLoadingPercentageText,
                        Mathf.FloorToInt(((carManager.loadedInstances + pedestrianManager.loadedInstances) * 100f) /
                                         assetsToLoad).ToString()) + "\n";
            }
            else
            {
                announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text =
                    string.Format(experimentLoadingPercentageText,
                        Mathf.FloorToInt(((carManager.loadedInstances + pedestrianManager.loadedInstances) * 100f) /
                                         assetsToLoad).ToString()) + "\n";
            }

            yield return null;
        }


        yield return null;
        if (firstBlock)
        {

            while (this.GetComponent<AudioSource>().isPlaying)
            {
                announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = consentText + "\n" + transferText + "\n";
                yield return null;
            }

            firstBlock = false;
        }

        if (!checkIfUserPresent())
        {
            Exit();
            StopAllCoroutines();
        }
        else
        {
            announcementBoard.GetComponent<TMPro.TextMeshProUGUI>().text = transferText;
        }
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            yield return new WaitForSeconds(float.Parse(Westdrive.Settings.ReadValue("delays", "recordDelay")));
        }
        GameObject ADV = Instantiate<GameObject>(experiment.ADVPrefab);
        ADV.SetActive(false);
        // activates the ADV sequenz and adjusts camera position in record mode
        if (WestdriveSettings.SimulationMode == mode.record)
        {
            ADV.GetComponent<CarEngine>().path = GameObject.Find(ADVPaths[index]).GetComponent<BezierSplines>();
            ADV.GetComponent<CarEngine>().startPecentage = startPercentage;
            ADV.SetActive(true);
            camera.transform.position = ADV.GetComponent<CarEngine>().cameraPos.position;
            camera.transform.rotation = ADV.GetComponent<CarEngine>().cameraPos.rotation;
            camera.transform.parent = ADV.GetComponent<CarEngine>().cameraPos;
            annoucementArea.SetActive(false);

            
            if (ADVModules[moduleIndex].name == "AVAS")
            {
#if NOTTS
                ADV.AddComponent<AVAS>();
                ADV.GetComponent<AVAS>().AVASMixerOutput = AVASOutput;
#elif TTS
                ADV.AddComponent<AVAS>();
#endif
            }

            if (ADVModules[moduleIndex].name == "RadioTalk")
            {
                ADV.GetComponent<CarEngine>().radio.SetActive(true);
            }

            if (ADVModules[moduleIndex].name == "TaxiDriver")
            {
                ADV.GetComponent<CarEngine>().taxiDriver.SetActive(true);
                ADV.GetComponent<CarEngine>().isTaxiDriver = true;
                ADV.AddComponent<TaxiDriverReaction>();
                ADV.GetComponent<TaxiDriverReaction>().reactionMixerOutput = reactionsOutput;
            }
        }
        else if (WestdriveSettings.SimulationMode == mode.simulate)
        {
            //Maybe write for this task a own mapper
            CarEngineReplay carEngineReplay = ADV.AddComponent<CarEngineReplay>();
            CarEngine carEngine = ADV.GetComponent<CarEngine>();
//            ADV.AddComponent<CarEngineReplay>();
            carEngineReplay.cameraPos = carEngine.cameraPos;
            carEngineReplay.taxiDriver = carEngine.taxiDriver;
            carEngineReplay.radio = carEngine.radio;
            carEngineReplay.SteeringWheel = carEngine.SteeringWheel;
            carEngineReplay.Profile = carEngine.Profile;
            carEngineReplay.wheelFL = carEngine.wheelFL;
            carEngineReplay.wheelFR = carEngine.wheelFR;
            carEngineReplay.wheelRL = carEngine.wheelRL;
            carEngineReplay.wheelRR = carEngine.wheelRR;
            carEngineReplay.caliperWheelFL = carEngine.caliperWheelFL;
            carEngineReplay.caliperWheelFR = carEngine.caliperWheelFR;
            carEngineReplay.path = GameObject.Find(ADVPaths[index]).GetComponent<BezierSplines>();
            carEngineReplay.dataPoints = experimentData.Data["ADV"];

            Destroy(carEngine);
            
            ADV.SetActive(true);
            camera.transform.position = carEngineReplay.cameraPos.position;
            camera.transform.rotation = carEngineReplay.cameraPos.rotation;
            camera.transform.parent = carEngineReplay.cameraPos;

            if (experiment.usetracking)
            {
                var trackerType = Type.GetType(experiment.Tracker.name);
                if (camera.transform.childCount != 0)
                {
                    camera.GetComponentInChildren<Camera>().gameObject.AddComponent(trackerType);
                    camera.GetComponentInChildren<Camera>().gameObject.GetComponent<GenericTracker>().currentContext =
                        ADVPaths[index] + "-" + ADVModules[moduleIndex].name;
                    camera.GetComponentInChildren<Camera>().gameObject.GetComponent<GenericTracker>().TrackAsync();
                }
                else
                {
                    camera.AddComponent(trackerType);
                    camera.GetComponent<GenericTracker>().currentContext =
                        ADVPaths[index] + "-" + ADVModules[moduleIndex].name;
                    camera.GetComponent<GenericTracker>().TrackAsync();
                }
            }

            annoucementArea.SetActive(false);
            if (ADVModules[moduleIndex].name == "AVAS")
            {
#if NOTTS
                ADV.AddComponent<AVAS>();
                ADV.GetComponent<AVAS>().AVASMixerOutput = AVASOutput;
#elif TTS
                ADV.AddComponent<AVAS>();
#endif
            }

            if (ADVModules[moduleIndex].name == "RadioTalk")
            {
                carEngineReplay.radio.SetActive(true);
            }

            if (ADVModules[moduleIndex].name == "TaxiDriver")
            {
                carEngineReplay.taxiDriver.SetActive(true);
                carEngineReplay.isTaxiDriver = true;
                ADV.AddComponent<TaxiDriverReaction>();
                ADV.GetComponent<TaxiDriverReaction>().reactionMixerOutput = reactionsOutput;
            }
        }

        if (!experiment.reuseADVPath)
        {
            ADVModules.RemoveAt(moduleIndex);
            ADVPaths.RemoveAt(index);
        }

        yield return null;
    }

    //shuffles the List T with a random variable
    void Shuffle<T>(ref List<T> array)
    {
        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
        int p = array.Count;
        for (int n = p - 1; n > 0; n--)
        {
            int r = rnd.Next(0, n);
            T t = array[r];
            array[r] = array[n];
            array[n] = t;
        }
    }
    private bool checkIfUserPresent()
    {
#if UNITY_EDITOR
        return true;
#else
        return SteamVR_Actions._default.HeadsetOnHead[SteamVR_Input_Sources.Any].state;
#endif
    }
    void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            setupDone = true;
        }
    }

    void DownloadFileProgress(object sender, DownloadProgressChangedEventArgs e)
    {
        downloadProgress = e.ProgressPercentage;
    }
}