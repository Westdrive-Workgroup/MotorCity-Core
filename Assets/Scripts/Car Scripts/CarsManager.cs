using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; //Needed for XML functionality
using System.Xml.Serialization; //Needed for XML Functionality
using System.IO;
using System.Xml.Linq; //Needed for XDocument
using System;

public class CarsManager : MonoBehaviour {

    private int characterPool;
    [Space]
    [Header("Asset List")]
    [SerializeField]
    private AssetLists assets;

    [Space] [Header("Init")]
    public bool selfInit = false;
    
    
    [Space]
    [Header("Looping the path")]
    public bool looping = true;
    [Space]
    [Header("Debug")]
    public bool debugShowSpawnPointsStatistics = false;
    public bool debugShowInitializationMessages = false;
    public bool debugShowSettingAttributes = false;
    [Space]
    [Header("internal")]
    public bool loadingDone = false;
    public float assetPopulation = 0;
    public float loadedInstances = 0;
    // Use this for initialization

    private List<GameObject> Cars;
    private GameObject[] spawnPoints;
    private List<GameObject> instantiatedCars;
    private List<Color> poolColors;
    private List<float> poolDensity;
    private List<int> populationPerColor;
    private bool locked = false;
    public void ImportSettings()
    {
        
        XDocument xmlDoc = XDocument.Load("Assets/Resources/Settings/Cars.manifest");
        if (xmlDoc == null)
        {
            Debug.LogWarning("no manifest file found, procceding normaly !");
            return;
        }
        XElement pool = xmlDoc.Element("carpool");
        
        IEnumerable<XElement> elements = pool.Elements();

        foreach (XElement element in elements)
        {
            XElement colorTag = element.Element("color");
            XElement densityTag = element.Element("density");
            Color importedColor = new Color();
            ColorUtility.TryParseHtmlString(colorTag.Value,out importedColor);
            poolColors.Add(importedColor);
            poolDensity.Add(float.Parse(densityTag.Value));
            if (debugShowSettingAttributes)
            {
                Debug.Log("color = " + colorTag.Value);
                Debug.Log("density = " + densityTag.Value);
            }
        }
        
    }
    public IEnumerator InitializeAsync()
    {
        // wait untill all pass instances are destroyed
        loadingDone = false;
        loadedInstances = 0;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("car spawn");

        if (spawnPoints != null)
        {

            if (spawnPoints.Length != 0)
            {
                if (debugShowInitializationMessages)
                    Debug.Log("There are " + spawnPoints.Length + " spawn points registered");
            }
            else
            {
                Debug.LogError("No Spawn Point could be found, please create at least one spawn point");
                yield return null;
            }
        }
        else
        {
            Debug.LogError("No spawn point could be found at all, Please create spawn points first");
            yield return null; 
        }

        Cars = new List<GameObject>(assets.Cars);

        if (Cars.Count == 0)
        {
            Debug.LogError("No Character Could be found, Please create at least one object as Character");
        }
        else
        {
            assetPopulation = spawnPoints.Length;
            if (debugShowInitializationMessages)
                Debug.Log("There are " + Cars.Count.ToString() + " cars found");
            characterPool = Cars.Count;
            instantiatedCars = new List<GameObject>();
            GameObject runtimeCarParent = new GameObject("Cars");
            while (GameObject.Find("Cars") == null)
            {
                yield return null;
            }
            runtimeCarParent.AddComponent<EyeTrackingTarget>();
            runtimeCarParent.transform.parent = this.transform;
            Shuffle(Cars);
            int carIndexToBeSpawned = 0;
            _calulateDensityFromPercentage(spawnPoints.Length);
            int settingIndex = 0;
            int settingSize = populationPerColor.Count;
            foreach (GameObject point in spawnPoints)
            {
                GameObject newCar = Instantiate<GameObject>(Cars[carIndexToBeSpawned % characterPool]);
                
                newCar.GetComponent<CarEngine>().objectID =
                    (point.GetComponent<SpawnPoint>().path.name + "_" + point.name);
                newCar.GetComponent<CarEngine>().duration = point.GetComponent<SpawnPoint>().duration;
                newCar.GetComponent<CarEngine>().isLoop = point.GetComponent<SpawnPoint>().isLooping;
                newCar.GetComponent<CarEngine>().path = point.GetComponent<SpawnPoint>().path;
                newCar.GetComponent<CarEngine>().startPecentage = point.GetComponent<SpawnPoint>().percentageGone;


                if (populationPerColor[settingIndex % settingSize] >= 1)
                {
                    populationPerColor[settingIndex % settingSize]--;

                    newCar.GetComponent<CarEngine>().setPaintColor(poolColors[settingIndex % settingSize]);
                }

                newCar.GetComponent<AudioSource>().clip = newCar.GetComponent<CarEngine>().Profile.engineSound;
                newCar.transform.parent = runtimeCarParent.transform;
                //newCar.SetActive(false);
                instantiatedCars.Add(newCar);
                carIndexToBeSpawned++;
                settingIndex++;
                loadedInstances++;
                yield return null;
            }
        }
        loadingDone = true;
    }
    public IEnumerator InitializeSimulateAsync(Payload dataBase)
    {
        // wait untill all pass instances are destroyed
        Payload loadedData = dataBase;
        loadingDone = false;
        loadedInstances = 0;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("car spawn");
        if (spawnPoints != null)
        {

            if (spawnPoints.Length != 0)
            {
                if (debugShowInitializationMessages)
                    Debug.Log("There are " + spawnPoints.Length + " spawn points registered");
            }
            else
            {
                Debug.LogError("No Spawn Point could be found, please create at least one spawn point");
                yield return null;
            }
        }
        else
        {
            Debug.LogError("No spawn point could be found at all, Please create spawn points first");
            yield return null;
        }
        Cars = new List<GameObject>(assets.Cars);

        if (Cars.Count == 0)
        {
            Debug.LogError("No Character Could be found, Please create at least one object as Character");
        }
        else
        {
            assetPopulation = spawnPoints.Length;
            if (debugShowInitializationMessages)
                Debug.Log("There are " + Cars.Count.ToString() + " cars found");
            characterPool = Cars.Count;
            instantiatedCars = new List<GameObject>();
            GameObject runtimeCarParent = new GameObject("Cars");
            while (GameObject.Find("Cars") == null)
            {
                yield return null;
            }
            runtimeCarParent.AddComponent<EyeTrackingTarget>();
            runtimeCarParent.transform.parent = this.transform;
            Shuffle(Cars);
            int carIndexToBeSpawned = 0;
            _calulateDensityFromPercentage(spawnPoints.Length);
            int settingIndex = 0;
            int settingSize = populationPerColor.Count;
            foreach (GameObject point in spawnPoints)
            {
                GameObject newCar = Instantiate<GameObject>(Cars[carIndexToBeSpawned % characterPool]);
                
                newCar.AddComponent<CarEngineReplay>();
                newCar.GetComponent<CarEngineReplay>().Profile = newCar.GetComponent<CarEngine>().Profile;
                newCar.GetComponent<CarEngineReplay>().wheelFL = newCar.GetComponent<CarEngine>().wheelFL;
                newCar.GetComponent<CarEngineReplay>().wheelFL = newCar.GetComponent<CarEngine>().wheelFL;
                newCar.GetComponent<CarEngineReplay>().wheelFR = newCar.GetComponent<CarEngine>().wheelFR;
                newCar.GetComponent<CarEngineReplay>().wheelRL = newCar.GetComponent<CarEngine>().wheelRL;
                newCar.GetComponent<CarEngineReplay>().wheelRR = newCar.GetComponent<CarEngine>().wheelRR;
                newCar.GetComponent<CarEngineReplay>().caliperWheelFL = newCar.GetComponent<CarEngine>().caliperWheelFL;
                newCar.GetComponent<CarEngineReplay>().caliperWheelFR = newCar.GetComponent<CarEngine>().caliperWheelFR;
                Destroy(newCar.GetComponent<CarEngine>());
                newCar.GetComponent<CarEngineReplay>().objectID = (point.GetComponent<SpawnPoint>().path.name + "_" + point.name);
                newCar.GetComponent<CarEngineReplay>().dataPoints =
                    loadedData.Data[(point.GetComponent<SpawnPoint>().path.name + "_" + point.name)];
                
                if (populationPerColor[settingIndex % settingSize] >= 1)
                    {
                        populationPerColor[settingIndex % settingSize]--;

                        newCar.GetComponent<CarEngine>().setPaintColor(poolColors[settingIndex % settingSize]);
                    }

                    newCar.GetComponent<AudioSource>().clip = newCar.GetComponent<CarEngine>().Profile.engineSound;
                newCar.transform.parent = runtimeCarParent.transform;
                
                instantiatedCars.Add(newCar);
                carIndexToBeSpawned++;
                settingIndex++;
                loadedInstances++;
                yield return null;
            }
        }
        loadingDone = true;
    }
    public IEnumerator InitializeVisualiseAsync(Payload dataBase)
    {
        // wait untill all pass instances are destroyed
        Payload loadedData = dataBase;
        loadingDone = false;
        loadedInstances = 0;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("car spawn");
        if (spawnPoints != null)
        {

            if (spawnPoints.Length != 0)
            {
                if (debugShowInitializationMessages)
                    Debug.Log("There are " + spawnPoints.Length + " spawn points registered");
            }
            else
            {
                Debug.LogError("No Spawn Point could be found, please create at least one spawn point");
                yield return null;
            }
        }
        else
        {
            Debug.LogError("No spawn point could be found at all, Please create spawn points first");
            yield return null;
        }
        Cars = new List<GameObject>(assets.Cars);

        if (Cars.Count == 0)
        {
            Debug.LogError("No Character Could be found, Please create at least one object as Character");
        }
        else
        {
            assetPopulation = spawnPoints.Length;
            if (debugShowInitializationMessages)
                Debug.Log("There are " + Cars.Count.ToString() + " cars found");
            characterPool = Cars.Count;
            instantiatedCars = new List<GameObject>();
            GameObject runtimeCarParent = new GameObject("Cars");
            while (GameObject.Find("Cars") == null)
            {
                yield return null;
            }
            runtimeCarParent.AddComponent<EyeTrackingTarget>();
            runtimeCarParent.transform.parent = this.transform;
            Shuffle(Cars);
            int carIndexToBeSpawned = 0;
            _calulateDensityFromPercentage(spawnPoints.Length);
            int settingIndex = 0;
            int settingSize = populationPerColor.Count;
            foreach (GameObject point in spawnPoints)
            {
                GameObject newCar = Instantiate<GameObject>(Cars[carIndexToBeSpawned % characterPool]);
                
                newCar.AddComponent<CarEngineVisualise>();
                newCar.GetComponent<CarEngineVisualise>().Profile = newCar.GetComponent<CarEngine>().Profile;
                
                Destroy(newCar.GetComponent<CarEngine>());
                newCar.GetComponent<CarEngineVisualise>().objectID = (point.GetComponent<SpawnPoint>().path.name + "_" + point.name);
                newCar.GetComponent<CarEngineVisualise>().dataPoints =
                    loadedData.Data[(point.GetComponent<SpawnPoint>().path.name + "_" + point.name)];
                newCar.layer = WestdriveSettings.trackableLayerMask;
                if (populationPerColor[settingIndex % settingSize] >= 1)
                    {
                        populationPerColor[settingIndex % settingSize]--;

                        newCar.GetComponent<CarEngineVisualise>().setPaintColor(poolColors[settingIndex % settingSize]);
                    }

                    //newCar.GetComponent<AudioSource>().clip = newCar.GetComponent<CarEngine>().Profile.engineSound;
                newCar.transform.parent = runtimeCarParent.transform;
                
                instantiatedCars.Add(newCar);
                carIndexToBeSpawned++;
                settingIndex++;
                loadedInstances++;
                yield return null;
            }
        }
        loadingDone = true;
    }

