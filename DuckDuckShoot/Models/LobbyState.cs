using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DuckDuckShoot.Models
{
    public class LobbyState
    {        
        [JsonProperty("players")]
        public Player[] Players { get; set; }
        [JsonProperty("users")]
        public User[] Users { get; set; }
        [JsonProperty("gameInProgress")]
        public bool GameInProgress { get; set; }
    }
}