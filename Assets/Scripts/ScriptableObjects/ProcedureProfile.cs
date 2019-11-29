using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the Procedure Profile
/// </summary>
[System.Serializable]
public struct StringListDict
{
    public string key;
    public List<string> value;
}
//[System.Serializable]
//public struct Dict<TKey, TValue>
//{
//    [SerializeField]
//    public List<TKey> keys;
//    [SerializeField]
//    public List<TValue> value;
//}
public class ProcedureProfile : ScriptableObject
{
    public string profileName = "New Procedure Profile Profile";
    public string language = "DE";
    [Space]
    [Header("Main Components")]
    public GameObject mainCamera;
    public GameObject ADVPrefab;
    public List<Object> ADVModules;
    public List<string> ADVPaths;
    public Object Tracker;
    [Space]
    [Header("Controlled Elements Per ADV Path")]
    [SerializeField]
    public List<StringListDict> disabledCarPaths;
    public List<StringListDict> disabledPedestrianPaths;
    [Space]
    [Header("ADV Settings")]
    public float feedbackDelay = 2;
    public bool feedbackOnHalt = false;
    [Space]
    [Header("Radio Talk Audios")]
    public AudioClip radioTalkEnglish;
    public AudioClip radioTalkGerman;
    [Space]
    [Header("Taxi Driver Audios")]
    public AudioClip taxiDriverMonolougeEnglish;
    public AudioClip taxiDriverMonolougeGerman;
    [Space]
    [Header("Consent Text Audios")]
    public AudioClip consetTextEnglish;
    public AudioClip consetTextGerman;
    [Header("Block Settings")]
    public bool randomizeADVPath = true;
    public bool randomizeADVModules = true;
    public bool reuseADVPath = false;
    public bool usetracking = false;
    [Header("End Parameters")]
    [Range(0f, 1f)]
    public float endTriggerOnPath;
    [Range(0f, 1f)]
    public float endTriggerFramePercentage;
    [Space]
    [Header("Post Experiment Setup")]
    public bool useOnlineQuestionaire = false;
    public bool fallbackOnFail = false;
    public bool openQuestionaireAfterExperiment = false;

}
