using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Main Main;

    public Text Text;
    
    void Awake()
    {
        Main.Init(this);

        Text.text = "Hello there.";

        // This is a script that exists in the game.html
        // this script when is called - tells angular that unity is ready to recieve the game session id.
        Application.ExternalCall("setGameReady");

        // TODO: On the loader we should have a message : Seting up session, while we wait for javascript and php.


        // TODO: this usually stays in SetSession
        // wich is called from the browser Javascript/Angular
        Main.PhpController.HandshakeSession("7618110C-58EC-43DC-95DB-5B4CFD0D40B6");
    }

    public void HandshakeSessionCallback(bool success)
    {
        //if (success)
            Main.GameSparksController.Init(Main);
    }

    public void SetSession(string sessionId)
    {
        Text.text = sessionId;
        //Main.PhpController.HandshakeSession(sessionId);
    }
}