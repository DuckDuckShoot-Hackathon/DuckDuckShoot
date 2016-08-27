using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading;
using System.Threading.Tasks;
using DuckDuckShoot.Models;

namespace DuckDuckShoot.Hubs
{
    [HubName("game")]
    public class GameHub : Hub
    {
        public Lobby GameLobby { get; set; }
        public Game CurrentGame { get; set; }
        private static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void StartGame()
        {
            GameLobby.CurrentGame = new Game(GameLobby.Users, new TimeSpan(0, 1, 0), 3);
            CurrentGame = GameLobby.CurrentGame;
        }

        public void StartTurn()
        {
            // Begin the timer to the end of the turn
            CurrentGame.ResetTurn();
            Timer t = new Timer(ProcessGameTurn, null, CurrentGame.TurnTime, CurrentGame.TurnTime);
        }

        public void SendAction(string actionString)
        {
            string[] tokens = actionString.Split(' ');

            // String format: [DUCK | SHOOT target] 
            
            Game.Action action = null;
            Player actor = CurrentGame.getPlayerFromConnectionId(Context.ConnectionId);

            if (actor == null)
            {
                return;
            }

            
            if (tokens[0].Equals("DUCK"))
            {
                action = new Game.Action(Game.Action.ActionType.DUCK, actor, actor);
            }
            else if (tokens[0].Equals("SHOOT"))
            {
                Player target = CurrentGame.getPlayerFromName(tokens[1]);
                action = new Game.Action(Game.Action.ActionType.SHOOT, actor, target);
            }
                                              
        }

        public void ProcessGameTurn(object state)
        {
            CurrentGame.ProcessTurn();

            // List of outcomes
            List<Game.Outcome> Outcomes = CurrentGame.TurnOutcomes;

            // Send outcomes to clients
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }

    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        public int Count => _connections.Count;

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }
}