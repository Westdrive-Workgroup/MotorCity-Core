using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FixMeshColliders : EditorWindow
{

    [MenuItem("Window/City AI/Helpers/Fix Mesh Colliders")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FixMeshColliders));
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
                        if(parent.transform.GetChild(index).GetComponent<MeshCollider>() != null || parent.transform.GetChild(index).GetComponent<BoxCollider>() != null)
                        {
                            if (parent.transform.GetChild(index).GetComponent<Rigidbody>() == null)
                            {
                                if(parent.transform.GetChild(index).GetComponent<MeshCollider>() != null)
                                    DestroyImmediate(parent.transform.GetChild(index).GetComponent<MeshCollider>());
                                if(parent.transform.GetChild(index).GetComponent<BoxCollider>() != null)
                                    DestroyImmediate(parent.transform.GetChild(index).GetComponent<BoxCollider>());
                                parent.transform.GetChild(index).gameObject.AddComponent<MeshCollider>();
                                parent.transform.GetChild(index).GetComponent<MeshCollider>().convex = false;
                            }
                        }
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
                                if(mesh != null)
                                {
                                    DestroyImmediate(mesh);
                                }
                               
                            }
                            BoxCollider[] oldBox = material.GetComponents<BoxCollider>();
                            foreach (BoxCollider mesh in oldBox)
                            {
                                DestroyImmediate(mesh);
                            }
                            if (material.GetComponent<MeshRenderer>() != null)
                            {
                                material.AddComponent<MeshCollider>();
                                material.transform.GetComponent<MeshCollider>().convex = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
