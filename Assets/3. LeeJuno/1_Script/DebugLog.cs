using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugLog : MonoBehaviour
{
    [SerializeField]
    private TMP_Text consoleText; // UI Text (혹은 TMP)


    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        consoleText.text += logString + "\n";
    }
}