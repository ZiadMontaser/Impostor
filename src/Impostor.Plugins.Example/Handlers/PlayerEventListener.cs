using System;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth.Customization;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example.Handlers
{
    public class PlayerEventListener : IEventListener
    {
        private static readonly Random Random = new Random();

        private readonly ILogger<PlayerEventListener> _logger;

        VireusPlugin VireusPlugin = new VireusPlugin(Random);

        public PlayerEventListener(ILogger<PlayerEventListener> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            _logger.LogDebug(e.PlayerControl.PlayerInfo.PlayerName + " spawned");
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
            VireusPlugin.OnStartGame(e.Game);
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            VireusPlugin.OnEndGame(e.Game);
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            //VireusPlugin.OnPlayerChat(e);
            if (e.Message.StartsWith('/'))
            {
                var command = e.Message.Split(' ');
                switch (command[0])
                {
                    case "/tp":
                        e.PlayerControl.NetworkTransform.SnapToAsync(new Vector2(float.Parse(command[1]), float.Parse(command[2])));
                        break;
                }
            }
        }

        [EventListener]
        public void OnPlayerReportedBodyEvent(IPlayerReportedBodyEvent e)
        {
            _logger.LogDebug("Player reported body");
        }
    }
}
