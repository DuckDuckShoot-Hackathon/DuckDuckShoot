using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace DuckDuckShoot.Models
{
    public class Game
    {
        public int InitialDucks { get; }
        public TimeSpan TurnTime { get; }

        public List<Player> Players { get; }

        public Dictionary<Player, Action> TurnActions { get; }

        public Game(List<User> users, TimeSpan turnTime, int initialDucks)
        {
            InitialDucks = initialDucks;
            TurnTime = turnTime;
            Players = new List<Player>();
            TurnActions = new Dictionary<Player, Action>();

            // Add all users in the lobby to the game
            users.ForEach(user => Players.Add(new Player(user, InitialDucks)));       
        }

        public void StartTurn()
        {
            // Clear the actions for each player
            TurnActions.Clear();

            // Begin the timer to the end of the turn
            Timer t = new Timer(ProcessTurn, null, TurnTime, TurnTime);

        }

        public void AddPlayerAction(Player player, Action action)
        {
            // Only alive players can add actions
            if (player.IsAlive)
            {
                TurnActions.Add(player, action);
            }
        }

        public void ProcessTurn(object state)
        {
            var actions = GetActionList();
            
            // Figure out who is ducking
            var duckingPlayers = new HashSet<Player>();
            foreach (Action action in actions)
            {
                // Duck only if they have Ducks left
                if (action.Actor.IsAlive && action.Actor.NumDucks > 0
                    && action.ActType == Action.ActionType.DUCK)
                {
                    duckingPlayers.Add(action.Actor);
                    action.Actor.Duck();
                }
            }

            // Perform shoot actions
            foreach (Action action in actions)
            {
                if (action.ActType == Action.ActionType.SHOOT)
                {
                    // Only kill the player if they are alive and not ducking
                    if (action.Target.IsAlive && !duckingPlayers.Contains(action.Target))
                    {
                        action.Target.Kill();
                    }
                }
            }
        }

        /// <summary>
        /// Get the actions performed this turn, in order
        /// </summary>
        /// <returns></returns>
        public List<Action> GetActionList()
        {
            List<Action> result = new List<Action>();

            foreach (KeyValuePair<Player, Action> entry in TurnActions)
            {
                result.Add(entry.Value);
            }

            return result;
        }

        public class Action
        {
            public enum ActionType { SHOOT, DUCK };

            public ActionType ActType { get; }
            public Player Actor { get; }
            public Player Target { get; }

            public Action(ActionType type, Player actor, Player target)
            {
                this.Actor = actor;
                this.ActType = type;
                this.Target = target;
            }
        }

    }
}