using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = System.Object;


public class VisualisationGUI : MonoBehaviour
{
    [Header("Data")] private List<FileInfo> dataFiles;
    private List<string> fileNames;
    private Payload experimentData;
    private int firstFrame;
    private int lastFrame;


    [Header("GUI Components")] private int selectedFileIndex = 0;

    private Rect window;
    private Rect cameraViewPort;
    private Vector2 scrollPosition = Vector2.zero;
    private string infoText = "";
    private string batchInfoText = "";
    [Header("Internal Components")] private PedestrianManager pedestrianManager;
    private CarsManager carsManager;
    private ProcedureProfile experiment;
    private visualisationMode visualisationMode;

    [Header("Booleans")] private bool playSelected = false;
    private bool playAll = false;
    private bool isLoading = false;
    private bool process = false;
    private bool hit = false;
    private bool combinedHit = false;
    private bool pause = false;
    private bool showControls = false;
    private string newFrameNumber = "0";
    private bool changingFrame = false;

    private bool batchProcess = false;

    private bool disableRenderers = false;

    // Start is called before the first frame update
    private void OnGUI()
    {
        if (visualisationMode == visualisationMode.unselcted)
        {
            Rect processBtn = new Rect((Screen.width / 2) - 300, (Screen.height / 2) - 50, 200, 100);
            Rect hitBtn = new Rect((Screen.width / 2), (Screen.height / 2) - 50, 200, 100);
            Rect CombinedBtn = new Rect((Screen.width / 2) + 300, (Screen.height / 2) - 50, 200, 100);
            process = GUI.Button(processBtn,
                "Raycast with tracking Data");
            hit = GUI.Button(hitBtn,
                "Visualise hit data");
            combinedHit = GUI.Button(CombinedBtn,
                "Visualise combined data");
        }
        else if (visualisationMode == visualisationMode.process)
        {
            if (!isLoading && !WestdriveSettings.isPlaying)
            {
                GUI.backgroundColor = Color.grey;
                GUI.contentColor = Color.white;
                window = GUI.Window(1, window, processFileListView, "Available Files");
            }
            else if (isLoading && !WestdriveSettings.isPlaying)
            {
                GUI.contentColor = Color.white;
                GUIStyle infoStyle = new GUIStyle();
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 30;
                centeredStyle.fontStyle = FontStyle.Bold;
                GUI.Label(cameraViewPort, infoText, centeredStyle);
            }
            else if (!isLoading && WestdriveSettings.isPlaying)
            {
                GUI.contentColor = Color.white;
                GUIStyle infoStyle = new GUIStyle();
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.LowerCenter;
                centeredStyle.fontSize = 30;
                centeredStyle.fontStyle = FontStyle.Bold;
                GUI.Label(this.gameObject.GetComponent<Camera>().pixelRect, batchInfoText, centeredStyle);
            }

            if (batchProcess)
            {
                GUI.contentColor = Color.white;
                GUIStyle infoStyle = new GUIStyle();
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 30;
                centeredStyle.fontStyle = FontStyle.Bold;
                GUI.Label(this.gameObject.GetComponent<Camera>().pixelRect, infoText, centeredStyle);
            }
        }
        else if (visualisationMode == visualisationMode.hit)
        {
            if (!isLoading && !WestdriveSettings.isPlaying && !showControls)
            {
                GUI.backgroundColor = Color.grey;
                GUI.contentColor = Color.white;
                window = GUI.Window(2, window, hitFileListView, "Available Files");
            }
            else if (isLoading && !WestdriveSettings.isPlaying)
            {
                GUI.contentColor = Color.white;
                GUIStyle infoStyle = new GUIStyle();
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 30;
                centeredStyle.fontStyle = FontStyle.Bold;
                GUI.Label(cameraViewPort, infoText, centeredStyle);
            }
            else if (showControls)
            {
                Rect frameLbl = new Rect((Screen.width / 2) - 550, Screen.height - 200, 1000, 30);
                Rect pausePlayBtn = new Rect((Screen.width / 2) - 100, Screen.height - 100, 100, 30);

                var centeredFrameLableStyle = GUI.skin.GetStyle("Label");
                centeredFrameLableStyle.alignment = TextAnchor.MiddleCenter;
                centeredFrameLableStyle.fontSize = 15;
                centeredFrameLableStyle.fontStyle = FontStyle.Bold;
                GUI.Label(frameLbl,
                    "Playing frame <" + TimeGaurd.getCurrentFrame() + "> from total <" + lastFrame +
                    ">frames. First frame number: " + firstFrame, centeredFrameLableStyle);
                if (GUI.Button(pausePlayBtn, "Pause / Play"))
                {
                    newFrameNumber = TimeGaurd.getCurrentFrame().ToString();
                    WestdriveSettings.isPlaying = !WestdriveSettings.isPlaying;
                    pause = !pause;
                }

                if (pause)
                {
                    int frameNumberAtPause = TimeGaurd.getCurrentFrame();


                    Rect changeBtn = new Rect((Screen.width / 2) - 200, Screen.height - 150, 100, 30);
                    Rect changeFrameBtn = new Rect((Screen.width / 2) - 90, Screen.height - 150, 100, 30);
                    Rect screenShotBtn = new Rect((Screen.width / 2) + 20, Screen.height - 150, 100, 30);
                    Rect pastFrameBtn = new Rect((Screen.width / 2) - 160, Screen.height - 100, 50, 30);
                    Rect nextFrameBtn = new Rect((Screen.width / 2) + 10, Screen.height - 100, 50, 30);
                    Rect hintLbl = new Rect((Screen.width / 2) - 550, Screen.height - 50, 1000, 30);
                    var centeredStyleTextField = GUI.skin.GetStyle("TextField");
                    centeredStyleTextField.alignment = TextAnchor.MiddleCenter;
                    newFrameNumber = GUI.TextField(changeFrameBtn, newFrameNumber, centeredStyleTextField);
                    if (GUI.Button(changeBtn, "Jump to:"))
                    {
                        if (int.Parse(newFrameNumber) >= firstFrame && int.Parse(newFrameNumber) <= lastFrame)
                        {
                            changingFrame = true;
                            this.GetComponent<TrackerReplay>().setCurretFrame(int.Parse(newFrameNumber));
                            StartCoroutine(playAFrame());
                        }
                    }

                    if (GUI.Button(screenShotBtn, "Screenshot"))
                    {
                        ScreenCapture.CaptureScreenshot(Application.dataPath +
                                                        Westdrive.Settings.ReadValue("output", "screenshots") +
                                                        dataFiles[selectedFileIndex].Name.Replace(
                                                            dataFiles[selectedFileIndex].Extension,
                                                            TimeGaurd.getCurrentFrame().ToString() +
                                                            Westdrive.Settings.ReadValue("extensions", "image")));
                    }

                    if (GUI.Button(pastFrameBtn, "<|"))
                    {
                        if (TimeGaurd.getCurrentFrame() - 1 >= firstFrame)
                        {
                            changingFrame = true;
                            this.GetComponent<TrackerReplay>().setCurretFrame(TimeGaurd.getCurrentFrame() - 1);
                            StartCoroutine(playAFrame());
                        }
                    }

                    if (GUI.Button(nextFrameBtn, "|>"))
                    {
                        if (TimeGaurd.getCurrentFrame() + 1 <= lastFrame)
                        {
                            changingFrame = true;
                            this.GetComponent<TrackerReplay>().setCurretFrame(TimeGaurd.getCurrentFrame() + 1);
                            StartCoroutine(playAFrame());
                        }
                    }


                    var centeredStyle = GUI.skin.GetStyle("Label");
                    centeredStyle.alignment = TextAnchor.MiddleCenter;
                    centeredStyle.fontSize = 15;
                    centeredStyle.fontStyle = FontStyle.Bold;
                    GUI.Label(hintLbl, "Press <End> button to release and lock the cursor ", centeredStyle);
                }
            }

            if (batchProcess)
            {
                GUI.contentColor = Color.white;
                GUIStyle infoStyle = new GUIStyle();
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 30;
                centeredStyle.fontStyle = FontStyle.Bold;
                GUI.Label(this.gameObject.GetComponent<Camera>().pixelRect, infoText, centeredStyle);
            }
        }
        else if (visualisationMode == visualisationMode.combinedHit)
        {
            if (!isLoading && !WestdriveSettings.isPlaying && !showControls)
            {
                GUI.backgroundColor = Color.grey;
                GUI.contentColor = Color.white;
                window = GUI.Window(2, window, combinedHitFileListView, "Available Files");
            }
            else if (isLoading && !WestdriveSettings.isPlaying)
            {
                GUI.contentColor = Color.white;
                GUIStyle infoStyle = new GUIStyle();
                var centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.MiddleCenter;
                centeredStyle.fontSize = 30;
                centeredStyle.fontStyle = FontStyle.Bold;
                GUI.Label(cameraViewPort, infoText, centeredStyle);
            }
            else if (showControls)
            {
                Rect frameLbl = new Rect((Screen.width / 2) - 550, Screen.height - 200, 1000, 30);
                Rect pausePlayBtn = new Rect((Screen.width / 2) - 100, Screen.height - 100, 100, 30);

                var centeredFrameLableStyle = GUI.skin.GetStyle("Label");
                centeredFrameLableStyle.alignment = TextAnchor.MiddleCenter;
                centeredFrameLableStyle.fontSize = 15;
                centeredFrameLableStyle.fontStyle = FontStyle.Bold;
                GUI.Label(frameLbl,
                    "Playing frame <" + TimeGaurd.getCurrentFrame() + "> from total <" + lastFrame +
                    ">frames. First frame number: " + firstFrame, centeredFrameLableStyle);
                if (GUI.Button(pausePlayBtn, "Pause / Play"))
                {
                    newFrameNumber = TimeGaurd.getCurrentFrame().ToString();
                    WestdriveSettings.isPlaying = !WestdriveSettings.isPlaying;
                    pause = !pause;
                }

                if (pause)
                {
                    int frameNumberAtPause = TimeGaurd.getCurrentFrame();

//                    Rect changeBtn = new Rect((Screen.width/2) - 160,Screen.height - 150,100,30);
//                    Rect changeFrameBtn = new Rect((Screen.width/2) - 50,Screen.height - 150,100,30);
                    Rect changeBtn = new Rect((Screen.width / 2) - 200, Screen.height - 150, 100, 30);
                    Rect changeFrameBtn = new Rect((Screen.width / 2) - 90, Screen.height - 150, 100, 30);
                    Rect screenShotBtn = new Rect((Screen.width / 2) + 20, Screen.height - 150, 100, 30);
                    Rect pastFrameBtn = new Rect((Screen.width / 2) - 160, Screen.height - 100, 50, 30);
                    Rect nextFrameBtn = new Rect((Screen.width / 2) + 10, Screen.height - 100, 50, 30);
                    Rect hintLbl = new Rect((Screen.width / 2) - 550, Screen.height - 50, 1000, 30);
                    var centeredStyleTextField = GUI.skin.GetStyle("TextField");
                    centeredStyleTextField.alignment = TextAnchor.MiddleCenter;
                    newFrameNumber = GUI.TextField(changeFrameBtn, newFrameNumber, centeredStyleTextField);
                    if (GUI.Button(changeBtn, "Jump to:"))
                    {
                        if (int.Parse(newFrameNumber) >= firstFrame && int.Parse(newFrameNumber) <= lastFrame)
                        {
                            changingFrame = true;
                            this.GetComponent<TrackerCombinedRelay>().setCurretFrame(int.Parse(newFrameNumber));
                            StartCoroutine(playAFrame());
                        }
                    }

                    if (GUI.Button(screenShotBtn, "Screenshot"))
                    {
                        ScreenCapture.CaptureScreenshot(Application.dataPath +
                                                        Westdrive.Settings.ReadValue("output", "screenshots") +
                                                        dataFiles[selectedFileIndex].Name.Replace(
                                                            dataFiles[selectedFileIndex].Extension,
                                                            TimeGaurd.getCurrentFrame().ToString() +
                                                            Westdrive.Settings.ReadValue("extensions", "image")));
                    }

                    if (GUI.Button(pastFrameBtn, "<|"))
                    {
                        if (TimeGaurd.getCurrentFrame() - 1 >= firstFrame)
                        {
                            changingFrame = true;
                            this.GetComponent<TrackerCombinedRelay>().setCurretFrame(TimeGaurd.getCurrentFrame() - 1);
                            StartCoroutine(playAFrame());
                        }
                    }

                    if (GUI.Button(nextFrameBtn, "|>"))
                    {
                        if (TimeGaurd.getCurrentFrame() + 1 <= lastFrame)
                        {
                            changingFrame = true;
                            this.GetComponent<TrackerCombinedRelay>().setCurretFrame(TimeGaurd.getCurrentFrame() + 1);
                            StartCoroutine(playAFrame());
                        }
                    }


                    var centeredStyle = GUI.skin.GetStyle("Label");
                    centeredStyle.alignment = TextAnchor.MiddleCenter;
                    centeredStyle.fontSize = 15;
                    centeredStyle.fontStyle = FontStyle.Bold;
                    GUI.Label(hintLbl, "Press <End> button to release and lock the cursor ", centeredStyle);
                }
            }
        }
    }

