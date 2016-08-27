using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using DuckDuckShoot.Models;

namespace DuckDuckShoot.Hubs
{
    [HubName("game")]
    public class GameHub : Hub
    {
        public Lobby GameLobby { get; set; }
        public Game CurrentGame { get; set; }

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void StartGame()
        {
            GameLobby.CurrentGame = new Game(GameLobby.Users, new System.TimeSpan(0, 1, 0), 3);
            CurrentGame = GameLobby.CurrentGame;
        }

        public void StartTurn()
        {
            // Begin the timer to the end of the turn
            CurrentGame.ResetTurn();
            Timer t = new Timer(ProcessGameTurn, null, CurrentGame.TurnTime, CurrentGame.TurnTime);
        }

        public void SendAction(string actionString)
        {

            // String format: [DUCK | SHOOT target] 
            Game.Action action = null;
            Player target;

            string[] tokens = actionString.Split(' ');
            if (tokens[0].Equals("DUCK"))
            {

                action = new Game.Action(Game.Action.ActionType.DUCK, null, null);
            }
            else if (tokens[0].Equals("SHOOT"))
            {
                Player actor;
                action = new Game.Action(Game.Action.ActionType.SHOOT, null, null);
            }
                        
        }

        public void ProcessGameTurn(object state)
        {
            CurrentGame.ProcessTurn();

            // List of outcomes
            List<Game.Outcome> Outcomes = CurrentGame.TurnOutcomes;

            // Send outcomes to clients
        }
    }
}