using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Main;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class GameSparksOfflineFactory : MonoBehaviour, IGameSparksFactory
{
    public void WhereAmI()
    {
        Debug.Log("offline");
    }

    public void Init(GameSparksController.GameSparksStatusCallback gameSparksStatusCallback)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("Fake | No GameSparks connection for now...", DebugMessageType.Warning));

        StartCoroutine(GameSparksAvailable((isAvailable) =>
        {
            if (isAvailable)
            {
                DebugTool.Instance().SetMessage(new DebugMessage("Fake | GameSparks Connected...", DebugMessageType.Message));
            }
            else
            {
                DebugTool.Instance().SetMessage(new DebugMessage("Fake | GameSparks Disconnected...", DebugMessageType.Warning));
            }
            gameSparksStatusCallback(isAvailable);
        }));
    }

    private delegate void GameSparksAvailableFunc(bool isAvailable);
    IEnumerator GameSparksAvailable(GameSparksAvailableFunc gameSparksAvailableFunc)
    {
        yield return new WaitForSeconds(2f);

        gameSparksAvailableFunc(true);
    }
    /*------------------------------------------------------------------------------------------------------------------------------------------------------
     * -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void AuthenticateUser(string userName, string password,
        GameSparksController.RegCallback regcallback,
        GameSparksController.AuthCallback authcallback,
        GameSparksController.ErrorCallback errorCallback)
    {

    }


    /*------------------------------------------------------------------------------------------------------------------------------------------------------
     * -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void FindPlayers(GameSparksController.NoMatchCallback noMatchCallback,
        GameSparksController.MatchCallback matchCallback)
    {

    }








    /*------------------------------------------------------------------------------------------------------------------------------------------------------
     * -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * * -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * 
     *                                                                              REALTIME
     *                                                                              
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

    public void StartNewRtSession(RtSessionInfo info,
        GameSparksRTController.OnPlayerConnectedToGame onPlayerConnectedToGame,
        GameSparksRTController.OnPlayerDisconnected onPlayerDisconnected,
        GameSparksRTController.OnRTReady onRtReady,
        GameSparksRTController.OnPacketReceived onPacketReceived,
        ref RtSessionInfo sessionInfo, ref GameSparksRTUnity gameSparksRtUnity)
    {

    }
}
