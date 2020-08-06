using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Configuration;

public class PhpController : MonoBehaviour
{
    private const string LocalUrl = "http://localhost:8080/FFS/backend";
    private const string UnityInitGame = LocalUrl + "/Session_UnityInitGame.php";

    private readonly Dictionary<string, string> _postHeader = new Dictionary<string, string> { { "Content-Type", "text/json" } };

    private Main _main;

    public void Init(Main main)
    {
        _main = main;
    }

    IEnumerator InitGame(string gameSessionId)
    {
        var jsonString = "{ \"gameSessionId\" : \"" + gameSessionId + "\" }";
        byte[] encodedMessage = Encoding.ASCII.GetBytes(jsonString);

        var gameData = new WWW(UnityInitGame, encodedMessage, _postHeader);
        yield return gameData;

        JsonData data = JsonMapper.ToObject(gameData.text);

        if (data.Keys.Contains("Error"))
        {
            Debug.LogError(data["Error"]);
            _main.Game.HandshakeSessionCallback(false);
        }
        else
        {
            Debug.Log(data["message"]);
            _main.Game.HandshakeSessionCallback(true);
        }
    }

    public void HandshakeSession(string gameSessionId)
    {
        StartCoroutine(InitGame(gameSessionId));
    }
}