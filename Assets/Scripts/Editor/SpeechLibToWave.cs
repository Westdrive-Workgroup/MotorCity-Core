using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEditor;
using SpeechLib;
public class SpeechLibToWave : EditorWindow
{
    [Header("Internal Components")]
    private string speechText = "";
    private string language = "";
    
    [Header("TTS based Modules")]
    private EventGate source;
    private EventHandler feedBackSource;
    private AVASEvent avasEventSource;
    
    [Header("SpVoice Settings")]
    private int GermanLanguageIndex = 0;
    private int EnglishLanguageIndex = 0;
    private SpObjectTokenCategory tokenCat;
    private ISpeechObjectTokens tokens;
    private SpVoice voice;
    private SpeechStreamFileMode speechFileMode;

    [MenuItem("Window/City AI/Helpers/Convert SpeechLib to Wave")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpeechLibToWave),true,"Convert SpeechLib to Wave");
        
    }

    private void OnGUI()
    {
        initialize();
        GUIStyle header = new GUIStyle();
        header.fontStyle = FontStyle.Bold;
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Convert Using Direct Input",header); 
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Text to be converted");
        speechText = EditorGUILayout.TextArea(speechText,GUILayout.MinHeight(50));
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Convert Direct"))
        {
            convertAndSaveDialog(speechText);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Convert Using Event Gates",header);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("EventGate Object");
        EditorGUILayout.HelpBox("Select a game object with EventGate attached to it", MessageType.Info);
        source = (EventGate)EditorGUILayout.ObjectField(source, typeof(EventGate), true); 
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        if (source != null)
        {
            
            if (GUILayout.Button("Convert From EventGate Object"))
            {
                convertAndSaveDialog(source);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("EventHandler Object");
        EditorGUILayout.HelpBox("Select a game object with EventHandler attached to it", MessageType.Info);
        feedBackSource = (EventHandler)EditorGUILayout.ObjectField(feedBackSource, typeof(EventHandler), true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        if (feedBackSource != null)
        {
            
            if (GUILayout.Button("Convert From EventHandler Object"))
            {
                convertAndSaveDialog(feedBackSource);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("AVASEvent Object");
        EditorGUILayout.HelpBox("Select a game object with AVASEvent attached to it", MessageType.Info);
        avasEventSource = (AVASEvent)EditorGUILayout.ObjectField(avasEventSource, typeof(AVASEvent), true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginHorizontal();
        if (avasEventSource != null)
        {
            
            if (GUILayout.Button("Convert From AVASEvent Object"))
            {
                convertAndSaveDialog(avasEventSource);
            }
        }
        EditorGUILayout.EndHorizontal();

    }

    private void initialize()
    {
        language = "DE";
        tokenCat = new SpObjectTokenCategory();
        tokenCat.SetId(SpeechLib.SpeechStringConstants.SpeechCategoryVoices, false);
        tokens = tokenCat.EnumerateTokens(null, null);
        int n = 0;
        int languageIndex = 0;
        foreach (SpObjectToken item in tokens)
        {   if (language == "DE")
            {
                if (item.GetDescription(0).Contains("Hedda") )
                {
                    languageIndex = n;
                    GermanLanguageIndex = n;
                }
                
            }
            if (language == "ENG")
            {
                if (item.GetDescription(0).Contains("Zira"))
                {
                    languageIndex = n;
                    EnglishLanguageIndex = n;
                }
                
            }
            
            n++;
        }
        
        voice = new SpVoice();
        voice.Voice = tokens.Item(languageIndex);
        speechFileMode = SpeechStreamFileMode.SSFMCreateForWrite;
    }

    private void convertAndSaveDialog(EventGate speechObject)
    {
        string fileName = "";
        if (speechObject.gameObject.GetComponent<EventHandler>() != null)
        {
            fileName = speechObject.gameObject.name;
        }
        else
        {
            fileName = speechObject.transform.parent.name;
        }
        var pathEnglish = EditorUtility.SaveFilePanel(
            "Save english speech as WAV",
            Application.dataPath,
            fileName + "-English.wav",
            "wav");
        SpFileStream SpFileStreamEnglish = new SpFileStream();
        SpFileStreamEnglish.Open(pathEnglish, speechFileMode, false);
        voice.AudioOutputStream = SpFileStreamEnglish;
        voice.Voice = tokens.Item(EnglishLanguageIndex);
        voice.Speak(speechObject.AVASEnglishText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(Timeout.Infinite);//Using System.Threading;
        SpFileStreamEnglish.Close();
        AudioClip speechEnglishAudio = (AudioClip) Resources.Load(pathEnglish);
        speechObject.AVASEnglishAudio = speechEnglishAudio;
        ShowNotification(new GUIContent("Conversion of English Text is done!"),2);

        var pathGerman = EditorUtility.SaveFilePanel(
            "Save english speech as WAV",
            Application.dataPath,
            fileName  + "-German.wav",
            "wav");
        SpFileStream SpFileStreamGerman = new SpFileStream();
        SpFileStreamGerman.Open(pathGerman, speechFileMode, false);
        voice.AudioOutputStream = SpFileStreamGerman;
        voice.Voice = tokens.Item(GermanLanguageIndex);
        voice.Speak(speechObject.AVASGermanText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(Timeout.Infinite);//Using System.Threading;
        SpFileStreamGerman.Close();
        AudioClip speechGermanAudio = (AudioClip) Resources.Load(pathGerman);
        speechObject.AVASEnglishAudio = speechGermanAudio;
        ShowNotification(new GUIContent("Conversion of German Text is done!"),2);
    }
    
    private void convertAndSaveDialog(string speechToConvert)
    {
        var path = EditorUtility.SaveFilePanel(
            "Save speech as WAV",
            "",
            "converted speech" + ".wav",
            "wav");
        SpFileStream SpFileStream = new SpFileStream();
        SpFileStream.Open(path, speechFileMode, false);
        voice.AudioOutputStream = SpFileStream;
        voice.Speak(speechToConvert, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(50000);//Using System.Threading;
        ShowNotification(new GUIContent("Conversion is done! \n you can find your file at: " +  path));
        
        SpFileStream.Close();
    }
    
    private void convertAndSaveDialog(EventHandler speechObject)
    {
        string fileName = speechObject.gameObject.name;
        
        var pathEnglish = EditorUtility.SaveFilePanel(
            "Save english speech as WAV",
            Application.dataPath,
            fileName + "-Feedback-English.wav",
            "wav");
        SpFileStream SpFileStreamEnglish = new SpFileStream();
        SpFileStreamEnglish.Open(pathEnglish, speechFileMode, false);
        voice.AudioOutputStream = SpFileStreamEnglish;
        voice.Voice = tokens.Item(EnglishLanguageIndex);
        voice.Speak(speechObject.AVASEnglishFeedbackText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(Timeout.Infinite);//Using System.Threading;
        SpFileStreamEnglish.Close();
        AudioClip speechEnglishAudio = (AudioClip) Resources.Load(pathEnglish);
        speechObject.AVASEnglishFeedbackAudio = speechEnglishAudio;
        ShowNotification(new GUIContent("Conversion of English Text is done!"),2);

        var pathGerman = EditorUtility.SaveFilePanel(
            "Save english speech as WAV",
            Application.dataPath,
            fileName  + "-Feedback-German.wav",
            "wav");
        SpFileStream SpFileStreamGerman = new SpFileStream();
        SpFileStreamGerman.Open(pathGerman, speechFileMode, false);
        voice.AudioOutputStream = SpFileStreamGerman;
        voice.Voice = tokens.Item(GermanLanguageIndex);
        voice.Speak(speechObject.AVASGermanFeedbackText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(Timeout.Infinite);//Using System.Threading;
        SpFileStreamGerman.Close();
        AudioClip speechGermanAudio = (AudioClip) Resources.Load(pathGerman);
        speechObject.AVASGermanFeedbackAudio = speechGermanAudio;
        ShowNotification(new GUIContent("Conversion of German Text is done!"),2);
    }
    private void convertAndSaveDialog(AVASEvent speechObject)
    {
        string fileName = speechObject.gameObject.name;
        
        var pathEnglish = EditorUtility.SaveFilePanel(
            "Save english speech as WAV",
            Application.dataPath,
            fileName + "-AVASEvent-English.wav",
            "wav");
        SpFileStream SpFileStreamEnglish = new SpFileStream();
        SpFileStreamEnglish.Open(pathEnglish, speechFileMode, false);
        voice.AudioOutputStream = SpFileStreamEnglish;
        voice.Voice = tokens.Item(EnglishLanguageIndex);
        voice.Speak(speechObject.AVASEnglishText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(Timeout.Infinite);//Using System.Threading;
        SpFileStreamEnglish.Close();
        AudioClip speechEnglishAudio = (AudioClip) Resources.Load(pathEnglish);
        speechObject.AVASEnglishAudio = speechEnglishAudio;
        ShowNotification(new GUIContent("Conversion of English Text is done!"),2);

        var pathGerman = EditorUtility.SaveFilePanel(
            "Save english speech as WAV",
            Application.dataPath,
            fileName  + "-AVASEvent-German.wav",
            "wav");
        SpFileStream SpFileStreamGerman = new SpFileStream();
        SpFileStreamGerman.Open(pathGerman, speechFileMode, false);
        voice.AudioOutputStream = SpFileStreamGerman;
        voice.Voice = tokens.Item(GermanLanguageIndex);
        voice.Speak(speechObject.AVASGermanText, SpeechVoiceSpeakFlags.SVSFlagsAsync);
        voice.WaitUntilDone(Timeout.Infinite);//Using System.Threading;
        SpFileStreamGerman.Close();
        AudioClip speechGermanAudio = (AudioClip) Resources.Load(pathGerman);
        speechObject.AVASGermanAudio = speechGermanAudio;
        ShowNotification(new GUIContent("Conversion of German Text is done!"),2);
    }
}