    private IEnumerator playAFrame()
    {
        WestdriveSettings.isPlaying = true;
        yield return new WaitForEndOfFrame();
        WestdriveSettings.isPlaying = false;
        yield return null;
        changingFrame = false;
    }

    private void combinedHitFileListView(int id)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(20, 20, 500, window.height - 100), scrollPosition,
            new Rect(0, 0, 500, window.height - 100));
        selectedFileIndex = GUI.SelectionGrid(new Rect(5, 5, 450, 200),
            selectedFileIndex, fileNames.ToArray(), 1);
        GUI.EndScrollView();
        if (fileNames.Count != 0)
        {
            playSelected = GUI.Button(new Rect(90, window.height - 80, 100, 50), "Play Selected");
//            playAll = GUI.Button(new Rect(200, window.height - 80, 100, 50), "Combine");   
        }

        if (GUI.Button(new Rect(310, window.height - 80, 100, 50), "Home"))
        {
            visualisationMode = visualisationMode.unselcted;
            WestdriveSettings.visualisationMode = visualisationMode.unselcted;
        }

        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 15;

        GUI.Label(new Rect(20, window.height - 20, 500, 50),
            "To enable and disable the flying camera please press the space bar.", textStyle);
        GUI.DragWindow();
    }

    private void hitFileListView(int id)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(20, 20, 500, window.height - 100), scrollPosition,
            new Rect(0, 0, 500, window.height - 100));
        selectedFileIndex = GUI.SelectionGrid(new Rect(5, 5, 450, 200),
            selectedFileIndex, fileNames.ToArray(), 1);
        GUI.EndScrollView();
        if (fileNames.Count != 0)
        {
            playSelected = GUI.Button(new Rect(90, window.height - 80, 100, 50), "Play Selected");
            playAll = GUI.Button(new Rect(200, window.height - 80, 100, 50), "Combine");
        }

        if (GUI.Button(new Rect(310, window.height - 80, 100, 50), "Home"))
        {
            visualisationMode = visualisationMode.unselcted;
            WestdriveSettings.visualisationMode = visualisationMode.unselcted;
        }

        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 15;

        GUI.Label(new Rect(20, window.height - 20, 500, 50),
            "To enable and disable the flying camera please press the space bar.", textStyle);
        GUI.DragWindow();
    }

    private void processFileListView(int id)
    {
        scrollPosition = GUI.BeginScrollView(new Rect(20, 20, 500, window.height - 100), scrollPosition,
            new Rect(0, 0, 500, window.height - 100));
        selectedFileIndex = GUI.SelectionGrid(new Rect(5, 5, 450, 200),
            selectedFileIndex, fileNames.ToArray(), 1);
        GUI.EndScrollView();
        disableRenderers = GUI.Toggle(new Rect(90, window.height - 100, 200, 50), disableRenderers,
            "Disable meshes on process.");
        if (fileNames.Count != 0)
        {
            playSelected = GUI.Button(new Rect(90, window.height - 80, 100, 50), "Play Selected");
            playAll = GUI.Button(new Rect(200, window.height - 80, 100, 50), "Play All");
        }

        if (GUI.Button(new Rect(310, window.height - 80, 100, 50), "Home"))
        {
            visualisationMode = visualisationMode.unselcted;
            WestdriveSettings.visualisationMode = visualisationMode.unselcted;
        }

        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 15;

        GUI.Label(new Rect(20, window.height - 20, 500, 50),
            "To enable and disable the flying camera please press the space bar.", textStyle);
        GUI.DragWindow();
    }

    public void setFiles(List<FileInfo> data)
    {
        this.dataFiles.Clear();
        this.fileNames.Clear();
        this.dataFiles = data;
        foreach (FileInfo file in this.dataFiles)
        {
            fileNames.Add(file.Name);
        }
    }

    private void Awake()
    {
        visualisationMode = visualisationMode.unselcted;
        WestdriveSettings.processedData = new List<AnalyzableData>();
        WestdriveSettings.hitData = new SerializableHitData();
        WestdriveSettings.EventData = new Payload();
        WestdriveSettings.isPlaying = false;
        AudioListener.volume = 0;
        dataFiles = new List<FileInfo>();
        fileNames = new List<string>();
        fileNames.Add("No files found");

        window = new Rect(
            this.gameObject.GetComponent<Camera>().pixelRect.center -
            (this.gameObject.GetComponent<Camera>().pixelRect.size / 2),
            this.gameObject.GetComponent<Camera>().pixelRect.size / 2);
        cameraViewPort = this.gameObject.GetComponent<Camera>().pixelRect;
        window.width = 540;
        WestdriveSettings.trackableLayerMask = UnityEngine.LayerMask.NameToLayer("trackingTarget");
    }

    // Update is called once per frame
    private void Update()
    {
        window = new Rect(
            this.gameObject.GetComponent<Camera>().pixelRect.center -
            (this.gameObject.GetComponent<Camera>().pixelRect.size / 2),
            this.gameObject.GetComponent<Camera>().pixelRect.size / 2);
        cameraViewPort = this.gameObject.GetComponent<Camera>().pixelRect;
        if (visualisationMode == visualisationMode.unselcted)
        {
            if (hit)
            {
                dataFiles.Clear();
                fileNames.Clear();
                this.setFiles(
                    Westdrive.IO.FilesInDirectory(Application.dataPath +
                                                  Westdrive.Settings.ReadValue("output", "hit")));
                visualisationMode = visualisationMode.hit;
                WestdriveSettings.visualisationMode = visualisationMode.hit;
                hit = false;
            }

            if (process)
            {
                dataFiles.Clear();
                fileNames.Clear();
                this.setFiles(
                    Westdrive.IO.FilesInDirectory(Application.dataPath +
                                                  Westdrive.Settings.ReadValue("output", "subPath")));
                visualisationMode = visualisationMode.process;
                WestdriveSettings.visualisationMode = visualisationMode.process;
                process = false;
            }

            if (combinedHit)
            {
                dataFiles.Clear();
                fileNames.Clear();
                this.setFiles(
                    Westdrive.IO.FilesInDirectory(Application.dataPath +
                                                  Westdrive.Settings.ReadValue("output", "combined")));
                visualisationMode = visualisationMode.combinedHit;
                WestdriveSettings.visualisationMode = visualisationMode.combinedHit;
                process = false;
            }
        }
        else
        {
            if (playSelected)
            {
                StartCoroutine(ProcessSingleData());
                playSelected = false;
            }
            else if (playAll)
            {
                if (visualisationMode == visualisationMode.process)
                {
                    batchProcess = true;
                    StartCoroutine(BatchProcessAll());
                }
                else if (visualisationMode == visualisationMode.hit)
                {
                    batchProcess = true;
                    StartCoroutine(Combine());
                }

                playAll = false;
            }
        }

        if (WestdriveSettings.isPlaying && !batchProcess)
        {
            if (TimeGaurd.getCurrentFrame() == lastFrame)
            {
                StartCoroutine(EndVisualisation());
            }
            else
            {
                infoText = ((TimeGaurd.getCurrentFrame() * 100) / lastFrame).ToString() + "% of processing <" +
                           dataFiles[selectedFileIndex].Name + "> is done.";
            }
        }

        if (visualisationMode == visualisationMode.hit)
        {
            if (pause && !changingFrame)
            {
                FlyCam flyCam = gameObject.GetComponent<FlyCam>();
                if (WestdriveSettings.isPlaying)
                {
                    if (flyCam != null)
                    {
                        Destroy(flyCam);
                    }
                }
                else
                {
                    if (flyCam == null)
                    {
                        this.gameObject.AddComponent<FlyCam>();
                        this.gameObject.GetComponent<FlyCam>().releaseCursor();
                    }
                }
            }
        }
    }

    private IEnumerator Combine()
    {
        batchProcess = true;
        Dictionary<string, SerializableHitData> fullDataSet = new Dictionary<string, SerializableHitData>();
        Dictionary<string, SerializableCombinedHitData> finalData =
            new Dictionary<string, SerializableCombinedHitData>();
        foreach (FileInfo file in dataFiles)
        {
            if (file.Extension != ".meta")
            {
                SerializableHitData hitData = new SerializableHitData();
                Westdrive.IO.AsyncReadResult<SerializableHitData> loadResult = data => { hitData = data; };
                //TODO: recondsider this method because the type argument specification is redundant
                Westdrive.IO.LoadAsync<SerializableHitData>(file.FullName, loadResult);
                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    batchInfoText = "Loading experiment data from : " + experiment.profileName +
                                    Westdrive.Settings.ReadValue("extensions", "binary");
                    yield return null;
                }

                fullDataSet.Add(file.Name, hitData);
                //yield return null;
            }
        }

        int totalElements = 0;
        int elementsDone = 0;
        foreach (KeyValuePair<string, SerializableHitData> fileData in fullDataSet)
        {
            foreach (KeyValuePair<int, HitPositionType> frameData in fileData.Value.Data)
            {
                totalElements++;
                //yield return null;
            }
        }

        //TODO: this lines looks a little bit wired
        batchInfoText = "total elemets to process: " + totalElements;
        batchInfoText = "aggregation started";
        foreach (KeyValuePair<string, SerializableHitData> fileData in fullDataSet)
        {
            if (finalData.ContainsKey(fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]))
            {
                foreach (KeyValuePair<int, HitPositionType> frameData in fileData.Value.Data)
                {
                    if (finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data
                        .ContainsKey(frameData.Key))
                    {
                        finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data[frameData.Key]
                            .Add(frameData.Value);
                    }
                    else
                    {
                        finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data
                            .Add(frameData.Key, new List<HitPositionType>());
                        finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data[frameData.Key]
                            .Add(frameData.Value);
                    }

                    elementsDone++;
                    batchInfoText = Mathf.FloorToInt((elementsDone * 100) / totalElements) + "% elements processed.";
                    // yield return null;
                }
            }
            else
            {
                finalData.Add(fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1],
                    new SerializableCombinedHitData());
                foreach (KeyValuePair<int, HitPositionType> frameData in fileData.Value.Data)
                {
                    if (finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data
                        .ContainsKey(frameData.Key))
                    {
                        finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data[frameData.Key]
                            .Add(frameData.Value);
                    }
                    else
                    {
                        finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data
                            .Add(frameData.Key, new List<HitPositionType>());
                        finalData[fileData.Key.Split('-')[0] + "-" + fileData.Key.Split('-')[1]].Data[frameData.Key]
                            .Add(frameData.Value);
                    }

                    // yield return null;
                }

                elementsDone++;
                batchInfoText = Mathf.FloorToInt((elementsDone * 100) / totalElements) + "% elements processed.";
            }


            //yield return null;
        }

        foreach (KeyValuePair<string, SerializableCombinedHitData> data in finalData)
        {
            string path = Application.dataPath + Westdrive.Settings.ReadValue("output", "combined") + data.Key;
            Westdrive.IO.SaveToFileAsync(path, data.Value, "combined",
                Westdrive.Settings.ReadValue("extensions", "hitData"));
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                batchInfoText = "saving combined hit data: " + data.Key + "-combined" +
                                Westdrive.Settings.ReadValue("extensions", "hitData");
                yield return null;
            }

            //yield return null;
        }

        batchProcess = false;
        yield return null;
    }

    private IEnumerator BatchProcessAll()
    {
        isLoading = true;
        infoText = "Start loading data";

        int totalFileNumber = 0;
        int processedFiles = 0;
        foreach (FileInfo file in dataFiles)
        {
            if (file.Extension != ".meta")
            {
                totalFileNumber++;
            }
        }

        string lastADVPath = "";
        foreach (FileInfo file in dataFiles)
        {
            string ADVPathName = file.Name.Split('-')[0];
            if (ADVPathName != lastADVPath)
            {
                Westdrive.IO.AsyncReadResult<Payload> readResult = setExperimentData;
                //TODO: Type argument specification, this code could maybe not be executed 
                Westdrive.IO.LoadAsync<Payload>(Application.dataPath +
                                                Westdrive.Settings.ReadValue("input", "simulationData") +
                                                experiment.profileName + "-" + ADVPathName +
                                                Westdrive.Settings.ReadValue("extensions", "binary"), readResult);
                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    infoText = "Loading experiment data from : " + experiment.profileName + "-" + ADVPathName +
                               Westdrive.Settings.ReadValue("extensions", "binary");
                    yield return null;
                }

                lastADVPath = ADVPathName;
            }

            batchInfoText = Mathf.FloorToInt(
                                (processedFiles * 100f) /
                                totalFileNumber) + "% files been processed.";
            if (file.Extension != ".meta")
            {
                WestdriveSettings.EventData.Data.Clear();
                Westdrive.IO.AsyncReadResult<Payload> loadEventsResult = setEventData;
                //TODO: Type argument specification, this code could maybe not be executed 
                Westdrive.IO.LoadAsync<Payload>(
                    Application.dataPath + Westdrive.Settings.ReadValue("output", "events") +
                    file.Name.Replace(file.Extension, Westdrive.Settings.ReadValue("extensions", "binary")),
                    loadEventsResult);
                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    infoText = "Loading event data from : " + file.Name.Replace(file.Extension,
                                   Westdrive.Settings.ReadValue("extensions", "binary"));
                    yield return null;
                }

                Westdrive.IO.AsyncReadResult<TrackingData> loadResult = setTrackerData;
                //TODO: Type argument specification, this code could maybe not be executed 
                Westdrive.IO.LoadAsync<TrackingData>(file.FullName, loadResult);
                while (WestdriveSettings.CheckIO != IOState.ready)
                {
                    infoText = "Loading tracking data from : " + file.Name;
                    yield return null;
                }

                WestdriveSettings.processedData.Clear();
                WestdriveSettings.hitData.Data.Clear();
                StartCoroutine(processFile(file));
                while (isLoading)
                {
                    yield return null;
                }

                while (WestdriveSettings.isPlaying)
                {
                    if (TimeGaurd.getCurrentFrame() == lastFrame)
                    {
                        WestdriveSettings.isPlaying = false;
                        infoText = "Resetting the scene";
                        carsManager.ResetSystemAsync();
                        pedestrianManager.ResetSystemAsync();
                        Destroy(GameObject.FindGameObjectWithTag("ADV"));

                        this.gameObject.GetComponent<GenericTracker>().stopRayCasting();
                        Westdrive.Serialization.CsvTools.SaveObjects(WestdriveSettings.processedData,
                            Application.dataPath + Westdrive.Settings.ReadValue("output", "csv") + file.Name
                                .Replace(file.Extension, ".csv"));

                        Westdrive.IO.SaveToFileAsync(
                            Application.dataPath + Westdrive.Settings.ReadValue("output", "hit") + file.Name
                                .Replace(file.Extension, ""), WestdriveSettings.hitData, "single",
                            Westdrive.Settings.ReadValue("extensions", "hitData"));
                        while (WestdriveSettings.CheckIO != IOState.ready)
                        {
                            infoText = "Saving hit data.";
                            yield return null;
                        }
                    }
                    else
                    {
                        infoText = ((TimeGaurd.getCurrentFrame() * 100) / lastFrame).ToString() + "% of processing <" +
                                   file.Name + "> is done.";
                    }

                    yield return null;
                }

                processedFiles++;
            }
        }

        batchProcess = false;
        yield return null;
    }

    private IEnumerator processFile(FileInfo file)
    {
        isLoading = true;
        infoText = "setting up the environment";
        //activates all paths
        string ADVPathName = file.Name.Split('-')[0];
        Debug.Log(ADVPathName);
        GameObject paths = GameObject.Find("Paths");
        for (int path_index = 0; path_index < paths.transform.childCount; path_index++)
        {
            paths.transform.GetChild(path_index).gameObject.SetActive(true);
        }

        //activates all Events
        GameObject events = GameObject.Find("Events");
        for (int path_index = 0; path_index < events.transform.childCount; path_index++)
        {
            events.transform.GetChild(path_index).gameObject.SetActive(true);
        }

        //deactivates the Events if the ADV path wasn't smoothed out with the BezierSplines
        for (int path_index = 0; path_index < events.transform.childCount; path_index++)
        {
            if (events.transform.GetChild(path_index).gameObject.GetComponent<EventHandler>()
                    .ADVPath != GameObject.Find(ADVPathName).GetComponent<BezierSplines>())
                events.transform.GetChild(path_index).gameObject.SetActive(false);
        }

        Debug.Log("Disabling car paths");
        // Disables the Car paths
        foreach (StringListDict dict in experiment.disabledCarPaths)
        {
            if (dict.key == ADVPathName)
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
            if (dict.key == ADVPathName)
            {
                foreach (string pathName in dict.value)
                {
                    GameObject.Find(pathName).SetActive(false);
                }
            }
        }


        infoText = "Initiating object spawn";
        carsManager.Init(experimentData);
        infoText = "CarsManager initiated";
        pedestrianManager.Init(experimentData);
        infoText = "PedestrianManager initiated";
        float assetsToLoad = carsManager.assetPopulation + pedestrianManager.assetPopulation;
        while (!carsManager.loadingDone || !pedestrianManager.loadingDone)
        {
            float loadedAssets = carsManager.loadedInstances + pedestrianManager.loadedInstances;
            int loadingPercentage = Mathf.FloorToInt(
                ((carsManager.loadedInstances + pedestrianManager.loadedInstances) * 100f) /
                assetsToLoad);
            infoText = "Loading Completed " + loadingPercentage.ToString() + "%. <" + loadedAssets.ToString() +
                       "> assets loaded from <" + assetsToLoad.ToString() + "> total objects";

            yield return null;
        }


        infoText = "Instantiating ADV prefab";
        GameObject ADV = Instantiate<GameObject>(experiment.ADVPrefab);
        ADV.SetActive(false);
        infoText = "Setting up ADV components";
        ADV.AddComponent<CarEngineVisualise>();
//        CarEngineVisualise carEngineVisualise = ADV.AddComponent<CarEngineVisualise>(); //should have the same effect, as the getComponent statement
        MapCarEngineToVisualise(ADV.GetComponent<CarEngineVisualise>(), ADV.GetComponent<CarEngine>(), ADVPathName);
//        CarEngineVisualise carEngineVisualise = ADV.GetComponent<CarEngineVisualise>();
//        CarEngine carEngine = ADV.GetComponent<CarEngine>();
//        carEngineVisualise.cameraPos = carEngine.cameraPos;
//        carEngineVisualise.taxiDriver = carEngine.taxiDriver;
//        carEngineVisualise.radio = carEngine.radio;
//        carEngineVisualise.Profile = carEngine.Profile;
//        carEngineVisualise.path = GameObject.Find(ADVPathName).GetComponent<BezierSplines>();
//        carEngineVisualise.dataPoints = experimentData.Data["ADV"];
//        Destroy(carEngine);
        if (file.Name.Split('-')[1] == "TaxiDriver")
        {
            ADV.GetComponent<CarEngineVisualise>().taxiDriver.SetActive(true);
//            carEngineVisualise.taxiDriver.SetActive(true);
            ADV.GetComponent<CarEngineVisualise>().isTaxiDriver = true;
//            carEngineVisualise.isTaxiDriver = true;
        }

        ADV.SetActive(true);
        if (disableRenderers)
        {
            infoText = "Finding Renderers";
            var meshRenderers = FindObjectsOfType<MeshRenderer>();
            var skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
            int meshCount = meshRenderers.Length + skinnedMeshRenderers.Length;
            infoText = "<" + meshCount.ToString() + "> renderers found";
            int meshesDisabled = 0;
            foreach (var mesh in meshRenderers)
            {
                Destroy(mesh);
                meshesDisabled++;
                infoText = Mathf.FloorToInt(
                               (meshesDisabled * 100f) /
                               meshCount) + "% meshes disabled. current mesh: " + mesh.name;
            }

            foreach (var mesh in skinnedMeshRenderers)
            {
                Destroy(mesh);
                meshesDisabled++;
                infoText = Mathf.FloorToInt(
                               (meshesDisabled * 100f) /
                               meshCount) + "% meshes disabled. current mesh: " + mesh.name;
            }
        }

        infoText = "All done! Transferring camera";

        this.transform.parent = this.transform;


        this.gameObject.GetComponent<GenericTracker>().startRayCasting();

        isLoading = false;
        WestdriveSettings.isPlaying = true;
    }

    private IEnumerator ProcessSingleData()
    {
        isLoading = true;
        infoText = "setting up the environment";
        //activates all paths
        string ADVPathName = dataFiles[selectedFileIndex].Name.Split('-')[0];

        Debug.Log(ADVPathName);
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
                    .ADVPath != GameObject.Find(ADVPathName).GetComponent<BezierSplines>())
                GameObject.Find("Events").transform.GetChild(path_index).gameObject.SetActive(false);
        }

        Debug.Log("Disabling car paths");
        // Disables the Car paths
        foreach (StringListDict dict in experiment.disabledCarPaths)
        {
            if (dict.key == ADVPathName)
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
            if (dict.key == ADVPathName)
            {
                foreach (string pathName in dict.value)
                {
                    GameObject.Find(pathName).SetActive(false);
                }
            }
        }

        infoText = "Start loading data";
        if (visualisationMode == visualisationMode.process)
        {
            Westdrive.IO.AsyncReadResult<TrackingData> loadResult = setTrackerData;
            Westdrive.IO.LoadAsync<TrackingData>(dataFiles[selectedFileIndex].FullName, loadResult);
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                infoText = "Loading tracking data from : " + dataFiles[selectedFileIndex].Name;
                yield return null;
            }

            WestdriveSettings.EventData.Data.Clear();
            Westdrive.IO.AsyncReadResult<Payload> loadEventsResult = setEventData;
            Westdrive.IO.LoadAsync<Payload>(
                Application.dataPath + Westdrive.Settings.ReadValue("output", "events") + dataFiles[selectedFileIndex]
                    .Name.Replace(dataFiles[selectedFileIndex].Extension,
                        Westdrive.Settings.ReadValue("extensions", "binary")), loadEventsResult);
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                infoText = "Loading event data from : " + dataFiles[selectedFileIndex].Name
                               .Replace(dataFiles[selectedFileIndex].Extension,
                                   Westdrive.Settings.ReadValue("extensions", "binary"));
                yield return null;
            }

        }
        else if (visualisationMode == visualisationMode.hit)
        {
            Westdrive.IO.AsyncReadResult<SerializableHitData> loadResult = setHitData;
            Westdrive.IO.LoadAsync<SerializableHitData>(dataFiles[selectedFileIndex].FullName, loadResult);
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                infoText = "Loading hit data from : " + dataFiles[selectedFileIndex].Name;
                yield return null;
            }

            WestdriveSettings.EventData.Data.Clear();
            Westdrive.IO.AsyncReadResult<Payload> loadEventsResult = setEventData;
            Westdrive.IO.LoadAsync<Payload>(
                Application.dataPath + Westdrive.Settings.ReadValue("output", "events") + dataFiles[selectedFileIndex]
                    .Name.Replace("-single", "").Replace(dataFiles[selectedFileIndex].Extension,
                        Westdrive.Settings.ReadValue("extensions", "binary")), loadEventsResult);
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                infoText = "Loading event data from : " + dataFiles[selectedFileIndex].Name
                               .Replace(dataFiles[selectedFileIndex].Extension,
                                   Westdrive.Settings.ReadValue("extensions", "binary"));
                yield return null;
            }
        }
        else if (visualisationMode == visualisationMode.combinedHit)
        {
            Westdrive.IO.AsyncReadResult<SerializableCombinedHitData> loadResult = setCombinedHitData;
            Westdrive.IO.LoadAsync<SerializableCombinedHitData>(dataFiles[selectedFileIndex].FullName, loadResult);
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                infoText = "Loading combined hit data from : " + dataFiles[selectedFileIndex].Name;
                yield return null;
            }
        }

        Westdrive.IO.AsyncReadResult<Payload> readResult = setExperimentData;
        Westdrive.IO.LoadAsync<Payload>(Application.dataPath + Westdrive.Settings.ReadValue("input", "simulationData") +
                                        experiment.profileName + "-" + ADVPathName +
                                        Westdrive.Settings.ReadValue("extensions", "binary"), readResult);
        while (WestdriveSettings.CheckIO != IOState.ready)
        {
            infoText = "Loading experiment data from : " + experiment.profileName + "-" + ADVPathName +
                       Westdrive.Settings.ReadValue("extensions", "binary");
            yield return null;
        }

        infoText = "Initiating object spawn";
        carsManager.Init(experimentData);
        infoText = "CarsManager initiated";
        pedestrianManager.Init(experimentData);
        infoText = "PedestrianManager initiated";
        float assetsToLoad = carsManager.assetPopulation + pedestrianManager.assetPopulation;
        while (!carsManager.loadingDone || !pedestrianManager.loadingDone)
        {
            float loadedAssets = carsManager.loadedInstances + pedestrianManager.loadedInstances;
            int loadingPercentage = Mathf.FloorToInt(
                ((carsManager.loadedInstances + pedestrianManager.loadedInstances) * 100f) /
                assetsToLoad);
            infoText = "Loading Completed " + loadingPercentage.ToString() + "%. <" + loadedAssets.ToString() +
                       "> assets loaded from <" + assetsToLoad.ToString() + "> total objects";


            yield return null;
        }

        infoText = "Instantiating ADV prefab";
        GameObject ADV = Instantiate<GameObject>(experiment.ADVPrefab);
        ADV.SetActive(false);
        infoText = "Setting up ADV components";
        ADV.AddComponent<CarEngineVisualise>();
        MapCarEngineToVisualise(ADV.GetComponent<CarEngineVisualise>(), ADV.GetComponent<CarEngine>(), ADVPathName);
