using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuckDuckShoot.Models
{
    public class Lobby
    {
        public List<User> Users { get; }
        public Game CurrentGame { get; set; }

        public Lobby()
        {
            Users = new List<User>();
            CurrentGame = null;
        }

    }
}