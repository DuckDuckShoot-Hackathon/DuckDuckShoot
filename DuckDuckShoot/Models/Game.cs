using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuckDuckShoot.Models
{
    public class Game
    {
        public int InitialDucks { get; }

        public List<Player> Players { get; }

        public bool TurnCounting { get; set; }
        public Dictionary<Player, Action> TurnActions { get; }
        public DateTime TurnEnd { get; set; }

        public Game(List<User> users, int initialDucks)
        {
            InitialDucks = initialDucks;
            Players = new List<Player>();
            TurnActions = new Dictionary<Player, Action>();

            // Add all users in the lobby to the game
            users.ForEach(user => Players.Add(new Player(user, InitialDucks)));

            
            TurnCounting = false;
            TurnEnd = new DateTime();            
        }

        public void StartTurn()
        {

        }

        public void AddPlayerAction()
        {

        }

        public void ProcessTurn()
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