using Impostor.Api.Events.Player;
using Impostor.Api.Events;
using ZirnoPlugin.Gamemode;
using Impostor.Api.Games;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Hazel;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using System.Collections.Generic;

namespace ZirnoPlugin
{
    partial class Plugin
    {
        public Gamemodes mode { get; protected set; }

        public virtual void OnPlayerSpawnd(IPlayerSpawnedEvent e) { }

        public virtual void OnPlayerChat(IPlayerChatEvent e) { }

        public virtual void OnPlayerMurder(IPlayerMurderEvent e) { }

        public virtual void OnPlayerDestroyed(IPlayerDestroyedEvent e) { }

        public virtual void OnGameStarted(IGameStartedEvent e) { }

        public virtual void OnGameEnded(IGameEndedEvent e) { }

        public virtual void SetGameOptions(IGame game) { }

        protected async Task SetNameOnlyAsync(IClientPlayer player, string name)
        {
            using (var w = MessageWriter.Get(MessageType.Reliable))
            {
                w.StartMessage(MessageFlags.GameData);
                w.Write(player.Game.Code);

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(player.Character.NetId);
                w.Write((byte)RpcCalls.SetName);
                w.Write(name);
                w.EndMessage();

                w.EndMessage();

                await player.Game.SendToAsync(w, player.Client.Id);
            }
        }

        protected async Task EndGameAsync(IGame game, List<IInnerPlayerControl> winners)
        {
            foreach (var player in game.Players)
            {
                if (winners.Contains(player.Character))
                {
                    player.Character.SetImpostor();
                }
                else
                {
                    player.Character.SetCrewmate();
                }
            }

            await game.GameNet.GameData.UpdateGameDataAsync();

            var impostor = game.GetClientPlayer(winners[0].OwnerId);
            foreach (var player in game.Players)
            {
                if (!player.Character.PlayerInfo.IsImpostor)
                {
                    player.Character.SetMurderedByAsync(impostor);
                }
            }
        }
    }
}
