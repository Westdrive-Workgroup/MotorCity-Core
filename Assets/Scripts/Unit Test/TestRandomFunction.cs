using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Tests if the 3 different Conditions are used uniformly over 10000trails
/// </summary>

public class TestRandomFunction : MonoBehaviour
{
    private List<string> ADVPaths;
    private List<string> ADVModules;
    public int iterations = 10000;
    private Dictionary<string, int> ADVPathsStats; 
    private Dictionary<string, int> ADVModulesStats;
    private int iterationIndex = 0;
    void Awake()
    {
        ADVPaths = new List<string>();
        ADVModules = new List<string>();
        ADVPathsStats = new Dictionary<string, int>();
        ADVModulesStats = new Dictionary<string, int>();
        ADVPaths.Add("MSW");
        ADVModules.Add("AVAS");
        ADVModules.Add("RadioTalk");
        ADVModules.Add("Taxi Driver");
        ADVModulesStats.Add("AVAS",0);
        ADVModulesStats.Add("RadioTalk",0);
        ADVModulesStats.Add("Taxi Driver",0);
    }
    void Update()
    {
        if (iterationIndex <= iterations)
        {
            chooseNextBlock();
            Debug.Log("proccessed = " + Mathf.FloorToInt((iterationIndex * 100)/iterations) + "%");
            iterationIndex++;
        }
        else
        {
            Debug.Log("ADV Module: AVAS = " + ADVModulesStats["AVAS"] + " times in " + iterations);
            Debug.Log("ADV Module: RadioTalk = " + ADVModulesStats["RadioTalk"] + " times in " + iterations);
            Debug.Log("ADV Module: Taxi Driver = " + ADVModulesStats["Taxi Driver"] + " times in " + iterations);
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
    private void chooseNextBlock()
    {
        var nextModuleIndex = Random.Range(0, 10000);
        int nextIndex = 0;
        if (true)
        {
            Shuffle<string>(ref ADVPaths);

            nextIndex = Random.Range(0, 10000);
            if (ADVPaths.Count != 0)
                nextIndex %= ADVPaths.Count;
            else
                nextIndex = 0;
        }
        else
        {
            nextIndex = 0;
        }

        if (true)
        {
            Shuffle<string>(ref ADVModules);
            if (ADVModules.Count != 0)
                nextModuleIndex %= ADVModules.Count;
            else
                nextModuleIndex = 0;

        }
        else
        {
            nextModuleIndex = nextIndex;
        }
        Statistics(nextIndex, nextModuleIndex);
    }

    void Statistics(int nextIndex,int nextModuleIndex)
    {
        
        if(ADVModulesStats.ContainsKey(ADVModules[nextModuleIndex]))
            ADVModulesStats[ADVModules[nextModuleIndex]]++;
        else
        {
            ADVModulesStats.Add(ADVModules[nextModuleIndex], 0 );
            ADVModulesStats[ADVModules[nextModuleIndex]]++;
        }

    }
    
    void Shuffle<T>(ref List<T> array)
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
