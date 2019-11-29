using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class CreateCarProfile 
{
   
    public static CarProfile CreateProfile(string fileName)
    {
        CarProfile profile = ScriptableObject.CreateInstance<CarProfile>();

        AssetDatabase.CreateAsset(profile, "Assets/Resources/Shared Data/Car Profiles/" +
           fileName + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profile;
        return profile;
    }
    [MenuItem("Assets/Create/CityAI/Cars/Create car profile")]
    public static CarProfile CreateProfile()
    {
        CarProfile profile = ScriptableObject.CreateInstance<CarProfile>();

        AssetDatabase.CreateAsset(profile, "Assets/Resources/Shared Data/Car Profiles/new car profile.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profile;
        return profile;
    }
}
