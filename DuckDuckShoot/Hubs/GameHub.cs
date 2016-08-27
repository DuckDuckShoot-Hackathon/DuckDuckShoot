﻿using System;
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
            GameLobby.CurrentGame = new Game(GameLobby.Users, new TimeSpan(0, 1, 0), (int)Math.Log(GameLobby.Users.Count, 2));

            // Tell all clients that the game has started
            Clients.All.gameStart(GameLobby.CurrentGame.Players.Select(p => p.PlayerUser.Name).ToArray());

            StartTurn();
        }

        public void EndGame()
        {

            var winners = GameLobby.CurrentGame.getAlivePlayers();

            GameLobby.CurrentGame = null;

            // Tell all clients that the game has ended
            Clients.All.gameEnd(winners.ToArray());
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
            if (GameLobby?.CurrentGame != null && !GameLobby.CurrentGame.IsMidTurn)
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

        public void BroadcastChatMessage(string message)
        {
            // Send message on to all other users
            User user = GameLobby.getUserFromConnectionId(Context.ConnectionId);
            Clients.All.receiveChatMessage(user, message);
        }

        public string GetName()
        {
            return GameLobby.getUserFromConnectionId(Context.ConnectionId)?.Name;
        }

        public bool NewName(string name)
        {
            string connectionId = Context.ConnectionId;
            // Add this person with their name/check if it exists already.
            if (string.IsNullOrEmpty(name) || GameLobby.getUserFromName(name) != null)
            {
                // Request a new name
                return false;
            }
            User newUser = new User(name, connectionId);
            GameLobby.Users.Add(newUser);
            Clients.Others.addUser(newUser);
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
            Clients.Others.removeUser(GameLobby.getUserFromConnectionId(connectionId));
            GameLobby.Users.Remove(GameLobby.getUserFromConnectionId(connectionId));
            return base.OnDisconnected(stopCalled);
        }
    }
}