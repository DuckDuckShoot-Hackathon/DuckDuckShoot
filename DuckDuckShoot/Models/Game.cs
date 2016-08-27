using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuckDuckShoot.Models
{
    public class Game
    {
        public enum Actions { DUCK, SHOOT };

        public int InitialDucks { get; }

        public List<Player> Players { get; }

        public bool TurnCounting { get; set; }
        public DateTime TurnEnd { get; set; }

        public Game(List<User> users, int initialDucks)
        {
            InitialDucks = initialDucks;
            Players = new List<Player>();

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

        
    }
}