using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using System.Threading.Tasks;
using DuckDuckShoot.Models;

namespace DuckDuckShoot.Hubs
{
    [HubName("game")]
    public class GameHub : Hub
    {
        public Lobby GameLobby => Lobby.LobbySingleton;

        private Timer turnTimer;

        public void StartGame()
        {
            GameLobby.CurrentGame = new Game(GameLobby.Users, new TimeSpan(0, 1, 0), 3);

            // Tell all clients that the game has started
            Clients.All.gameStart(GameLobby.CurrentGame.Players.Select(p => p.PlayerUser.Name).ToArray());

            StartTurn();
        }

        public void EndGame()
        {
            GameLobby.CurrentGame = null;

            // Tell all clients that the game has ended
            var winners = new string[1] { "memes" };
            Clients.All.gameEnd();
        }

        public void StartTurn()
        {
            if (GameLobby.CurrentGame.IsGameOver())
            {
                EndGame();
                return;
            }
            
            // Begin the timer to the end of the turn
            GameLobby.CurrentGame.StartTurn();

            // Tell all clients the turn is starting
            Clients.All.turnStart(GameLobby.getLobbyState());

             turnTimer = new Timer(ProcessGameTurn, null, GameLobby.CurrentGame.TurnTime, TimeSpan.FromMilliseconds(-1));

            
        }

        /// <summary>
        /// Method for client to call to send their action for a turn
        /// </summary>
        /// <param name="actionString"> the formatted string of the action to take</param>
        public void SendAction(string actionString)
        {
            string[] tokens = actionString.Split(' ');

            // String format: [DUCK | SHOOT target] 
            
            Game.Action action = null;
            Player actor = GameLobby.CurrentGame.getPlayerFromConnectionId(Context.ConnectionId);

            if (actor == null)
            {
                return;
            }

            
            if (tokens[0].Equals("DUCK"))
            {
                action = new Game.Action(Game.Action.ActionType.DUCK, actor, actor);
            }
            else if (tokens[0].Equals("SHOOT"))
            {
                Player target = GameLobby.CurrentGame.getPlayerFromName(tokens[1]);
                action = new Game.Action(Game.Action.ActionType.SHOOT, actor, target);
            }

            GameLobby.CurrentGame.AddPlayerAction(actor, action);                                   
        }
        
        /// <summary>
        /// Method the client sends to indicate it is ready for the next turn
        /// </summary>
        public void SendReady()
        {
            if (!GameLobby.CurrentGame.IsMidTurn)
            {
                GameLobby.CurrentGame.UnreadiedPlayers--;
                if (GameLobby.CurrentGame.UnreadiedPlayers <= 0)
                {
                    StartTurn();
                }
            }
        }

        public void ProcessGameTurn(object state)
        {
            GameLobby.CurrentGame.ProcessTurn();

            // List of outcomes
            List<Game.Outcome> outcomes = GameLobby.CurrentGame.TurnOutcomes;

            // Send outcomes to clients
            Clients.All.getOutcomes(outcomes.ToArray());
            
        }

        public string GetName()
        {
            return GameLobby.getUserFromConnectionId(Context.ConnectionId)?.Name;
        }

        public bool NewName(string name)
        {
            string connectionId = Context.ConnectionId;
            // Add this person with their name/check if it exists already.
            if (GameLobby.getUserFromName(name) != null)
            {
                // Request a new name
                return false;
            }
            GameLobby.Users.Add(new User(name, connectionId));
            Clients.Others.addPlayer(name);
            // Returns whether or not the name is valid
            return true;
        }

        public LobbyState GetCurrentLobbyState()
        {
            return GameLobby.getLobbyState();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;

            //Remove this user using the connection id
            if (GameLobby.CurrentGame != null)
            {
                GameLobby.CurrentGame.RemovePlayerFromGame(GameLobby.CurrentGame.getPlayerFromConnectionId(connectionId));
            }
            Clients.Others.removePlayer(GameLobby.getUserFromConnectionId(connectionId).Name);
            GameLobby.Users.Remove(GameLobby.getUserFromConnectionId(connectionId));
            return base.OnDisconnected(stopCalled);
        }
    }
}