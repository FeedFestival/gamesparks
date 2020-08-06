using System.Collections.Generic;
using Assets.Scripts.Main;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, GameObject> scope;

    private string _previousView;
    private string _nextView;
    
    // GameStarts.
    void Start()
    {
        SceneManager.Instance().Init(this);
        Init();
    }

    private void Init()
    {
        scope = SceneManager.Instance().CanvasController.InitViews(scope);
        
        scope = SceneManager.Instance().LoginController.Init(scope, OnLobbyEnter);

        InitButtonLogic();

        StartApp();
    }
    /*------------------------------------------------------------------------------------------------------------------------------------------------------
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
    
    private void InitButtonLogic()
    {
        scope["DebugConsoleButton"].GetComponent<Button>().onClick.AddListener(
            () =>
            {
                scope["DebugWindow"].SetActive(!scope["DebugWindow"].activeSelf);
            }
            );
    }
    
    private void StartApp()
    {
        scope["MenuView"].SetActive(true);
        scope["MainMenuView"].SetActive(true);
        scope["LoginContainer"].SetActive(true);
        /**/
        scope["DebugWindow"].SetActive(true);
        /**/
        SceneManager.Instance().LoginController.DisableLoginForm(true);
        /**/
        GameSparksController.Instance().Init(GameSparksStatusCallback);
    }

    private void GameSparksStatusCallback(bool isAvailable)
    {
        SceneManager.Instance().LoginController.DisableLoginForm(!isAvailable);     // -> If its available then don't disable it.
    }
    
    private void OnLobbyEnter()
    {
        scope["LoginContainer"].SetActive(false);
        scope["FriendListContainer"].SetActive(true);
        /**/
        
        GameSparksController.Instance().FindPlayers(NoMatchFound, MatchFound);
    }

    private RtSessionInfo _tempRTSessionInfo;

    private void MatchFound(MatchFoundMessage mfMsg, string msg)
    {
        _tempRTSessionInfo = new RtSessionInfo(mfMsg);
        SceneManager.Instance().ChatController.Init(SceneManager.Instance(), _tempRTSessionInfo);
    }

    public void NoMatchFound()
    {
        scope["matchDetails"].GetComponent<Text>().text = "No Match Found...";
    }
}