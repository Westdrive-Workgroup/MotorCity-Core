using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateCarProfileList
{
   
    public static CarProfileList CreateProfilesList(string fileName)
    {
        CarProfileList profileList = ScriptableObject.CreateInstance<CarProfileList>();

        AssetDatabase.CreateAsset(profileList, "Assets/Resources/Shared Data/Profile Lists/" +
            fileName + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profileList;
        return profileList;
    }
    [MenuItem("Assets/Create/CityAI/Cars/Create car profiles list")]
    public static CarProfileList CreateProfilesList()
    {
        CarProfileList profileList = ScriptableObject.CreateInstance<CarProfileList>();

        AssetDatabase.CreateAsset(profileList, "Assets/Resources/Shared Data/Profile Lists/new car profiles list.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = profileList;
        return profileList;
    }
}
