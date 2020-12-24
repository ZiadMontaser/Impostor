using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Inner;
using ZirnoPlugin.Gamemode;

namespace ZirnoPlugin.Standerd
{
    class StanderdPlugin  :Plugin
    {
        public StanderdPlugin() => mode = Gamemodes.Standerd;

        public override void OnGameStarted(IGameStartedEvent e)
        {
            Task.Run(async () => {
                await Task.Delay(10 * 1000);
                using (var w = Impostor.Hazel.MessageWriter.Get(MessageType.Reliable))
                {
                    w.StartMessage(MessageFlags.GameDataTo);
                    w.Write(e.Game.Code);
                    w.Write(e.Game.Host.Client.Id);

                    w.StartMessage(GameDataTag.RpcFlag);
                    w.WritePacked(e.Game.GameNet.ShipStatus.NetId);
                    w.Write((byte)RpcCalls.CloseDoorsOfType);
                    w.Write((byte)SystemTypes.Weapons);
                    w.EndMessage();

                    w.EndMessage();

                    await e.Game.SendToAllAsync(w);
                }

                //e.Game.Host.Character.SetImpostor();
                //await e.Game.GameNet.GameData.UpdateGameDataAsync();
            });
        }
    }
}
