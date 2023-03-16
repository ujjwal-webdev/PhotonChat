using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextSpeech;
using UnityEngine.UI;
using UnityEngine.Android;

public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "en-US";
    [SerializeField]
    Text uiText;

    void Start()
    {
        Setup(LANG_CODE);
        #if UNITY_ANDROID
            SpeechToText.Instance.onPartialResultsCallback = onPartialSpeechResult;
        #endif
        SpeechToText.Instance.onResultCallback = onFinalSpeechResult;
        TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnSpeakStop;

        checkPermission();
    }

    void checkPermission()
    {
        #if UNITY_ANDROID
            if(!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
        #endif
    }

    #region Text to Speech

    public void StartSpeaking(string message)
    {
        TextToSpeech.Instance.StartSpeak(message);
    }

    public void StopSpeaking()
    {
        TextToSpeech.Instance.StopSpeak();
    }

    void OnSpeakStart()
    {
        Debug.Log("Talking Started...");
    }

    void OnSpeakStop()
    {
        Debug.Log("Talking Stopped...");
    }

    #endregion

    #region Speech to Text

    public void StartListening()
    {
        SpeechToText.Instance.StartRecording();
    }

    public void StopListening()
    {
        SpeechToText.Instance.StopRecording();
    }

    void onFinalSpeechResult(string result)
    {
        uiText.text = result;
    }

    void onPartialSpeechResult(string result)
    {
        uiText.text = result;
    }

    #endregion

    void Setup(string code)
    {
        TextToSpeech.Instance.Setting(code,1,1);
        SpeechToText.Instance.Setting(code);
    }
}