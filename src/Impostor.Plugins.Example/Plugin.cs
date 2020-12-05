using Impostor.Api.Events.Player;
using Impostor.Api.Events;
using Impostor.Plugins.Example.Gamemode;
using Impostor.Api.Games;

namespace Impostor.Plugins.Example
{
     class Plugin
    {
        public Gamemodes mode { get; protected set; }

        public virtual void OnPlayerSpawnd(IPlayerSpawnedEvent e) { }

        public virtual void OnPlayerChat(IPlayerChatEvent e) { }

        public virtual void OnPlayerDestroyed(IPlayerDestroyedEvent e) { }

        public virtual void OnGameStarted(IGameStartedEvent e) { }

        public virtual void OnGameEnded(IGameEndedEvent e) { }

        public virtual void SetGameOptions(IGame game) { }
    }
}
