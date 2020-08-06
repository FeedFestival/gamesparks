using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private static SceneManager instance = null;
    public static SceneManager Instance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            Debug.LogError("SceneManager.cs Not Initialized...");
        }
        return null;
    }

    void Awake()
    {
        instance = this;
    }

    /**/

    [HideInInspector] public CanvasController CanvasController;

    [HideInInspector] public PhpController PhpController;
    
    [HideInInspector] public Game Game;

    [HideInInspector] public ChatController ChatController;

    [HideInInspector] public LoginController LoginController;

    public void Init(Game game)
    {
        Game = game;

        CanvasController = transform.GetChild(0).GetComponent<CanvasController>();
        CanvasController.Init(this);

        PhpController = GetComponent<PhpController>();
        PhpController.Init(GameSparksController.Instance().PhpOffline);

        LoginController = GetComponent<LoginController>();

        ChatController = GetComponent<ChatController>();
    }
}