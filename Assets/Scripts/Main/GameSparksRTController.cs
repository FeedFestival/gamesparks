using Assets.Scripts.Main;
using GameSparks.RT;
using UnityEngine;

public class GameSparksRTController : MonoBehaviour
{
    private GameSparksRTUnity _gameSparksRtUnity;
    public GameSparksRTUnity GetRtSession()
    {
        return _gameSparksRtUnity;
    }

    private RtSessionInfo _sessionInfo;
    public RtSessionInfo GetSessionInfo()
    {
        return _sessionInfo;
    }

    private static GameSparksRTController instance = null;
    public static GameSparksRTController Instance()
    {
        if (instance != null)
        {
            return instance; // return the singleton if the instance has been setup
        }
        else
        { // otherwise return an error
            DebugTool.Instance().SetMessage(new DebugMessage("GSM| GameSparksRTController Not Initialized...", DebugMessageType.Error));
        }
        return null;
    }

    void Awake()
    {
        instance = this;
    }
    
    // DELEGATES

    public delegate void OnPlayerConnectedToGame(int peerId);

    public delegate void OnPlayerDisconnected(int peerId);

    public delegate void OnRTReady(bool isReady);

    public delegate void OnPacketReceived(RTPacket packet);

    public void StartNewRtSession(RtSessionInfo info,
        OnPlayerConnectedToGame onPlayerConnectedToGame,
        OnPlayerDisconnected onPlayerDisconnected,
        OnRTReady onRtReady,
        OnPacketReceived onPacketReceived)
    {
        GameSparksController.Instance().GetGameSparksFactory().StartNewRtSession(
            info,
            onPlayerConnectedToGame,
            onPlayerDisconnected,
            onRtReady,
            onPacketReceived,
            ref _sessionInfo,
            ref _gameSparksRtUnity
            );
    }
}