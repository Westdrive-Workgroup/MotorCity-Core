using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarsManagerInspector : EditorWindow
{
    public CarProfileList CarProfiles;
    private int viewIndex = 1;
    private bool startCreatePath;
    private bool startCreateCar;
    private int pathIndex = 0;
    private bool startSpawnPointEditing = false;
    private bool StartEdit = false;
    private bool carButtonEnabled = false;
    private bool spawButtonEnabled = false;
    private bool ShowCarButtonHelp = false;
    private bool createCarManager = false;
    [MenuItem("Window/City AI/Cars/Cars Manager")]
    public static void ShowWindow()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");


        // Adding a Tag
        string s = "car spawn";

        // First check if it is not already present
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(s)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = s;
        }


        // and to save the changes
        tagManager.ApplyModifiedProperties();
        EditorWindow.GetWindow(typeof(CarsManagerInspector));
    }
    private void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            CarProfiles = AssetDatabase.LoadAssetAtPath("/Assets/Resources/Shared Data/", typeof(CarProfileList)) as CarProfileList;
            // "/Assets/Resources/Shared Data/"
        }
    }
    private void OnSceneGUI(SceneView scnView)
    {
    }
    private void OnGUI()
    {
        if (GameObject.Find("CarsManager") == null)
        {
            carButtonEnabled = true;
            spawButtonEnabled = false;
            ShowCarButtonHelp = false;

        }
        else
        {
            carButtonEnabled = false;
            ShowCarButtonHelp = true;
        }
        if (GameObject.FindObjectOfType<BezierSplines>() != null)
        {

            startSpawnPointEditing = true;

        }

        GUI.enabled = carButtonEnabled;
        createCarManager = GUILayout.Button("Create Car Manager System");
        GUI.enabled = true;
        if (ShowCarButtonHelp)
            EditorGUILayout.HelpBox("There should be only one instance of Car manager in the scene at any given time. If you want to create a new Car System please delete the old one first", MessageType.Info);

        GUI.enabled = true;
        startCreatePath = GUILayout.Button("Add a Path");
        if (startCreatePath)
        {
            if (GameObject.Find("Paths") == null)
            {

                GameObject Paths = new GameObject("Paths");
                Undo.RecordObject(Paths, "path system added");
                EditorUtility.SetDirty(Paths);
            }
            GameObject pathSystem = GameObject.Find("Paths");
            BezierSplines[] CurrentPaths = GameObject.FindObjectsOfType<BezierSplines>();
            pathIndex = CurrentPaths.Length;
            EditorGUI.BeginChangeCheck();
            GameObject path = new GameObject("Path_ " + (pathIndex + 1).ToString() + " (Car)");
            Undo.RecordObject(path, "new path added");
            path.transform.parent = pathSystem.transform;
            path.AddComponent<BezierSplines>();
            path.GetComponent<BezierSplines>().pathMode = "Car";
            EditorUtility.SetDirty(path);



        }
        
        if (GameObject.FindObjectOfType<BezierSplines>() != null)
        {

            startSpawnPointEditing = true;

        }
        GUI.enabled = true;
        GUILayout.BeginHorizontal();
        if (!startSpawnPointEditing)
        {
            EditorGUILayout.HelpBox("You have to have at least one path in the scene in order to create spawnpoints", MessageType.Info);
        }
        if (Selection.gameObjects.Length != 0)
        {
            if (Selection.gameObjects[0].GetComponent<BezierSplines>() != null)
                StartEdit = true;
            else
            {
                StartEdit = false;
                EditorGUILayout.HelpBox("There should be at least one Path selected in the scene at any given time. If there is no path created yet please create a path first", MessageType.Info);

            }
        }
        else
        {
            StartEdit = false;
            EditorGUILayout.HelpBox("There should be at least one Path selected in the scene at any given time. If there is no path created yet please create a path first", MessageType.Info);

        }
        GUI.enabled = StartEdit;
        startSpawnPointEditing = GUILayout.Button("Create a new spawn point");
        GUILayout.EndHorizontal();
        if (createCarManager)
        {
            if (GameObject.Find("CarsManager") == null)
            {
                GameObject PedestrianSystem = new GameObject("CarsManager");
                PedestrianSystem.AddComponent<CarsManager>();
            }

        }
        if (startSpawnPointEditing)
        {
            int spawnpoints = Selection.gameObjects[0].transform.childCount;
            if (Selection.gameObjects[0].GetComponent<BezierSplines>().pathMode == "Car")
            {
                GameObject newSpawnPoint = new GameObject("Spawn Point " + (spawnpoints + 1).ToString() + " (Car)");
                Undo.RecordObject(newSpawnPoint, "spawn point added!");
                newSpawnPoint.transform.parent = Selection.gameObjects[0].transform;
                newSpawnPoint.AddComponent<SpawnPoint>();
                newSpawnPoint.GetComponent<SpawnPoint>().path = Selection.gameObjects[0].GetComponent<BezierSplines>();
                newSpawnPoint.GetComponent<SpawnPoint>().duration = Selection.gameObjects[0].GetComponent<BezierSplines>().duration;
                newSpawnPoint.tag = "car spawn";
                EditorUtility.SetDirty(newSpawnPoint);
            }
            else
            {
                EditorGUILayout.HelpBox("The path selected is not created for pedestrians, please make sure the path is made for pedestrians", MessageType.Info);
            }
        }
        GUILayout.BeginHorizontal();
        GUI.enabled = true;
        startCreateCar = GUILayout.Button("Create As Car");
        GUI.enabled = true;
        if(startCreateCar)
        {
           
        }

        
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("The above section is for Kinematic base AI only, further options will be developed soon. To see how to rigg an Auto for the KineAI please see the Guide.pdf", MessageType.Info);
        GUILayout.EndHorizontal();
    }
    void CreateNewprofileList()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        //viewIndex = 1;
        //CarProfiles = CreateCarProfiles.Create();
        //if (CarProfiles)
        //{
        //    CarProfiles.profileList = new List<InventoryItem>();
        //    string relPath = AssetDatabase.GetAssetPath(CarProfiles);
        //    EditorPrefs.SetString("ObjectPath", relPath);
        //}
    }
}
