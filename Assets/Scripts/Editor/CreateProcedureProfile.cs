using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateProcedureProfile 
{
    
    public static ProcedureProfile CreateProfile(string fileName)
    {
        ProcedureProfile profile = ScriptableObject.CreateInstance<ProcedureProfile>();

        AssetDatabase.CreateAsset(profile, "Assets/Resources/Shared Data/Procedure Profiles/" +
           fileName + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profile;
        return profile;
    }
    [MenuItem("Assets/Create/CityAI/Procedure/Create procedure profile")]
    public static ProcedureProfile CreateProfile()
    {
        ProcedureProfile profile = ScriptableObject.CreateInstance<ProcedureProfile>();

        AssetDatabase.CreateAsset(profile, "Assets/Resources/Shared Data/Procedure Profiles/new experiment.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profile;
        return profile;
    }
}
