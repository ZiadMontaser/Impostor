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
            VireusPlugin.OnPLayerJoin(e);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            VireusPlugin.OnStartGame(e.Game);
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            VireusPlugin.OnPlayerChat(e);
        }

        [EventListener]
        public void OnPlayerReportedBodyEvent(IPlayerReportedBodyEvent e)
        {
            _logger.LogDebug("Player reported body");
        }
    }
}
