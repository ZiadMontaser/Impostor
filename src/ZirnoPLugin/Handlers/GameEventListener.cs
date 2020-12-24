using System;
using Impostor.Api.Events;
using ZirnoPlugin.Gamemode;

namespace ZirnoPlugin.Handlers
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
            GamemodeManager.CreateGame(e.Game);
        }


        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            Console.WriteLine("Game > destroyed");
            GamemodeManager.DeleteGame(e.Game);
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            Console.WriteLine("Game > starting");
            GamemodeManager.games[e.Game].SetGameOptions(e.Game);
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
            e.Game.Options.IsDefaults = true;
            e.Game.SyncSettingsAsync();
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