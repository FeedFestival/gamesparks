using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Configuration;

public class PhpController : MonoBehaviour
{
    private const string LocalUrl = "http://localhost:8080/GameCrib.Service";
    private const string OnlineUrl = "http://gamescrypt.com/GameCrib.Service";

    private string _loginUrl;

    private readonly Dictionary<string, string> _postHeader = new Dictionary<string, string> { { "Content-Type", "text/json" } };

    public void Init(bool offline)
    {
        if (offline)
        {
            _loginUrl = LocalUrl + "/SessionService/AttemptLogin.php";
        }
        else
        {
            _loginUrl = OnlineUrl + "/SessionService/AttemptLogin.php";
        }
    }

    // The login callback
    public delegate void OnPhpAuthentication(bool success, string userId = null);
    private OnPhpAuthentication _onPhpAuthentication;
    
    IEnumerator LoginGame(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        
        var response = new WWW(_loginUrl, form.data);

        yield return response;
        
        DebugTool.Instance().SetMessage(new DebugMessage(response.text, DebugMessageType.Message));

        JsonData data = JsonMapper.ToObject(response.text);

        if (data.Keys.Contains("Error"))
        {
            Debug.LogError(data["Error"]);
            _onPhpAuthentication(false);
        }
        else
        {
            _onPhpAuthentication(true, data["user_id"].ToString());
        }
    }

    public void Login(string username, string password, OnPhpAuthentication onPhpAuthentication)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("Sending credentials to gameScrypt. [" + username + "]", DebugMessageType.Message));

        _onPhpAuthentication = onPhpAuthentication;

        StartCoroutine(LoginGame(username, password));
    }
}