    public void Init(Payload dataBase)
    {
        if(WestdriveSettings.SimulationMode == mode.record)
            StartCoroutine(InitializeAsync());
        else if(WestdriveSettings.SimulationMode == mode.simulate)
            StartCoroutine(InitializeSimulateAsync(dataBase));
        else if(WestdriveSettings.SimulationMode == mode.visualize)
            StartCoroutine(InitializeVisualiseAsync(dataBase));
    }
    private void _calulateDensityFromPercentage(int population)
    {
        populationPerColor = new List<int>();
        foreach(float density in poolDensity)
        {
            populationPerColor.Add(Mathf.FloorToInt(population * density));
        }
    }
    private void Awake()
    {
        poolColors = new List<Color>();
        poolDensity = new List<float>();
        ImportSettings();
        if (selfInit)
        {
            StartCoroutine(InitializeAsync());
        }
    }

    public void ResetSystemAsync(Payload dataBase)
    {
        locked = true;
        StopAllCoroutines();
        Transform carsParent = GameObject.Find("Cars").transform;
        int carPopulation = carsParent.childCount;
        // this part can be improved for better performance
        for (int carIndex = 0; carIndex < carPopulation; carIndex++)
        {
            dataBase.Data.Add(carsParent.GetChild(carIndex).gameObject.GetComponent<CarEngine>().objectID, carsParent.GetChild(carIndex).gameObject.GetComponent<CarEngine>().getData());
            carsParent.GetChild(carIndex).gameObject.SetActive(false);
        }
        Destroy(carsParent.gameObject);
        locked = false;
    }
    public void ResetSystemAsync()
    {
        
        locked = true;
        StopAllCoroutines();
        
        Destroy(GameObject.Find("Cars"));
        locked = false;

    }
  
    void Shuffle<T>(List<T> array)
    {
        System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
        int p = array.Count;
        for (int n = p - 1; n > 0; n--)
        {
            int r = rnd.Next(0, n);
            T t = array[r];
            array[r] = array[n];
            array[n] = t;
        }
    }
}
