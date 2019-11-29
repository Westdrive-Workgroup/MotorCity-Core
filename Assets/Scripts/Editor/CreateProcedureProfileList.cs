using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CreateProcedureProfileList
{
    public static ProcedureProfileList CreateProfilesList(string fileName)
    {
        ProcedureProfileList profileList = ScriptableObject.CreateInstance<ProcedureProfileList>();

        AssetDatabase.CreateAsset(profileList, "Assets/Resources/Shared Data/Profile Lists/" +
            fileName + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profileList;
        return profileList;
    }
    [MenuItem("Assets/Create/CityAI/Procedure/Create procedure profiles list")]
    public static ProcedureProfileList CreateProfilesList()
    {
        ProcedureProfileList profileList = ScriptableObject.CreateInstance<ProcedureProfileList>();

        AssetDatabase.CreateAsset(profileList, "Assets/Resources/Shared Data/Profile Lists/new experiments list.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profileList;
        return profileList;
    }
}
