using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTool : MonoBehaviour
{
    public bool showInUnityConsole;

    private static DebugTool instance = null;
    public static DebugTool Instance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            Debug.LogError("DebugTool.cs Not Initialized...");
        }
        return null;
    }

    void Awake()
    {
        instance = this;
    }

    private string _consoleMessages;
    public string ConsoleMessages
    {
        get
        {
            return _consoleMessages;
        }
        protected set { _consoleMessages = value; }
    }
    private List<DebugMessage> _messages;
    public List<DebugMessage> Messages()
    {
        if (_messages == null)
            _messages = new List<DebugMessage>();
        return _messages;
    }
    public void SetMessage(DebugMessage debugMessage)
    {
        if (_messages == null)
            _messages = new List<DebugMessage>();
        _messages.Add(debugMessage);
        /**/
        var date = DateTime.Now;
        _consoleMessages += Environment.NewLine + " [" + date.ToString("HH:mm:ss") + "] " + debugMessage.Message;

        SceneManager.Instance().Game.scope["DebugContent"].GetComponent<Text>().text = _consoleMessages;

        if (showInUnityConsole)
            Debug.Log(debugMessage.Message);
    }
}

public enum DebugMessageType
{
    Message, Error, Warning
}

public class DebugMessage
{
    public string Message { get; set; }
    public DebugMessageType MType { get; set; }

    public DebugMessage(string message, DebugMessageType mType)
    {
        this.Message = message;
        this.MType = mType;
    }
}