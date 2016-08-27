using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace DuckDuckShoot.Models
{
    public class Game
    {
        public int InitialDucks { get; }
        public TimeSpan TurnTime { get; }

        public List<Player> Players { get; }

        public Dictionary<Player, Action> TurnActions { get; }

        public Game(List<User> users, TimeSpan turnTime, int initialDucks)
        {
            InitialDucks = initialDucks;
            TurnTime = turnTime;
            Players = new List<Player>();
            TurnActions = new Dictionary<Player, Action>();

            // Add all users in the lobby to the game
            users.ForEach(user => Players.Add(new Player(user, InitialDucks)));       
        }

        public void StartTurn()
        {
            // Clear the actions for each player
            TurnActions.Clear();

            // Begin the timer
            Timer t = new Timer(ProcessTurn, null, TurnTime, TurnTime);

        }

        public void AddPlayerAction()
        {

        }

        public void ProcessTurn(object state)
        {

        }

        public class Action
        {
            public enum ActionType { SHOOT, DUCK };

            public ActionType ActType { get; }
            public Player Target { get; }

            public Action(ActionType type, Player target)
            {
                this.ActType = type;
                this.Target = target;
            }
        }

    }
}