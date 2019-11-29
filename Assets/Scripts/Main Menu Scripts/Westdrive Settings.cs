using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Declares the Settings for Westdrive
/// </summary>
//declares the relation between simulate and record
public enum mode
{
    simulate, record , visualize
}
//creates a relation between ready, working and pending
public enum IOState
{
    ready, working , pending
}
public enum visualisationMode
{
    process,hit,combinedHit,unselcted
}

public enum reloadBehaviour
{
    close, goToLobby
}
public static class WestdriveSettings {

   
    public static string language;
    public static AudioClip radioTalkEnglish;
    public static AudioClip radioTalkGerman;
    public static AudioClip taxiDriverMonolougeEnglish;
    public static AudioClip taxiDriverMonolougeGerman;
    // Visualisation parameters
    public static int frameCorrection;
    public static bool useInterpolate;
    public static bool isPlaying;
    public static bool firstRun;
    public static SerializableHitData hitData;
    
    private static  mode simulationMode;
    public static visualisationMode visualisationMode;
    public static List<AnalyzableData> processedData;
    public static Payload EventData;
    public static int trackableLayerMask;
    // sets a simulationMode value
    public static mode SimulationMode;
    public static reloadBehaviour experimentReload;
    
    // sets a IOState value
    private static IOState checkIO;
    public static IOState CheckIO
    {
        get { return checkIO; }
        set { checkIO = value; }
    }
    //sets the progress value
    private static float progress;
    public static float Progress
    {
        get
        {
            return progress;
        }
        set
        {
            if(value >= 0f && value <= 1f)
            {
                progress = value;
            }
        }
    }
    //Sets the randomCodeMax value
    private static float randomCodeMax = 100;
    public static float RandomCodeMax
    {
        get
        {
            return randomCodeMax;
        }
        set
        {
            if(value >= 1)
                randomCodeMax = value;
        }
    }
    //creates the ParticipantCode and returns it
    private static int participantCode = 0;
    public static int ParticipantCode
    {
        get
        {
            if (participantCode == 0)
            {
                participantCode = Mathf.FloorToInt(Random.Range(1f, 100f));
                return participantCode;
            }
            else return participantCode;
        }
    }
    // sets and returns the Participant UID
    private static string participantUID = "";
    public static string ParticipantUID
    {
        get
        {
            if (participantUID == "")
            {
                participantUID = Guid.NewGuid().ToString().Replace("-","");
                
                return participantUID;
            }
            else return participantUID;
        }
    }

    public static void ResetUID()
    {
        participantUID = Guid.NewGuid().ToString().Replace("-","");
    }
    //Resets the participantCode
    public static void ResetParticipantCode()
    {
        participantCode = 0;
    }

    
}