//        ADV.GetComponent<CarEngineVisualise>().cameraPos = ADV.GetComponent<CarEngine>().cameraPos;
//        ADV.GetComponent<CarEngineVisualise>().taxiDriver = ADV.GetComponent<CarEngine>().taxiDriver;
//        ADV.GetComponent<CarEngineVisualise>().radio = ADV.GetComponent<CarEngine>().radio;
//        ADV.GetComponent<CarEngineVisualise>().Profile = ADV.GetComponent<CarEngine>().Profile;
//        ADV.GetComponent<CarEngineVisualise>().path = GameObject.Find(ADVPathName).GetComponent<BezierSplines>();
//        ADV.GetComponent<CarEngineVisualise>().dataPoints = experimentData.Data["ADV"];
//        Destroy(ADV.GetComponent<CarEngine>());

        if (dataFiles[selectedFileIndex].Name.Split('-')[1] == "TaxiDriver")
        {
            ADV.GetComponent<CarEngineVisualise>().taxiDriver.SetActive(true);
            ADV.GetComponent<CarEngineVisualise>().isTaxiDriver = true;
        }

        ADV.SetActive(true);
        if (visualisationMode == visualisationMode.process)
        {
            if (disableRenderers)
            {
                infoText = "Finding Renderers";
                var meshRenderers = FindObjectsOfType<MeshRenderer>();
                var skinnedMeshRenderers = FindObjectsOfType<SkinnedMeshRenderer>();
                int meshCount = meshRenderers.Length + skinnedMeshRenderers.Length;
                infoText = "<" + meshCount.ToString() + "> renderers found";
                int meshesDisabled = 0;
                foreach (var mesh in meshRenderers)
                {
                    Destroy(mesh);
                    meshesDisabled++;
                    infoText = Mathf.FloorToInt(
                                   (meshesDisabled * 100f) /
                                   meshCount) + "% meshes disabled. current mesh: " + mesh.name;
                }

                foreach (var mesh in skinnedMeshRenderers)
                {
                    Destroy(mesh);
                    meshesDisabled++;
                    infoText = Mathf.FloorToInt(
                                   (meshesDisabled * 100f) /
                                   meshCount) + "% meshes disabled. current mesh: " + mesh.name;
                }
            }
        }

        infoText = "All done! Transferring camera";

        this.transform.parent = this.transform;
        if (visualisationMode == visualisationMode.process)
            this.gameObject.GetComponent<GenericTracker>().startRayCasting();
        else if (visualisationMode == visualisationMode.hit || visualisationMode == visualisationMode.combinedHit)
            showControls = true;
        isLoading = false;
        WestdriveSettings.isPlaying = true;
    }

    private void MapCarEngineToVisualise(CarEngineVisualise carEngineVisualise, CarEngine carEngine, string advPathName)
    {
        carEngineVisualise.cameraPos = carEngine.cameraPos;
        carEngineVisualise.taxiDriver = carEngine.taxiDriver;
        carEngineVisualise.radio = carEngine.radio;
        carEngineVisualise.Profile = carEngine.Profile;
        carEngineVisualise.path = GameObject.Find(advPathName).GetComponent<BezierSplines>();
        carEngineVisualise.dataPoints = experimentData.Data["ADV"];
        Destroy(carEngine);
//        throw new NotImplementedException();
    }

    // creates the percentage of the loading bar
    private IEnumerator EndVisualisation()
    {
        WestdriveSettings.isPlaying = false;
        if (visualisationMode == visualisationMode.process)
        {
            this.gameObject.GetComponent<GenericTracker>().stopRayCasting();
            Westdrive.Serialization.CsvTools.SaveObjects(WestdriveSettings.processedData,
                Application.dataPath + Westdrive.Settings.ReadValue("output", "csv") + dataFiles[selectedFileIndex].Name
                    .Replace(dataFiles[selectedFileIndex].Extension, ".csv"));

            Westdrive.IO.SaveToFileAsync(
                Application.dataPath + Westdrive.Settings.ReadValue("output", "hit") + dataFiles[selectedFileIndex].Name
                    .Replace(dataFiles[selectedFileIndex].Extension, ""), WestdriveSettings.hitData, "single",
                Westdrive.Settings.ReadValue("extensions", "hitData"));
            while (WestdriveSettings.CheckIO != IOState.ready)
            {
                infoText = "Saving hit data.";
                yield return null;
            }
        }
        else if (visualisationMode == visualisationMode.hit)
            showControls = false;

        yield return null;
    }

    private void setExperimentData(Payload dataFromFile)
    {
        experimentData = dataFromFile;
    }

    private void setEventData(Payload dataFromFile)
    {
        WestdriveSettings.EventData = dataFromFile;
    }

    private void setTrackerData(TrackingData dataFromFile)
    {
        var trackerType = Type.GetType(experiment.Tracker.name);
        if (this.gameObject.GetComponent(trackerType) == null)
        {
            this.gameObject.AddComponent(trackerType);
        }

        this.gameObject.GetComponent<GenericTracker>().SetData(dataFromFile);
        this.firstFrame = dataFromFile.Data.Keys.First();
        this.lastFrame = dataFromFile.Data.Keys.Last();
        TimeGaurd.setCurrentFrame(dataFromFile.Data.Keys.First());
    }

    private void setHitData(SerializableHitData dataFromFile)
    {
        this.gameObject.AddComponent<TrackerReplay>();
        this.gameObject.GetComponent<TrackerReplay>().SetData(dataFromFile);
        this.firstFrame = dataFromFile.Data.Keys.First();
        this.lastFrame = dataFromFile.Data.Keys.Last();
        TimeGaurd.setCurrentFrame(dataFromFile.Data.Keys.First());
    }

    private void setCombinedHitData(SerializableCombinedHitData dataFromFile)
    {
        this.gameObject.AddComponent<TrackerCombinedRelay>();
        this.gameObject.GetComponent<TrackerCombinedRelay>().SetData(dataFromFile);
        this.firstFrame = dataFromFile.Data.Keys.First();
        this.lastFrame = dataFromFile.Data.Keys.Last();
        TimeGaurd.setCurrentFrame(dataFromFile.Data.Keys.First());
    }

    public void setManagers(CarsManager carsManagerInScene, PedestrianManager pedestrianManagerInScene)
    {
        this.carsManager = carsManagerInScene;
        this.pedestrianManager = pedestrianManagerInScene;
    }


    public void setProfile(ProcedureProfile profile)
    {
        this.experiment = profile;
    }
}