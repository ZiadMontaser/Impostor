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

        Gamemodes gamemodes = Gamemodes.HotPotato;
        VireusPlugin VireusPlugin = new VireusPlugin(Random);

        public PlayerEventListener(ILogger<PlayerEventListener> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            if (gamemodes == Gamemodes.HotPotato)
            {
                _logger.LogDebug(e.PlayerControl.PlayerInfo.PlayerName + " spawned");
                VireusPlugin.SetGameOptions(e.Game);
            }
            else
            {
                e.Game.Options.IsDefaults = true;
                e.Game.SyncSettingsAsync();
            }
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            if (gamemodes == Gamemodes.HotPotato)
            {
                VireusPlugin.OnStartGame(e.Game);
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            if (gamemodes == Gamemodes.HotPotato)
            {
                VireusPlugin.OnEndGame(e.Game);
            }
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
