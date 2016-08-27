using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DuckDuckShoot.Models
{
    public class LobbyState
    {
        [JsonProperty("names")]
        public string[] Names { get; set; }
        [JsonProperty("gameInProgress")]
        public bool GameInProgress { get; set; }
        [JsonProperty("aliveNames")]
        public string[] AliveNames { get; set; }
    }
}