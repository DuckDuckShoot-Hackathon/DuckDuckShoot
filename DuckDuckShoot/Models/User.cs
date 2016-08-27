using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DuckDuckShoot.Models
{
    public class User
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ConnectionId")]
        public string ConnectionId { get; set; }

        public User(string name, string connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
        }
    }
}