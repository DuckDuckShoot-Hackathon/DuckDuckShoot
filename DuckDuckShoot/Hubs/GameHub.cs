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
        public Game CurrentGame { get; set; }

        public void Send(string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(GameLobby.getUserFromConnectionId(Context.ConnectionId).Name, message);
        }

        public void StartGame()
        {
            GameLobby.CurrentGame = new Game(GameLobby.Users, new TimeSpan(0, 1, 0), 3);
            CurrentGame = GameLobby.CurrentGame;
        }

        public void StartTurn()
        {
            // Begin the timer to the end of the turn
            CurrentGame.StartTurn();
            Timer t = new Timer(ProcessGameTurn, null, CurrentGame.TurnTime, CurrentGame.TurnTime);

            // Tell all clients the turn is starting
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
            Player actor = CurrentGame.getPlayerFromConnectionId(Context.ConnectionId);

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
                Player target = CurrentGame.getPlayerFromName(tokens[1]);
                action = new Game.Action(Game.Action.ActionType.SHOOT, actor, target);
            }

            CurrentGame.AddPlayerAction(actor, action);                                   
        }
        
        /// <summary>
        /// Method the client sends to indicate it is ready for the next turn
        /// </summary>
        public void SendReady()
        {
            if (!CurrentGame.IsMidTurn)
            {
                CurrentGame.UnreadiedPlayers--;
                if (CurrentGame.UnreadiedPlayers <= 0)
                {
                    StartTurn();
                }
            }
        }

        public void ProcessGameTurn(object state)
        {
            CurrentGame.ProcessTurn();

            // List of outcomes
            List<Game.Outcome> Outcomes = CurrentGame.TurnOutcomes;

            // Send outcomes to clients
            
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
            // Returns whether or not the name is valid
            return true;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string connectionId = Context.ConnectionId;

            //Remove this user using the connection id
            return base.OnDisconnected(stopCalled);
        }
    }
}