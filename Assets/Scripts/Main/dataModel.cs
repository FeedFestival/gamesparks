using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utils;
using GameSparks.Api.Messages;

namespace Assets.Scripts.Main
{
    public class User
    {
        public int Id { get; set; }
        
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }
        public int Maps { get; set; }
        public bool IsUsingSound { get; set; }
        public int ControllerType { get; set; }
        
        public FacebookApp FacebookApp { get; set; }

        public override string ToString()
        {
            return string.Format("User { Id={0}, Name={1}, Maps={2}, IsUsingSound={3}, ControllerType={4}, FacebookId={5} }", Id, Name, Maps, IsUsingSound, ControllerType, FacebookApp.FacebookId);
        }

        public static User PhpFillData(string properties)
        {
            return new User
            {
                Id = utils.GetIntDataValue(properties, "ID:"),
                Name = utils.GetDataValue(properties, "Name:"),
                Maps = utils.GetIntDataValue(properties, "Maps:"),
                IsUsingSound = utils.GetBoolDataValue(properties, "IsUsingSound:"),
                ControllerType = utils.GetIntDataValue(properties, "ControllerType:"),
                FacebookApp = new FacebookApp
                {
                    FacebookId = utils.GetLongDataValue(properties, "FacebookId:")
                }
            };
        }
    }

    public class FacebookApp
    {
        //[PrimaryKey, AutoIncrement]   // sqLite
        public int Id { get; set; }

        public long FacebookId { get; set; }

        public string Name { get; set; }

        public int ConnectId { get; set; }

        public string BToken { get; set; }
    }

    /**/
    public class RtSessionInfo
    {
        private string hostURL;
        public string GetHostURL() { return this.hostURL; }
        private string acccessToken;
        public string GetAccessToken() { return this.acccessToken; }
        private int portID;
        public int GetPortID() { return this.portID; }
        private string matchID;
        public string GetMatchID() { return this.matchID; }

        private List<RTPlayer> playerList = new List<RTPlayer>();
        public List<RTPlayer> GetPlayerList()
        {
            return playerList;
        }

        /// <summary>
        /// Creates a new RTSession object which is held until a new RT session is created
        /// </summary>
        /// <param name="_message">Message.</param>
        public RtSessionInfo(MatchFoundMessage _message)
        {
            portID = (int)_message.Port;
            hostURL = _message.Host;
            acccessToken = _message.AccessToken;
            matchID = _message.MatchId;
            // we loop through each participant and get their peerId and display name //
            foreach (MatchFoundMessage._Participant p in _message.Participants)
            {
                playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
            }
        }

        public class RTPlayer
        {
            public RTPlayer(string _displayName, string _id, int _peerId)
            {
                this.displayName = _displayName;
                this.id = _id;
                this.peerId = _peerId;
            }

            public string displayName;
            public string id;
            public int peerId;
            public bool isOnline;
        }
    }
}