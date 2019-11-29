using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CarEngine))]
public class CarEngineInspector : Editor
{
    
    private CarEngine car;
    private void OnSceneGUI()
    {
        GUIStyle carGUI = new GUIStyle();
        GUIStyle carScene = new GUIStyle();
        car = target as CarEngine;
        Handles.color = Color.white;
        if (car.Profile.debugGUI)
        {
            Handles.BeginGUI();
            carGUI.normal.textColor = Color.white;
            carGUI.fontSize = 15;
            carGUI.fontStyle = FontStyle.Bold;
            GUILayout.BeginArea(new Rect(10,10,3000,500));
            GUILayout.Label("Car on path: " + car.path.name + " with  duration of " + car.duration, carGUI);
            GUILayout.Label("Car Progress " + car.Progress, carGUI);
            GUILayout.Label("initial Increament Unit :" + car.initialIncreament + " - Current Increament Unit :" + car.currentIncreament, carGUI);
            if (car.Avoiding)
            {
                
                if (car.hitDistance <= car.Profile.avoidanceCriticalDistance)
                {
                    carGUI.normal.textColor = Color.red;
                    GUILayout.Label("is Avoiding with Hit Distance: " + car.hitDistance, carGUI);
                }
                else if (car.hitDistance > car.Profile.avoidanceCriticalDistance)
                {
                    carGUI.normal.textColor = Color.yellow;
                    GUILayout.Label("is Avoiding with Hit Distance: " + car.hitDistance, carGUI);
                }
                carGUI.normal.textColor = Color.white;
                GUILayout.Label("Detected Object = " + car.hittedObjectName, carGUI);
            }
            GUILayout.EndArea();

            Handles.EndGUI();
        }
        if(car.Profile.debugScene)
        {
            carScene.normal.textColor = Color.white;
            carScene.fontSize = 10;
            carScene.fontStyle = FontStyle.Bold;
            Handles.Label(new Vector3(car.transform.position.x, car.transform.position.y + 10, car.transform.position.z), "Car on path: " + car.path.name + " with  duration of " + car.duration, carScene);
            Handles.Label(new Vector3(car.transform.position.x, car.transform.position.y + 9, car.transform.position.z), "Car Progress " + car.Progress , carScene);
            Handles.Label(new Vector3(car.transform.position.x, car.transform.position.y + 8, car.transform.position.z), "initial Increament Unit :" + car.initialIncreament + " - Current Increament Unit :" + car.currentIncreament, carScene);
            if(car.Avoiding)
            {
                Handles.color = Color.red;
                Handles.SphereHandleCap(0, new Vector3(car.transform.position.x, car.transform.position.y + 7, car.transform.position.z), Quaternion.identity, 0.5f ,EventType.Repaint);
                Handles.color = Color.white;
                if(car.hitDistance <= car.Profile.avoidanceCriticalDistance)
                {
                    carScene.normal.textColor = Color.red;
                    Handles.Label(new Vector3(car.transform.position.x + 1, car.transform.position.y + 7, car.transform.position.z), "Hit Distance: " + car.hitDistance, carScene);
                }
                else if (car.hitDistance > car.Profile.avoidanceCriticalDistance)
                {
                    carScene.normal.textColor = Color.yellow;
                    Handles.Label(new Vector3(car.transform.position.x + 5, car.transform.position.y + 7, car.transform.position.z + 5), "Hit Distance: " + car.hitDistance, carScene);
                }
                carScene.normal.textColor = Color.white;
            }
        }
    }

    // Update is called once per frame
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //EditorGUI.BeginChangeCheck();
        //debugGUI = EditorGUILayout.Toggle("Debug Info GUI", this.debugGUI);
        //debugScene = EditorGUILayout.Toggle("Debug Info On Scene", this.debugScene);

    }

}