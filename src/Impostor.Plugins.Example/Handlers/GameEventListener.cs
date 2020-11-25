using System;
using Impostor.Api.Events;

namespace Impostor.Plugins.Example.Handlers
{
    public class GameEventListener : IEventListener
    {
        [EventListener(EventPriority.Monitor)]
        public void OnGame(IGameEvent e)
        {
            Console.WriteLine(e.GetType().Name + " triggered");
        }

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            Console.WriteLine("Game > created");
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            Console.WriteLine("Game > starting");
            e.Game.Options.IsDefaults = false;
            e.Game.Options.EmergencyCooldown = int.MaxValue;
            e.Game.Options.NumEmergencyMeetings = 0;
            e.Game.Options.NumCommonTasks = 2;
            e.Game.Options.NumLongTasks = 3;
            e.Game.Options.NumShortTasks = 5;
            e.Game.Options.KillCooldown = float.MaxValue;
            e.Game.Options.PlayerSpeedMod = 1.5f;
            e.Game.Options.VotingTime = 1;
            e.Game.Options.DiscussionTime = 0;
            e.Game.SyncSettingsAsync();
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            Console.WriteLine("Game > started");

            foreach (var player in e.Game.Players)
            {
                var info = player.Character.PlayerInfo;

                Console.WriteLine($"- {info.PlayerName} {info.IsImpostor}");
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            Console.WriteLine("Game > ended");
            Console.WriteLine("- Reason: " + e.GameOverReason);
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            Console.WriteLine("Game > destroyed");
        }

        [EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            Console.WriteLine("Player joined a game.");
        }

        [EventListener]
        public void OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            Console.WriteLine("Player left a game.");
        }
    }
}
