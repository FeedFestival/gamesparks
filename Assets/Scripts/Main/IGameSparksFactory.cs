using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Main
{
    public interface IGameSparksFactory
    {
        void WhereAmI();
        void Init(GameSparksController.GameSparksStatusCallback gameSparksStatusCallback);
        void FindPlayers(GameSparksController.NoMatchCallback noMatchCallback, GameSparksController.MatchCallback matchCallback);
        void AuthenticateUser(string userName, string password, GameSparksController.RegCallback regcallback,
            GameSparksController.AuthCallback authcallback, GameSparksController.ErrorCallback errorCallback);

        void StartNewRtSession(RtSessionInfo info,
            GameSparksRTController.OnPlayerConnectedToGame onPlayerConnectedToGame,
            GameSparksRTController.OnPlayerDisconnected onPlayerDisconnected,
            GameSparksRTController.OnRTReady onRtReady,
            GameSparksRTController.OnPacketReceived onPacketReceived,
            ref RtSessionInfo sessionInfo, ref GameSparksRTUnity gameSparksRtUnity);
    }
}
