using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DuckDuckShoot.Models
{
    public class Player
    {

        // The User in the lobby that controls this player
        [JsonProperty("PlayerUser")]
        public User PlayerUser { get; }
        [JsonProperty("IsActiveUser")]
        public bool IsActiveUser { get; set; }

        // Whether this Player is alive
        private bool isAlive;
        [JsonProperty("IsAlive")]
        public bool IsAlive { get { return isAlive; } }
        // The time at which the player died
        [JsonProperty("DeathTime")]
        public DateTime? DeathTime { get; set; }

        // The number of ducks a player has left
        private int numDucks;
        [JsonProperty("NumDucks")]
        public int NumDucks { get { return numDucks; } }

        public Player(User user, int numDucks)
        {
            isAlive = true;
            IsActiveUser = true;
            PlayerUser = user;
            this.numDucks = numDucks;
            DeathTime = null;
        }

        /// <summary>
        /// Set the player's status to dead
        /// </summary>
        public void Kill()
        {
            isAlive = false;
            DeathTime = DateTime.Now;
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

        public override string ToString()
        {
            return String.Format("{0}: {1} - {2} ducks left", PlayerUser.Name,
                IsAlive ? "Alive" : "Dead", NumDucks);
        }

    }
}