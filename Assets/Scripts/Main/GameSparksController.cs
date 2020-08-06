using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Main;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;

public class GameSparksController : MonoBehaviour
{
    private static GameSparksController _instance = null;
    public static GameSparksController Instance()
    {
        if (_instance != null)
        {
            return _instance; // return the singleton if the instance has been setup
        }
        else
        { // otherwise return an error
            DebugTool.Instance().SetMessage(new DebugMessage("GSM| GameSparksManager Not Initialized...", DebugMessageType.Error));
        }
        return null;
    }

    void Awake()
    {
        _instance = this; // if not, give it a reference to this class...
        DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes

        if (GameSparksOffline)
        {
            gameObject.AddComponent<GameSparksOfflineFactory>();
            _gameSparksFactory = GetComponent<GameSparksOfflineFactory>();
        }
        else
        {
            gameObject.AddComponent<GameSparksOnlineFactory>();
            _gameSparksFactory = GetComponent<GameSparksOnlineFactory>();
        }
    }

    public bool GameSparksOffline;
    public bool PhpOffline;

    private IGameSparksFactory _gameSparksFactory;

    public IGameSparksFactory GetGameSparksFactory()
    {
        return _gameSparksFactory;
    }

    /*------------------------------------------------------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/


    //  DELEGATES



    public delegate void GameSparksStatusCallback(bool isAvailable);

    public delegate void AuthCallback(AuthenticationResponse _authresp2);
    public delegate void RegCallback(RegistrationResponse _authResp);
    public delegate void ErrorCallback();

    public delegate void NoMatchCallback();
    public delegate void MatchCallback(MatchFoundMessage mfMsg, string msg);

    /*------------------------------------------------------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void WhereAmI()
    {
        _gameSparksFactory.WhereAmI();
    }

    public void Init(GameSparksStatusCallback gameSparksStatusCallback)
    {
        _gameSparksFactory.Init(gameSparksStatusCallback);
    }

    public void AuthenticateUser(string userName, string password, RegCallback regcallback, AuthCallback authcallback, ErrorCallback errorCallback)
    {
        _gameSparksFactory.AuthenticateUser(userName, password, regcallback, authcallback, errorCallback);
    }

    public void FindPlayers(NoMatchCallback noMatchCallback, MatchCallback matchCallback)
    {
        _gameSparksFactory.FindPlayers(noMatchCallback, matchCallback);
    }
}