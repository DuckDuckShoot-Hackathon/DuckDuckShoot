using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuckDuckShoot.Models
{
    public class Lobby
    {
        private static Lobby lobbySingleton = null;

        public static Lobby LobbySingleton
        {
            get
            {
                if (lobbySingleton == null)
                {
                    lobbySingleton = new Lobby();
                }
                return lobbySingleton;
            }
        }

        public List<User> Users { get; }
        public Game CurrentGame { get; set; }

        public Lobby()
        {
            Users = new List<User>();
            CurrentGame = null;
        }

        public User getUserFromName(string name)
        {
            foreach (User user in Users)
            {
                if (user.Name.Equals(name))
                {
                    return user;
                }
            }
            return null;
        }

        public User getUserFromConnectionId(string connId)
        {
            foreach (User user in Users)
            {
                if (user.ConnectionId.Equals(connId))
                {
                    return user;
                }
            }
            return null;
        }

        public List<string> getTakenNames()
        {
            var names = new List<string>();

            foreach (User user in Users)
            {
                names.Add(user.Name);
            }

            return names;
        }
        
        public LobbyState getLobbyState()
        {
            LobbyState state = new LobbyState();

            state.Users = Users.ToArray();

            state.GameInProgress = (CurrentGame != null);

            state.Players = null;

            if (state.GameInProgress)
            {
                state.Players = CurrentGame.Players.ToArray();
            }
            
            return state;
        }
    }
}