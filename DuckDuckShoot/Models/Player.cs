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

        // Whether this Player is alive
        private bool isAlive;
        public bool IsAlive { get { return isAlive; } }

        // The number of ducks a player has left
        private int numDucks;
        public int NumDucks { get { return numDucks; } }

        public Player(User user, int numDucks)
        {
            isAlive = true;
            PlayerUser = user;
            this.numDucks = numDucks;
        }

        /// <summary>
        /// Set the player's status to dead
        /// </summary>
        public void Kill()
        {
            isAlive = false;
        }

        /// <summary>
        /// Remove one duck from the player's count
        /// </summary>
        public void Duck()
        {
            if (NumDucks > 0)
            {
                numDucks--;
            }
        }

    }
}