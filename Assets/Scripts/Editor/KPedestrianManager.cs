using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class KPedestrianManagerInspector : EditorWindow
{

    private bool createPedestrainManager = false;
    private bool PedestrianButtonEnabled = false;
    private bool ShowPedButtonHelp = false;
    private bool spawButtonEnabled = false;
    private bool startSpawnPointEditing = false;
    private bool startCreatePath;
    private int pathIndex = 0;
    private bool StartEdit = true;
    private GameObject SpawnPointSystem;
    private bool CreateCharBtn = false;
    //private string prefabPath = "Assets/City AI/Resources/KCharacters/" ;
    [MenuItem("Window/City AI/Kinematic Pedestian Manager")]
    public static void ShowWindow()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        
        // Adding a Tag
        string s = "pedestrian spawn";

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
        EditorWindow.GetWindow(typeof(KPedestrianManagerInspector));
    }

    private void OnSceneGUI(SceneView scnView)
    {
       // HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        
       

    }
    private void OnGUI()
    {
  

        
        //
        if (GameObject.Find("PedestriansManager") == null)
        {
            PedestrianButtonEnabled = true;
            spawButtonEnabled = false;
            ShowPedButtonHelp = false;

        }
        else
        {
            PedestrianButtonEnabled = false;
            ShowPedButtonHelp = true;
        }
        if (GameObject.FindObjectOfType<BezierSplines>() != null)
        {

            startSpawnPointEditing = true;
           
        }
       
        GUI.enabled = PedestrianButtonEnabled;
        createPedestrainManager = GUILayout.Button("Create Pedestrian System");
        GUI.enabled = true;
        if (ShowPedButtonHelp)
            EditorGUILayout.HelpBox("There should be only one instance of Pedestrian manager in the scene at any given time. If you want to create a new Pedestrian System please delete the old one first", MessageType.Info);
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
            GameObject path = new GameObject("Path_ " + (pathIndex + 1).ToString() + " (Pedestrian)");
            Undo.RecordObject(path, "new path added");
            path.transform.parent = pathSystem.transform;
            path.AddComponent<BezierSplines>();
            path.GetComponent<BezierSplines>().pathMode = "Pedestrian";
            EditorUtility.SetDirty(path);



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
        GUI.enabled = true;
        CreateCharBtn = GUILayout.Button("Create As Characters");
        GUI.enabled = true;
        if (createPedestrainManager)
        {
            if (GameObject.Find("PedestriansManager") == null)
            {
                GameObject PedestrianSystem = new GameObject("PedestriansManager");
                PedestrianSystem.AddComponent<PedestrianManager>();
            }

        }
        
        if (startSpawnPointEditing)
        {
            int spawnpoints = Selection.gameObjects[0].transform.childCount;
            if (Selection.gameObjects[0].GetComponent<BezierSplines>().pathMode == "Pedestrian")
            {
                GameObject newSpawnPoint = new GameObject("Spawn Point " + (spawnpoints + 1).ToString() + " (Pedestrian)");
                Undo.RecordObject(newSpawnPoint, "spawn point added!");
                newSpawnPoint.transform.parent = Selection.gameObjects[0].transform;
                newSpawnPoint.AddComponent<SpawnPoint>();
                newSpawnPoint.GetComponent<SpawnPoint>().path = Selection.gameObjects[0].GetComponent<BezierSplines>();
                newSpawnPoint.GetComponent<SpawnPoint>().duration = Selection.gameObjects[0].GetComponent<BezierSplines>().duration;
                newSpawnPoint.tag = "pedestrian spawn";
                EditorUtility.SetDirty(newSpawnPoint);
            }
            else
            {
                EditorGUILayout.HelpBox("The path selected is not created for pedestrians, please make sure the path is made for pedestrians", MessageType.Info);
            }
        }
       
        
        if (CreateCharBtn)
        {
            GameObject[] Characters = Selection.gameObjects;
            //Animator animationController = Resources.Load<Animator>("Animation Controllers/Pedestrain Anim Controller");
            
                if (Characters.Length > 0)
                {
                    int currentObject = 1;
                    int objectCount = Characters.Length + 1;
                    EditorUtility.DisplayProgressBar("Character Building", "Creating and preparing all characters", currentObject / objectCount);
                    foreach (GameObject Character in Characters)
                    {
                        Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Resources/KCharacters/" + Character.name + ".prefab");
                        PrefabUtility.ReplacePrefab(Character, prefab, ReplacePrefabOptions.Default);
                        GameObject tmpAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/KCharacters/" + Character.name + ".prefab");

                        tmpAsset.AddComponent<CharacterManager>();

                        AssetDatabase.SaveAssets();
                        currentObject++;
                    }
                    EditorUtility.ClearProgressBar();
                }
                else
                    EditorGUILayout.HelpBox("Please select at least one object to perform this task", MessageType.Error);
           
        }
    }
}
