using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Main;
using GameSparks.Api.Responses;
using GameSparks.Core;
using UnityEngine;

public class GameSparksOnlineFactory : MonoBehaviour, IGameSparksFactory
{
    public void WhereAmI()
    {
        Debug.Log("online");
    }

    public void Init(GameSparksController.GameSparksStatusCallback gameSparksStatusCallback)
    {
        // we wont immediately have connection, so at the start of the lobby we will set the connection status to show this
        DebugTool.Instance().SetMessage(new DebugMessage("No Connection...", DebugMessageType.Warning));

        GS.GameSparksAvailable += (isAvailable) =>
        {
            if (isAvailable)
            {
                DebugTool.Instance().SetMessage(new DebugMessage("GameSparks Connected...", DebugMessageType.Message));
            }
            else
            {
                DebugTool.Instance().SetMessage(new DebugMessage("GameSparks Disconnected...", DebugMessageType.Warning));
            }
            gameSparksStatusCallback(isAvailable);
        };
    }

    public void AuthenticateUser(string userName, string password, GameSparksController.RegCallback regcallback, GameSparksController.AuthCallback authcallback, GameSparksController.ErrorCallback errorCallback)
    {
        new GameSparks.Api.Requests.RegistrationRequest()
                  // this login method first attempts a registration //
                  // if the player is not new, we will be able to tell as the registrationResponse has a bool 'NewPlayer' which we can check
                  // for this example we use the user-name was the display name also //
                  .SetDisplayName(userName)
                  .SetUserName(userName)
                  .SetPassword(password)
                  .Send((regResp) =>
                  {
                      if (!regResp.HasErrors)
                      { // if we get the response back with no errors then the registration was successful
                          DebugTool.Instance().SetMessage(new DebugMessage("GSM| Registration Successful...", DebugMessageType.Message));
                          regcallback(regResp);
                      }
                      else
                      {
                          // if we receive errors in the response, then the first thing we check is if the player is new or not
                          if (!(bool)regResp.NewPlayer) // player already registered, lets authenticate instead
                          {
                              DebugTool.Instance().SetMessage(new DebugMessage("GSM| Existing User, Switching to Authentication", DebugMessageType.Message));
                              new GameSparks.Api.Requests.AuthenticationRequest()
                                  .SetUserName(userName)
                                  .SetPassword(password)
                                  .Send((authResp) =>
                                  {
                                      if (!authResp.HasErrors)
                                      {
                                          DebugTool.Instance().SetMessage(new DebugMessage("Authentication Successful...", DebugMessageType.Message));
                                          authcallback(authResp);
                                      }
                                      else
                                      {
                                          DebugTool.Instance().SetMessage(new DebugMessage("GSM| Error Authenticating User \n", DebugMessageType.Error));
                                      }
                                  });
                          }
                          else
                          {
                              // if there is another error, then the registration must have failed
                              DebugTool.Instance().SetMessage(new DebugMessage("GSM| Error Authenticating User \n", DebugMessageType.Error));
                              errorCallback();
                          }
                      }
                  });
    }

    public void FindPlayers(GameSparksController.NoMatchCallback noMatchCallback, GameSparksController.MatchCallback matchCallback)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("GSM| Attempting Matchmaking...", DebugMessageType.Message));

        new GameSparks.Api.Requests.MatchmakingRequest()
            .SetMatchShortCode("S_CHAT") // set the shortCode to be the same as the one we created in the first tutorial
            .SetSkill(0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
            .Send((response) =>
            {
                if (response.HasErrors)
                { // check for errors
                    DebugTool.Instance().SetMessage(new DebugMessage("GSM| MatchMaking Error \n", DebugMessageType.Error));
                }
            });

        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) =>
        {
            noMatchCallback();
        };

        GameSparks.Api.Messages.MatchFoundMessage.Listener = (_message) =>
        {
            DebugTool.Instance().SetMessage(new DebugMessage("Match Found!", DebugMessageType.Message));

            StringBuilder sBuilder = new StringBuilder();
            sBuilder.AppendLine("Match Found...");
            sBuilder.AppendLine("Host URL:" + _message.Host);
            sBuilder.AppendLine("Port:" + _message.Port);
            sBuilder.AppendLine("Access Token:" + _message.AccessToken);
            sBuilder.AppendLine("MatchId:" + _message.MatchId);
            sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
            sBuilder.AppendLine("_________________");
            sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
            foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
            {
                sBuilder.AppendLine("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
            }

            DebugTool.Instance().SetMessage(new DebugMessage(sBuilder.ToString(), DebugMessageType.Message));

            matchCallback(_message, sBuilder.ToString());
        };
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
        ref RtSessionInfo sessionInfo, 
        ref GameSparksRTUnity gameSparksRtUnity)
    {
        DebugTool.Instance().SetMessage(new DebugMessage("GSM| Creating New RT Session Instance...", DebugMessageType.Message));
        sessionInfo = info;
        gameSparksRtUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game
        // In order to create a new RT game we need a 'FindMatchResponse' //
        // This would usually come from the server directly after a successful MatchmakingRequest //
        // However, in our case, we want the game to be created only when the first player decides using a button //
        // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
        // is passed in. //
        GSRequestData mockedResponse = new GSRequestData()
                                            .AddNumber("port", (double)info.GetPortID())
                                            .AddString("host", info.GetHostURL())
                                            .AddString("accessToken", info.GetAccessToken()); // construct a dataset from the game-details

        FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
        // So in the game-config method we pass in the response which gives the instance its connection settings //
        // In this example, I use a lambda expression to pass in actions for 
        // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
        // These methods are self-explanatory, but the important one is the OnPacket Method //
        // this gets called when a packet is received //

        gameSparksRtUnity.Configure(response,
            (peerId) => { onPlayerConnectedToGame(peerId); },
            (peerId) => { onPlayerDisconnected(peerId); },
            (ready) => { onRtReady(ready); },
            (packet) => { onPacketReceived(packet); });
        gameSparksRtUnity.Connect(); // when the config is set, connect the game
    }
}