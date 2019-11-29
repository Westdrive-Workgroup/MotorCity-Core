using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarProfileEditor : EditorWindow
{
    public CarProfileList CarProfiles;
    private int viewIndex = 1;
    private string newProfileName = "New profile name";
    [MenuItem("Window/City AI/Cars/ Car Profile Manager")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(CarProfileEditor));
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("ObjectPath"))
        {
            string objectPath = EditorPrefs.GetString("ObjectPath");
            CarProfiles = AssetDatabase.LoadAssetAtPath(objectPath, typeof(CarProfileList)) as CarProfileList;
        }

    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal ();
        GUILayout.Label ("Car Profile Editor", EditorStyles.boldLabel);
        if (CarProfiles != null) {
            if (GUILayout.Button("Show Profile List")) 
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = CarProfiles;
            }
        }
        if (GUILayout.Button("Open Profile List")) 
        {
                OpenProfileList();
        }
        if (GUILayout.Button("New Profile List")) 
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = CarProfiles;
        }
        GUILayout.EndHorizontal ();
        
        if (CarProfiles == null) 
        {
            GUILayout.BeginHorizontal ();
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Profile List", GUILayout.ExpandWidth(false))) 
            {
                CreateNewProfileList();
            }
            if (GUILayout.Button("Open Existing Profile List", GUILayout.ExpandWidth(false))) 
            {
                OpenProfileList();
            }
            GUILayout.EndHorizontal ();
        }
            
            GUILayout.Space(20);
            
        if (CarProfiles != null) 
        {
            GUILayout.BeginHorizontal ();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false))) 
            {
                if (viewIndex > 1)
                    viewIndex --;
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false))) 
            {
                if (viewIndex < CarProfiles.profileList.Count) 
                {
                    viewIndex ++;
                }
            }
            
            GUILayout.Space(60);
            newProfileName = EditorGUILayout.TextField("New profile name", newProfileName );
            if (GUILayout.Button("Add New Profile", GUILayout.ExpandWidth(false))) 
            {
                AddProfile(newProfileName);
            }
            if (GUILayout.Button("Delete Profile", GUILayout.ExpandWidth(false))) 
            {
                DeleteProfile(viewIndex - 1);
            }
            
            GUILayout.EndHorizontal ();
            if (CarProfiles.profileList == null)
                Debug.Log("Profile List Empty");
            if (CarProfiles.profileList.Count > 0) 
            {
                GUILayout.BeginHorizontal();
                viewIndex = Mathf.Clamp(EditorGUILayout.IntField("Current Profile", viewIndex, GUILayout.ExpandWidth(false)), 1, CarProfiles.profileList.Count);
                Mathf.Clamp (viewIndex, 1, CarProfiles.profileList.Count);
                EditorGUILayout.LabelField("of   " + CarProfiles.profileList.Count.ToString() + "  profiles", "", GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
                CarProfiles.profileList[viewIndex - 1].profileName = EditorGUILayout.TextField("Profile Name", CarProfiles.profileList[viewIndex - 1].profileName as string);
                CarProfiles.profileList[viewIndex - 1].tag = EditorGUILayout.TextField("Car Tag", CarProfiles.profileList[viewIndex - 1].tag as string);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Debug Options");
                GUILayout.BeginVertical();
                CarProfiles.profileList[viewIndex - 1].debugAlwaysShowCast = (bool)EditorGUILayout.Toggle("Always show on cast", CarProfiles.profileList[viewIndex - 1].debugAlwaysShowCast, GUILayout.ExpandWidth(false));
                CarProfiles.profileList[viewIndex - 1].debugShowCastOnHit = (bool)EditorGUILayout.Toggle("Always show on hit", CarProfiles.profileList[viewIndex - 1].debugShowCastOnHit, GUILayout.ExpandWidth(false));
                CarProfiles.profileList[viewIndex - 1].debugGUI = (bool)EditorGUILayout.Toggle("GUI", CarProfiles.profileList[viewIndex - 1].debugGUI, GUILayout.ExpandWidth(false));
                CarProfiles.profileList[viewIndex - 1].debugScene = (bool)EditorGUILayout.Toggle("Scene", CarProfiles.profileList[viewIndex - 1].debugScene, GUILayout.ExpandWidth(false));
                CarProfiles.profileList[viewIndex - 1].debugHitDistance = EditorGUILayout.FloatField("Hit distance", CarProfiles.profileList[viewIndex - 1].debugHitDistance);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Setup Parameters");
                GUILayout.BeginVertical();
                CarProfiles.profileList[viewIndex - 1].nonPhysicalSimulation = (bool)EditorGUILayout.Toggle("Uses Kinematic", CarProfiles.profileList[viewIndex - 1].nonPhysicalSimulation, GUILayout.ExpandWidth(false));
                CarProfiles.profileList[viewIndex - 1].useRuntimeColor = (bool)EditorGUILayout.Toggle("Uses Runtime Color", CarProfiles.profileList[viewIndex - 1].useRuntimeColor, GUILayout.ExpandWidth(false));
                CarProfiles.profileList[viewIndex - 1].paint = EditorGUILayout.ColorField("Paint Color", CarProfiles.profileList[viewIndex - 1].paint);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Sensor Parameters");
                GUILayout.BeginVertical();
                CarProfiles.profileList[viewIndex - 1].avoidingLayerName = EditorGUILayout.TextField("Avoiding Layer Name ", CarProfiles.profileList[viewIndex - 1].avoidingLayerName as string);
                CarProfiles.profileList[viewIndex - 1].avoidanceSenrosLength = EditorGUILayout.FloatField("Sensor Length", CarProfiles.profileList[viewIndex - 1].avoidanceSenrosLength);
                CarProfiles.profileList[viewIndex - 1].avoidanceCriticalDistance = EditorGUILayout.FloatField("Critical crash distance", CarProfiles.profileList[viewIndex - 1].avoidanceCriticalDistance);
                CarProfiles.profileList[viewIndex - 1].turningAvoidanceTreshold = EditorGUILayout.FloatField("Hit angle treshold to ignore", CarProfiles.profileList[viewIndex - 1].turningAvoidanceTreshold);

                GUILayout.EndVertical();

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Engine Sound");
                GUILayout.BeginVertical();
                CarProfiles.profileList[viewIndex - 1].engineSound = (AudioClip)EditorGUILayout.ObjectField("Audio Clip",CarProfiles.profileList[viewIndex - 1].engineSound, typeof(AudioClip),true);
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("This Car List List is Empty.");
            }
        }
            if (GUI.changed) 
        {
            EditorUtility.SetDirty(CarProfiles);
        }
        
    }
    void CreateNewProfileList()
    {
        // There is no overwrite protection here!
        // There is No "Are you sure you want to overwrite your existing object?" if it exists.
        // This should probably get a string from the user to create a new name and pass it ...
        viewIndex = 1;
        CarProfiles = CreateCarProfileList.CreateProfilesList("CarProfileList");
        if (CarProfiles)
        {
            CarProfiles.profileList = new List<CarProfile>();
            string relPath = AssetDatabase.GetAssetPath(CarProfiles);
            EditorPrefs.SetString("ObjectPath", relPath);
        }
    }

    void OpenProfileList()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Inventory Item List", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            CarProfiles = AssetDatabase.LoadAssetAtPath(relPath, typeof(CarProfileList)) as CarProfileList;
            if (CarProfiles.profileList == null)
                CarProfiles.profileList = new List<CarProfile>();
            if (CarProfiles)
            {
                EditorPrefs.SetString("ObjectPath", relPath);
            }
        }
    }

    void AddProfile(string name)
    {

        CarProfile newProfile = CreateCarProfile.CreateProfile(name);
        
        if (newProfile)
        {
            newProfile.profileName = name;
            CarProfiles.profileList.Add(newProfile);

            viewIndex = CarProfiles.profileList.Count;          
        }
        
    }

    void DeleteProfile(int index)
    {
        CarProfiles.profileList.RemoveAt(index);
    }

}

