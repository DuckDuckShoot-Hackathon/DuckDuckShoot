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

        public User getUserFromName(string name)
        {
            foreach (User user in Users)
            {
                if (user.Name.Equals(name))
                {
                    return user;
                }
            }
            return null;
        }

        public List<string> getTakenNames()
        {
            var names = new List<string>();

            foreach (User user in Users)
            {
                names.Add(user.Name);
            }

            return names;
        }

    }
}