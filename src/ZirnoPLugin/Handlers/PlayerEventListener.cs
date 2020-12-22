using System;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using ZirnoPlugin.Gamemode;
using ZirnoPlugin.Commands;
using Microsoft.Extensions.Logging;
using Impostor.Api.Net.Messages;

namespace ZirnoPlugin.Handlers
{
    public class PlayerEventListener : IEventListener
    {
        private static readonly Random Random = new Random();

        private readonly ILogger<PlayerEventListener> _logger;
        private readonly IMessageWriterProvider _provider;

        CommandPlugin CommandPlugin = new CommandPlugin();

        public PlayerEventListener(ILogger<PlayerEventListener> logger , IMessageWriterProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            GamemodeManager.games[e.Game].OnPlayerSpawnd(e);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            GamemodeManager.games[e.Game].OnGameStarted(e);
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            GamemodeManager.games[e.Game].OnGameEnded(e);
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            GamemodeManager.games[e.Game].OnPlayerDestroyed(e);
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            CommandPlugin.OnPLayerChat(e);
            GamemodeManager.games[e.Game].OnPlayerChat(e);
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
    }
}
