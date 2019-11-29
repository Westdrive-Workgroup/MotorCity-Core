using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
///  Creates a green or yellow sphere on the hitpositionpoint
/// </summary>
public class TrackerCombinedRelay : MonoBehaviour
{
    private  SerializableCombinedHitData recordedData { get; set; }

    private Transform visualisationDotsParent;
    private int firstFrame;
    private int lastFrame;
    private int currentFrame;
    private Material baseMaterial;
    void Start()
    {
        GameObject visualisationDots = new GameObject();
        this.visualisationDotsParent = visualisationDots.transform;
        baseMaterial = Resources.Load<Material>("Custom Materials/VisulisationDotsMaterial");
    }

    // creates a green or yellow spehere
    void Update()
    {
        if (WestdriveSettings.isPlaying)
        {
            if (currentFrame <= lastFrame)
            {
 
                transform.position = recordedData.Data[currentFrame][0].cameraPosition;
                transform.rotation = recordedData.Data[currentFrame][0].cameraRotation;
                foreach (HitPositionType hitPoint in recordedData.Data[currentFrame])
                {
                    if (hitPoint.centerHitPostion != Vector3.zero)
                    {
                        GameObject centerHit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        centerHit.transform.position = hitPoint.centerHitPostion;
                        centerHit.transform.lossyScale.Set(0.5f, 0.5f, 0.5f);
                        centerHit.transform.parent = visualisationDotsParent;
                        Renderer renderer = centerHit.GetComponent<Renderer>();
                        renderer.material = new Material(baseMaterial);
                        renderer.material.SetColor("_Color", Color.green);

                    }

                    if (hitPoint.boxHitPostion != Vector3.zero)
                    {
                        GameObject boxHit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        boxHit.transform.position = hitPoint.boxHitPostion;
                        boxHit.transform.lossyScale.Set(0.5f, 0.5f, 0.5f);
                        boxHit.transform.parent = visualisationDotsParent;
                        Renderer renderer = boxHit.GetComponent<Renderer>();
                        renderer.material = new Material(baseMaterial);
                        renderer.material.SetColor("_Color", Color.yellow);
                    }
                }

                TimeGaurd.setCurrentFrame(currentFrame);
                currentFrame++;
            }
        }
    }
    // gives the frame Number to the TimeGuard class
    public void setCurretFrame(int frameNumber)
    {
        this.currentFrame = frameNumber;
        TimeGaurd.setCurrentFrame(currentFrame);
    }
    // saves the recorded data
    public void SetData(SerializableCombinedHitData data)
    {
        recordedData = data;
        if (WestdriveSettings.SimulationMode == mode.visualize)
        {
            firstFrame = recordedData.Data.Keys.First();
            lastFrame = recordedData.Data.Keys.Last();
            currentFrame = firstFrame;
        }
    }
}
