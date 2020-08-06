using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Responses;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public delegate void OnLobbyEnter();

    private OnLobbyEnter _onLobbyEnter;

    private InputField _userField;
    private InputField _passField;

    private Button _loginButton;
    private Button Player1LoginButton;
    private Button Player2LoginButton;
    
    public Dictionary<string, GameObject> Init(Dictionary<string, GameObject> scope, OnLobbyEnter onLobbyEnter)
    {
        _onLobbyEnter = onLobbyEnter;

        _userField = scope["UsernameField"].GetComponent<InputField>();     // add MainInputField
        _passField = scope["PasswordField"].GetComponent<InputField>();

        _loginButton = scope["LoginButton"].GetComponent<Button>();

        Player1LoginButton = scope["Player1LoginButton"].GetComponent<Button>();
        Player2LoginButton = scope["Player2LoginButton"].GetComponent<Button>();

        _loginButton.onClick.AddListener(
            () =>
            {
                if (string.IsNullOrEmpty(_userField.text) || string.IsNullOrEmpty(_passField.text))
                    return;

                Login();
            }
            );

        /*------------------------------------------------------------------------------------------------------------------------------------------------------
        TEST 
        --------------------------------------------------------------------------------------------------------------------------------------------------------
        --------------------------------------------------------------------------------------------------------------------------------------------------------
        ------------------------------------------------------------------------------------------------------------------------------------------------------*/
        scope["Player1LoginButton"].GetComponent<Button>().onClick.AddListener(
           () =>
           {
               _userField.text = "Legion";
               _passField.text = "Lordofchaos1";
               scope["Player1LoginButton"].GetComponent<Button>().interactable = false;

               OnPhpAuthentication(true);
           }
           );

        scope["Player2LoginButton"].GetComponent<Button>().onClick.AddListener(
           () =>
           {
               _userField.text = "FeedFestival";
               _passField.text = "Parolae3u";
               scope["Player2LoginButton"].GetComponent<Button>().interactable = false;

               OnPhpAuthentication(true);
           }
           );

        /*
        TEST_END
        --------------------------------------------------------------------------------------------------------------------------------------------------------
        */

        return scope;
    }

    private void Login()
    {
        SceneManager.Instance().PhpController.Login(_userField.text, _passField.text, OnPhpAuthentication);
        DisableLoginForm(true);
    }

    public void OnPhpAuthentication(bool success, string userId = null)
    {
        if (success)
        {
            GameSparksController.Instance()
                .AuthenticateUser(_userField.text, _passField.text, OnRegistration, OnAuthentication, ErrorCallback);

            DebugTool.Instance().SetMessage(new DebugMessage("Credentials are fine.", DebugMessageType.Message));
        }
        else
        {
            DebugTool.Instance().SetMessage(new DebugMessage("Password or username was not good.", DebugMessageType.Message));
            DisableLoginForm(false);
        }
    }

    public void DisableLoginForm(bool val)
    {
        _userField.readOnly = val;
        _passField.readOnly = val;
        _userField.interactable = !val;
        _passField.interactable = !val;
        _loginButton.interactable = !val;
        Player1LoginButton.interactable = !val;
        Player2LoginButton.interactable = !val;
    }

    /*
     CALLBACKS
     --------------------------------------------------------------------------------------------------------------------------------------------------------
     --------------------------------------------------------------------------------------------------------------------------------------------------------
     */
    private void OnAuthentication(AuthenticationResponse _authresp2)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("User Authenticated.", DebugMessageType.Message));

        _onLobbyEnter();
    }

    private void OnRegistration(RegistrationResponse _authresp)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("New User Registered.", DebugMessageType.Message));

        _onLobbyEnter();
    }

    private void ErrorCallback()
    {
        SceneManager.Instance().LoginController.DisableLoginForm(false);
    }
}
