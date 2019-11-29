using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Manages and loads the Pedestrians in Async
/// </summary>

public class PedestrianManager : MonoBehaviour

{

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
    [Space]
    [Header("internal")]
    public bool loadingDone = false;
    public float assetPopulation = 0;
    public float loadedInstances = 0;

   
    // Use this for initialization
    
    private List<GameObject> Characters;
    private GameObject[] spawnPoints;
    private List<GameObject> instantiatedChar;
    private bool locked = false;
    //Pre-Loads the pedestrians in the background and assorting them to the spawnpoints 
    public IEnumerator InitializeAsync()
    {
        loadingDone = false;
        loadedInstances = 0;
        // wait untill all past instances are destroyed
        while (locked)
        {
            yield return null; 
        }
        // Fetching all destinatios;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("pedestrian spawn");

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

        
        Characters = new List<GameObject>(assets.Pedestrians);
        if (Characters.Count == 0)
        {
            Debug.LogError("No Character Could be found, Please create at least one object as Character");
            yield return null;
        }
        else
        {
            assetPopulation = spawnPoints.Length; 
            if (debugShowInitializationMessages)
                Debug.Log("There are " + Characters.Count.ToString() + " Characters found");
            characterPool = Characters.Count;
            instantiatedChar = new List<GameObject>();
            GameObject runtimeCharParent = new GameObject("Characters");
            while (GameObject.Find("Characters") == null)
            {
                yield return null;
            }
            runtimeCharParent.AddComponent<EyeTrackingTarget>();
            runtimeCharParent.transform.parent = this.transform;
            Shuffle(Characters);
            int characterIndexToBeSpawned = 0;
            int population = spawnPoints.Length;
           
            foreach (GameObject point in spawnPoints)
            {
                GameObject newCharacter = Instantiate<GameObject>(Characters[characterIndexToBeSpawned % characterPool]);
                
                newCharacter.GetComponent<CharacterManager>().objectID =
                    (point.GetComponent<SpawnPoint>().path.name + "_" + point.name);
                newCharacter.GetComponent<CharacterManager>().duration = point.GetComponent<SpawnPoint>().duration;
                newCharacter.GetComponent<CharacterManager>().isLoop = point.GetComponent<SpawnPoint>().isLooping;
                newCharacter.GetComponent<CharacterManager>().path = point.GetComponent<SpawnPoint>().path;
                newCharacter.GetComponent<CharacterManager>().startPecentage =
                    point.GetComponent<SpawnPoint>().percentageGone;
                newCharacter.GetComponent<CharacterManager>().goingForward = true;
                newCharacter.GetComponent<CharacterManager>().lookForward = true;
           
                newCharacter.transform.parent = runtimeCharParent.transform;
                instantiatedChar.Add(newCharacter);
                characterIndexToBeSpawned++;
                loadedInstances++;
                yield return null;
                
            }
        }
        loadingDone = true;
    }
    // Does the same as InitializeAsync but changes the CharacterManager with the CharacterManagerReplay Component
    public IEnumerator InitializeSimulateAsync(Payload dataBase)
    {
        Payload loadedData = dataBase;
        loadingDone = false;
        loadedInstances = 0;
        // wait untill all past instances are destroyed
        while (locked)
        {
            yield return null;
        }
        // Fetching all destinatios;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("pedestrian spawn");

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

        Characters = new List<GameObject>(assets.Pedestrians);
        if (Characters.Count == 0)
        {
            Debug.LogError("No Character Could be found, Please create at least one object as Character");
            yield return null;
        }
        else
        {
            assetPopulation = spawnPoints.Length;
            if (debugShowInitializationMessages)
                Debug.Log("There are " + Characters.Count.ToString() + " Characters found");
            characterPool = Characters.Count;
            instantiatedChar = new List<GameObject>();
            GameObject runtimeCharParent = new GameObject("Characters");
            while (GameObject.Find("Characters") == null)
            {
                yield return null;
            }
            runtimeCharParent.AddComponent<EyeTrackingTarget>();
            runtimeCharParent.transform.parent = this.transform;
            Shuffle(Characters);
            int characterIndexToBeSpawned = 0;
            int population = spawnPoints.Length;
            foreach (GameObject point in spawnPoints)
            {
                GameObject newCharacter = Instantiate<GameObject>(Characters[characterIndexToBeSpawned % characterPool]);
                Destroy(newCharacter.GetComponent<CharacterManager>());
                newCharacter.AddComponent<CharacterManagerReplay>();
                newCharacter.GetComponent<CharacterManagerReplay>().objectID =
                    (point.GetComponent<SpawnPoint>().path.name + "_" + point.name);
                newCharacter.GetComponent<CharacterManagerReplay>().dataPoints =
                    loadedData.Data[(point.GetComponent<SpawnPoint>().path.name + "_" + point.name)];
                
                newCharacter.transform.parent = runtimeCharParent.transform;
                instantiatedChar.Add(newCharacter);
                characterIndexToBeSpawned++;
                loadedInstances++;
                yield return null;

            }
        }
        loadingDone = true;
    }
    // enables the Pedestrian spawns for the visualize mode
    public IEnumerator InitializeVisualiseAsync(Payload dataBase)
    {
        Payload loadedData = dataBase;
        loadingDone = false;
        loadedInstances = 0;
        // wait untill all past instances are destroyed
        while (locked)
        {
            yield return null;
        }
        // Fetching all destinatios;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("pedestrian spawn");

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

        Characters = new List<GameObject>(assets.Pedestrians);
        if (Characters.Count == 0)
        {
            Debug.LogError("No Character Could be found, Please create at least one object as Character");
            yield return null;
        }
        else
        {
            assetPopulation = spawnPoints.Length;
            if (debugShowInitializationMessages)
                Debug.Log("There are " + Characters.Count.ToString() + " Characters found");
            characterPool = Characters.Count;
            instantiatedChar = new List<GameObject>();
            GameObject runtimeCharParent = new GameObject("Characters");
            while (GameObject.Find("Characters") == null)
            {
                yield return null;
            }
            runtimeCharParent.AddComponent<EyeTrackingTarget>();
            runtimeCharParent.transform.parent = this.transform;
            Shuffle(Characters);
            int characterIndexToBeSpawned = 0;
            int population = spawnPoints.Length;
            foreach (GameObject point in spawnPoints)
            {
                GameObject newCharacter = Instantiate<GameObject>(Characters[characterIndexToBeSpawned % characterPool]);
                Destroy(newCharacter.GetComponent<CharacterManager>());
                newCharacter.AddComponent<CharacterManagerVisualise>();
                newCharacter.GetComponent<CharacterManagerVisualise>().objectID =
                    (point.GetComponent<SpawnPoint>().path.name + "_" + point.name);
                newCharacter.GetComponent<CharacterManagerVisualise>().dataPoints =
                    loadedData.Data[(point.GetComponent<SpawnPoint>().path.name + "_" + point.name)];
                newCharacter.layer = WestdriveSettings.trackableLayerMask;
                newCharacter.transform.parent = runtimeCharParent.transform;
                instantiatedChar.Add(newCharacter);
                characterIndexToBeSpawned++;
                loadedInstances++;
                yield return null;

            }
        }
        loadingDone = true;
    }

