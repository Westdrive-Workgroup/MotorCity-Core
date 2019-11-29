using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// creates a green or yellow sphere on the hitpositionpoint
/// </summary>
public class TrackerReplay : MonoBehaviour
{
    private  SerializableHitData recordedData { get; set; }

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

    // Update is called once per frame
    void Update()
    {
        if (WestdriveSettings.isPlaying)
        {
            if (currentFrame <= lastFrame)
            {
 
                transform.position = recordedData.Data[currentFrame].cameraPosition;
                transform.rotation = recordedData.Data[currentFrame].cameraRotation;
                if (recordedData.Data[currentFrame].centerHitPostion != Vector3.zero)
                {
                    GameObject centerHit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    centerHit.transform.position = recordedData.Data[currentFrame].centerHitPostion;
                    centerHit.transform.lossyScale.Set(0.5f,0.5f,0.5f); 
                    centerHit.transform.parent = visualisationDotsParent;
                    Renderer renderer = centerHit.GetComponent<Renderer> ();
                    renderer.material = new Material(baseMaterial);
                    
                    renderer.material.SetColor("_Color",Color.green);
                    
                }

                if (recordedData.Data[currentFrame].boxHitPostion != Vector3.zero)
                {
                    GameObject boxHit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    boxHit.transform.position = recordedData.Data[currentFrame].boxHitPostion;
                    boxHit.transform.lossyScale.Set(0.5f,0.5f,0.5f); 
                    boxHit.transform.parent = visualisationDotsParent;
                    Renderer renderer = boxHit.GetComponent<Renderer> ();
                    renderer.material = new Material(baseMaterial);
                    renderer.material.SetColor("_Color",Color.yellow);
                }

                TimeGaurd.setCurrentFrame(currentFrame);
                currentFrame++;
            }
        }
    }

    public void setCurretFrame(int frameNumber)
    {
        this.currentFrame = frameNumber;
        TimeGaurd.setCurrentFrame(currentFrame);
    }
    public void SetData(SerializableHitData data)
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
