using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;


public class EyeTraking : MonoBehaviour {
    /*private SMI.SMIEyeTrackingUnity instance;
    string logMessage = "";
    [Header("Debug Settings")]
    public bool isDebugMode = false;
    [Space]
    [Header("SMI camera")]
    public GameObject SMICamera;
    public GameObject localCamera;
    [Space]
    [Header("Data Settings")]
    public string participantID;
    public string age;
    public string population;
    public string type;
    public bool includeNullObjects = false;
    private string fileName;
    string filePath;
    public string delimiter = ",";
    private List<string[]> rowData = new List<string[]>();
    IEnumerator checkTracking()
    {
        logMessage = "";
        GameObject gameObj = instance.smi_GetGazedObject();
        if (gameObj != null)
        {
            logMessage += ("Gazed object: " + gameObj.transform.parent.name + " timestamp: " + instance.smi_GetTimeStamp().ToString() + "\n");
            string[] dataPoint = new string[4];
            dataPoint[0] = instance.smi_GetTimeStamp().ToString();
            if (gameObj.GetComponentInParent<EyeTrackingTarget>() != null)
            {
                dataPoint[1] = gameObj.GetComponentInParent<EyeTrackingTarget>().gameObject.name;
                dataPoint[2] = gameObj.transform.parent.name;
                dataPoint[3] = gameObj.name;
                rowData.Add(dataPoint);
            }

            else
            {
                dataPoint[1] = "null";
                dataPoint[2] = gameObj.transform.parent.name;
                dataPoint[3] = gameObj.name;
                if (includeNullObjects)
                {
                    rowData.Add(dataPoint);
                }
            }


        }
        if (isDebugMode)
            Debug.Log(logMessage);
        yield return new WaitForEndOfFrame();
    }
    public void saveData()
    {
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);


        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));




        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
        Debug.Log("saved");
    }
    private void Awake()
    {


    }
    void Start () {
        fileName = participantID + "_" + age +"_" + population +  "_" + this.gameObject.name + "_" + type + ".csv";
        filePath = Application.dataPath + "/CSV/" + fileName;
        instance = SMI.SMIEyeTrackingUnity.Instance;
        
        
        SMICamera.transform.parent = localCamera.transform;
        string[] dataPoint = new string[4];
        dataPoint[0] = "timestamp";
        dataPoint[1] = "object_category";
        dataPoint[2] = "object_subcategory";
        dataPoint[3] = "object_name";
        rowData.Add(dataPoint);
        

    }
    private void OnDisable()
    {
        Debug.Log("saving");
        saveData();
    }
    private void OnDestroy()
    {
        Debug.Log("saving");
        saveData();
    }
    // Update is called once per frame
    void Update () {
        if (instance == null)
            return;
        checkTrackingData();
    }
    private void FixedUpdate()
    {
       
    }
    public void checkTrackingData()
    {
        logMessage = "";
        GameObject gameObj = instance.smi_GetGazedObject();
        if (gameObj != null)
        {
            logMessage += ("Gazed object: " + gameObj.transform.parent.name + " timestamp: " + instance.smi_GetTimeStamp().ToString() + "\n");
            string[] dataPoint = new string[4];
            dataPoint[0] = instance.smi_GetTimeStamp().ToString();
            if (gameObj.GetComponentInParent<EyeTrackingTarget>() != null)
            {
                dataPoint[1] = gameObj.GetComponentInParent<EyeTrackingTarget>().gameObject.name;
                dataPoint[2] = gameObj.transform.parent.name;
                dataPoint[3] = gameObj.name;
                rowData.Add(dataPoint);
            }

            else
            {
                dataPoint[1] = "null";
                dataPoint[2] = gameObj.transform.parent.name;
                dataPoint[3] = gameObj.name;
                if (includeNullObjects)
                {
                    rowData.Add(dataPoint);
                }
            }


        }
        if (isDebugMode)
            Debug.Log(logMessage);
    }
    */
}
