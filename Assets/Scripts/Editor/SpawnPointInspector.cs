using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnPoint))]
public class SpawnPointInspector : Editor
{
    private SpawnPoint point;
    // Use this for initialization
    private void OnScene(SceneView sceneview)
    {
        OnSceneGUI();
    }
    private void OnEnable()
    {
        point = (SpawnPoint) target;
    }
    // Update is called once per frame
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Undo.RecordObject(point, "Change spawn point position");
        //point.percentageGone = EditorGUILayout.FloatField("Percentage Gone:", point.percentageGone);
        point.percentageGone = EditorGUILayout.Slider("Percentage Gone:",point.percentageGone, 0f, 1f);
        point.setPosition(point.path.GetPoint(point.percentageGone));
        EditorUtility.SetDirty(point);
        
    }
    private void OnSceneGUI()
    {
        Handles.color = point.path.curveColor;
        Handles.DrawWireDisc(point.pointPosition, Vector3.up, point.radius);
        if (point.visibleWhenNotSelected)
        {
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate += OnScene;
        }
        if (!point.visibleWhenNotSelected)
        {
            SceneView.onSceneGUIDelegate -= OnScene;
        }
    }
}
