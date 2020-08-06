using System;
using System.Collections.Generic;
using Assets.Scripts.Main;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    private SceneManager _sceneManager;

    private GameObject ChatPanel;
    private Button sendMessageBttn;
    private InputField messageInput;

    public Button ChatPanelToggle;
    private Button ChatPanelToggleOff;

    public Text ChatPanelToggleText;
    public Text ChatPanelToggleOffText;

    public Text chatLogOutput;
    public int elementsInChatLog = 7;
    private Queue<string> chatLog = new Queue<string>();

    public void Init(SceneManager sceneManager, RtSessionInfo tempRTSessionInfo)
    {
        _sceneManager = sceneManager;

        ChatPanelToggle = _sceneManager.Game.scope["ChatPanelToggle"].GetComponent<Button>();
        ChatPanelToggleOff = _sceneManager.Game.scope["ChatPanelToggleOff"].GetComponent<Button>();

        ChatPanelToggleText = _sceneManager.Game.scope["ChatPanelToggleText"].GetComponent<Text>();
        ChatPanelToggleOffText = _sceneManager.Game.scope["ChatPanelToggleOffText"].GetComponent<Text>();

        ChatPanel = _sceneManager.Game.scope["ChatPanel"];
        sendMessageBttn = _sceneManager.Game.scope["SendChatButton"].GetComponent<Button>();
        messageInput = _sceneManager.Game.scope["ChatInputField"].GetComponent<InputField>();
        chatLogOutput = _sceneManager.Game.scope["ChatContent"].GetComponent<Text>();

        GameSparksRTController.Instance().StartNewRtSession(
            tempRTSessionInfo,
            OnPlayerConnectedToGame,
            OnPlayerDisconnected,
            OnRTReady,
            OnPacketReceived
            );

        ChatPanelToggle.onClick.AddListener(() =>
        {
            ChatPanelToggle.gameObject.SetActive(false);
            ChatPanel.SetActive(true);
        });

        ChatPanelToggleOff.onClick.AddListener(() =>
        {
            ChatPanelToggle.gameObject.SetActive(true);
            ChatPanel.SetActive(false);
        });

        sendMessageBttn.onClick.AddListener(SendMessage);

        chatLogOutput.text = string.Empty; // we want to clear the chat log at the start of the game in case there is any debug text in there
    }

    public void SetupChatPanel()
    {
        ChatPanelToggle.gameObject.SetActive(true);

        string userName = "a user";
        foreach (RtSessionInfo.RTPlayer player in GameSparksRTController.Instance().GetSessionInfo().GetPlayerList())
        {
            if (player.peerId != GameSparksRTController.Instance().GetRtSession().PeerId.Value)
            {
                userName = player.displayName;
            }
        }

        ChatPanelToggleText.text = userName;
        ChatPanelToggleOffText.text = userName;
    }

    private void SendMessage()
    {
        if (messageInput.text != string.Empty)
        { // first check to see if there is any message to send
          // for all RT-data we are sending, we use an instance of the RTData object //
          // this is a disposable object, so we wrap it in this using statement to make sure it is returned to the pool //
            using (RTData data = RTData.Get())
            {
                data.SetString(1, messageInput.text); // we add the message data to the RTPacket at key '1', so we know how to key it when the packet is receieved
                data.SetString(2, DateTime.Now.ToString()); // we are also going to send the time at which the user sent this message

                UpdateChatLog("Me", messageInput.text, DateTime.Now.ToString()); // we will update the chat-log for the current user to display the message they just sent

                messageInput.text = string.Empty; // and we clear the message window
                
                DebugTool.Instance().SetMessage(new DebugMessage("Sending Message to All Players... \n" + messageInput.text, DebugMessageType.Error));

                // for this example we are sending RTData, but there are other methods for sending data we will look at later //
                // the first parameter we use is the op-code. This is used to index the type of data being send, and so we can identify to ourselves which packet this is when it is received //
                // the second parameter is the delivery intent. The intent we are using here is 'reliable', which means it will be send via TCP. This is because we aren't concerned about //
                // speed when it comes to these chat messages, but we very much want to make sure the whole packet is received //
                // the final parameter is the RTData object itself //
                GameSparksRTController.Instance().GetRtSession().SendData(1, GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE, data);
            }
        }
        else
        {
            DebugTool.Instance().SetMessage(new DebugMessage("Not Chat Message To Send...", DebugMessageType.Error));
        }
    }

    public void OnMessageReceived(RTPacket _packet)
    {
        Debug.Log(_packet.Data.GetString(1));

        foreach (RtSessionInfo.RTPlayer player in GameSparksRTController.Instance().GetSessionInfo().GetPlayerList())
        {
            if (player.peerId == _packet.Sender)
            {
                // we want to get the message and time and print those to the local users chat-log //
                UpdateChatLog(player.displayName, _packet.Data.GetString(1), _packet.Data.GetString(2));
            }
        }
    }

    private void UpdateChatLog(string _sender, string _message, string _date)
    {
        // In this example, the message we want to display is formatted so that we can distinguish each part of the message when //
        // it is displayed, all the information is clearly visible //
        chatLog.Enqueue("<b>" + _sender + "</b>\n<color=black>" + _message + "</color>" + "\n<i>" + _date + "</i>");
        if (chatLog.Count > elementsInChatLog)
        { // if we have exceeded the amount of messages in the log, then remove the top message
            chatLog.Dequeue();
        }
        chatLogOutput.text = string.Empty; // we need to clear the log, otherwise we always get the same messages repeating
        foreach (string logEntry in chatLog.ToArray())
        { // go through each chat item and add the entry to the output log
            chatLogOutput.text += logEntry + "\n";
        }
    }

    private void OnPlayerConnectedToGame(int peerId)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("GSM| Player Connected, " + peerId, DebugMessageType.Message));

        SceneManager.Instance().ChatController.SetupChatPanel();
    }

    private void OnPlayerDisconnected(int peerId)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("GSM| Player Disconnected, " + peerId, DebugMessageType.Message));
    }

    private void OnRTReady(bool isReady)
    {
        if (isReady)
        {
            DebugTool.Instance().SetMessage(new DebugMessage("GSM| RT Session Connected...", DebugMessageType.Message));

            SceneManager.Instance().ChatController.SetupChatPanel();
        }
    }

    private void OnPacketReceived(RTPacket packet)
    {
        switch (packet.OpCode)
        {
            // op-code 1 refers to any chat-messages being received by a player //
            // from here, we'll send them to the chat-manager //
            case 1:
                SceneManager.Instance().ChatController.OnMessageReceived(packet); // send the whole packet to the chat-manager
                break;
        }
    }
}