    private void Awake()
    {
        if (selfInit)
        {
            StartCoroutine(InitializeAsync());
        }
    }
    //Starts the Coroutine depending on the mode
    public void Init(Payload dataBase)
    {
        if (WestdriveSettings.SimulationMode == mode.record)
            StartCoroutine(InitializeAsync());
        else if (WestdriveSettings.SimulationMode == mode.simulate)
            StartCoroutine(InitializeSimulateAsync(dataBase));
        else if (WestdriveSettings.SimulationMode == mode.visualize)
            StartCoroutine(InitializeVisualiseAsync(dataBase));
    }
    //destroys the all characters and stops all coroutines
    public void ResetSystemAsync(Payload dataBase)
    {
        locked = true;
        StopAllCoroutines();
        Transform pedestrianParent = GameObject.Find("Characters").transform;
        int pedestrianPopulation = pedestrianParent.childCount;
        // this part can be improved for better performance
        for (int pedestrianIndex = 0; pedestrianIndex < pedestrianPopulation; pedestrianIndex++)
        {
            dataBase.Data.Add(pedestrianParent.GetChild(pedestrianIndex).gameObject.GetComponent<CharacterManager>().objectID, pedestrianParent.GetChild(pedestrianIndex).gameObject.GetComponent<CharacterManager>().getData());
        }
        Destroy(GameObject.Find("Characters"));
        locked = false;
    }
    //destroys the all characters and stops all coroutines
    public void ResetSystemAsync()
    {
        locked = true;
        StopAllCoroutines();
        Destroy(GameObject.Find("Characters"));
        locked = false;
        //Init();
    }
    //shuffels the List
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
