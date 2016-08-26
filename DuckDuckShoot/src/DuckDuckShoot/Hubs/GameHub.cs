using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DuckDuckShoot.Hubs
{
    [HubName("game")]
    public class GameHub : Hub
    {
        public void Echo(string message)
        {
            Clients.Caller.echo(message);
        }
    }
}
