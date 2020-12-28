using System;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using ZirnoPlugin.Gamemode;
using ZirnoPlugin.Commands;
using Microsoft.Extensions.Logging;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Inner.Objects;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ZirnoPlugin.Handlers
{
    internal class PlayerEventListener : IEventListener
    {
        private static readonly Random Random = new Random();

        private readonly ILogger<PlayerEventListener> _logger;
        private readonly IMessageWriterProvider _provider;
        private readonly IServiceProvider _serviceProvider;

        private readonly CommandPlugin _commandPlugin;

        public PlayerEventListener(ILogger<PlayerEventListener> logger, IServiceProvider serviceProvider , IMessageWriterProvider provider , CommandPlugin commandPlugin)
        {
            _logger = logger;
            _provider = provider;
            _serviceProvider = serviceProvider;
            _commandPlugin = commandPlugin;
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
        public void OnPlayerMurder(IPlayerMurderEvent e)
        {
            GamemodeManager.games[e.Game].OnPlayerMurder(e);
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            _commandPlugin.OnPLayerChat(e);
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

        [EventListener]
        public async ValueTask OnGameSarting(IGameStartingEvent e)
        {
            Console.WriteLine("Starting");
            var players = new List<IInnerPlayerControl> { e.Game.Host.Character };
            await e.Game.SetInfectedAsync(players);
        }
    }
}
