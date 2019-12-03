using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class LeasErsterManager : MonoBehaviour
{

    [SerializeField] private Camera meineKamera;

    [SerializeField] private int leasZahl;

    [SerializeField] private LeasCarController car;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        car.UpdatePosition();
        //meineKamera.transform.position = meineKamera.transform.position + Vector3.one;
    }
}
