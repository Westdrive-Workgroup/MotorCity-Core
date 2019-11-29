using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FixStreetColliders : EditorWindow
{

    [MenuItem("Window/City AI/Helpers/Fix Street Colliders")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FixStreetColliders));
    }
    private void OnGUI()
    {
        if (Selection.gameObjects != null)
        {
            if (GUILayout.Button("Fix Now!"))
            {
                GameObject[] parentObjects = Selection.gameObjects;
                foreach (GameObject parent in parentObjects)
                {
                    List<GameObject> ChildrenObjects = new List<GameObject>();
                    for (int index = 0; index < parent.transform.childCount; index++)
                    {
                        ChildrenObjects.Add(parent.transform.GetChild(index).gameObject);
                    }
                    foreach (GameObject Street in ChildrenObjects)
                    {
                        List<GameObject> MaterialObjects = new List<GameObject>();
                        for (int index = 0; index < Street.transform.childCount; index++)
                        {
                            MaterialObjects.Add(Street.transform.GetChild(index).gameObject);
                        }
                        foreach (GameObject material in MaterialObjects)
                        {
                            MeshCollider[] old = material.GetComponents<MeshCollider>();
                            foreach (MeshCollider mesh in old)
                            {
                                DestroyImmediate(mesh);
                            }
                            material.AddComponent<MeshCollider>();
                        }
                    }
                }
            }
        }
    }
}
