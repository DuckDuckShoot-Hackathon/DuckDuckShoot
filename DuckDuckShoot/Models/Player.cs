using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuckDuckShoot.Models
{
    public class Player
    {

        // The User in the lobby that controls this player
        public User PlayerUser { get; }

        // Whetehr this Player is alive

        public User Alive { get; }

        // The number of ducks a player has left
        private int numDucks;
        public int NumDucks { get { return numDucks; } }

        public Player(User user, int numDucks)
        {
            PlayerUser = user;
            this.numDucks = numDucks;
        }

    }
}