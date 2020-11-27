using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Events.Player;
using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;


namespace Impostor.Plugins.Example
{
    class Plugin
    {
        public virtual void OnPlayerSpawnd(IPlayerSpawnedEvent e) { }

        public virtual void OnPlayerChat(IPlayerChatEvent e) { }

        public virtual void OnPlayerLeft(IGamePlayerLeftEvent e) { }

        public virtual void OnGameStarted(IGameStartedEvent e) { }

        public virtual void OnGameEnded(IGameEndedEvent e) { }
    }
}
