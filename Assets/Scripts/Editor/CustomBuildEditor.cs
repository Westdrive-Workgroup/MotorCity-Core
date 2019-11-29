using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;


public class CustomBuildEditor : EditorWindow
{
    public ProcedureProfile experimentProfiles;
    private int methodIndex = 0;
    private int typeIndex = 0;
    private int currentPickerWindow;
    private List<string> methodsName;
    private Object buildClass = null;
    [MenuItem("Window/City AI/Build/ Build Custom Windows x86-64")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(CustomBuildEditor));
    }

    private delegate void CustomProcess();
    

   
    private void OnEnable()
    {
        methodsName = new List<string>();
        GameObject Experiment = GameObject.Find("Experiment Manager");
        if (Experiment != null)
        {
            experimentProfiles = Experiment.GetComponent<ProcedureController>().experiment;
        }
        else
        {
            Debug.LogError("No Expeiment Manager find, please set the procedure profile manually to continue.");
        }
    }

    private void OnGUI()
    {
        /*if (experimentProfiles != null)
        {
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            
            buildClass = (Object)EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Select Custom Class", buildClass, typeof(Object));
            System.Type[] types = System.Reflection.Assembly.GetCallingAssembly().GetTypes();
            System.Type[] possible = (from System.Type type in types where type.IsSubclassOf(typeof(GenericBuild)) select type).ToArray();
            GUILayout.EndHorizontal();
            MethodInfo[] allMethods;
            if (buildClass != null)
            {
                
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                allMethods = buildClass.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (MethodInfo info in allMethods)
                { 
                    
                    methodsName.Add(info.Name);
                }
                
                methodIndex = EditorGUI.Popup(
                    new Rect(3, 25, position.width - 6, 20),
                    "Select Build Method:",
                    methodIndex,
                    methodsName.ToArray());

                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            List<string> possibleTypes = new List<string>();
            foreach (System.Type type in possible)
            {
                possibleTypes.Add(type.FullName);
            }
            typeIndex = EditorGUI.Popup(
                new Rect(3, 45, position.width - 6, 20),
                "Select Type:",
                typeIndex,
                possibleTypes.ToArray());
            GUILayout.EndHorizontal();
            
            
            
        }
        */


        
    }

    private void BuildProject(CustomProcess buildProcess)
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/Scenes/Main Menu.unity", "Assets/Scenes/Stadt.unity" };

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/Westdrive.exe", BuildTarget.StandaloneWindows, BuildOptions.ShowBuiltPlayer);

        buildProcess();
        

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Westdrive.exe";
        proc.Start();
    }
}
