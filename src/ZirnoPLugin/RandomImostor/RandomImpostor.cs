using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Server.Net.Inner;
using ZirnoPlugin;
using ZirnoPlugin.Gamemode;
using ZirnoPlugin.Viresed;

namespace ZirnoPLugin.RandomImostor
{
    class RandomImpostor : Plugin
    {
        int RoundLongSeconds = 60;

        private readonly Outfit _impostorOutfit;
        private readonly Outfit _crewmateOutfit;


        Thread updateThread;

        public RandomImpostor()
        {
            mode = Gamemodes.RandomImpostor;

            _impostorOutfit = new Outfit(HatType.Fred, ColorType.Red, SkinType.None, PetType.NoPet);
            _crewmateOutfit = new Outfit(HatType.HaloHat, ColorType.White, SkinType.None, PetType.NoPet);

        }

        public override void OnGameStarted(IGameStartedEvent e)
        {
            updateThread = new Thread(async ()=> await UpdateAsync(e.Game));

            updateThread.Start();
        }

        async Task UpdateAsync(IGame game)
        {
            Console.WriteLine("Random Impostor Started");

            while(game.GameState == GameStates.Started)
            {
                Thread.Sleep(1000 * RoundLongSeconds);

                if (game.GameState != GameStates.Started) break;

                var alivePlayers = game.Players.Where((player => !player.Character.PlayerInfo.IsDead)).ToList();

                int randomIndex = new Random().Next(alivePlayers.Count);

                List<IClientPlayer> playersNeedUpdate = new();

                for(byte i = 0; i < alivePlayers.Count; i++)
                {
                    var player = alivePlayers[i];
                    if (player.Character != null)
                    {
                        if(i == randomIndex)
                        {
                            player.Character.SetImpostor();
                            playersNeedUpdate.Add(player);
                        }
                        else
                        {
                            if (player.Character.PlayerInfo.IsImpostor)
                            {
                                playersNeedUpdate.Add(player);
                            }
                            player.Character.SetCrewmate();
                        }
                    }
                }

                await game.GameNet.GameData.UpdateGameDataAsync();

                foreach(var player in playersNeedUpdate)
                {
                    YourStateChanged(player);
                }

                //using (var w = MessageWriter.Get(MessageType.Reliable))
                //{
                //    w.StartMessage(MessageFlags.GameData);
                //    w.Write(game.Code);

                //    w.StartMessage(GameDataTag.RpcFlag);
                //    w.WritePacked(game.Host.Character.NetId);
                //    w.Write((byte)RpcCalls.StartMeeting);
                //    w.Write(byte.MaxValue);
                //    w.EndMessage();

                //    w.StartMessage(GameDataTag.ReadyFlag);
                //    w.WritePacked(game.GameNet.GameData.NetId);
                //    w.Write((byte)RpcCalls.Close);
                //    w.EndMessage();

                //    w.EndMessage();

                //    await game.SendToAllAsync(w);
                //    //foreach (var p in playersNeedUpdate) game.SendToAsync(w, p.OwnerId);
                //}
            }

            Console.WriteLine("Random Impostor Ended");

        }

        async Task YourStateChanged(IClientPlayer player)
        {
            string oldName = player.Character.PlayerInfo.PlayerName;
            Outfit oldOutfit = new Outfit(player.Character.PlayerInfo);

            if (player.Character.PlayerInfo.IsImpostor)
            {
                await _impostorOutfit.DressUpAsync(player , player.Client.Id);
                await SetNameOnlyAsync(player , "Open the map and close it to show the kill button");
            }
            else
            {
                await _crewmateOutfit.DressUpAsync(player, player.Client.Id);
                await SetNameOnlyAsync(player, "Open the map and close it to hide the kill button");

            }
            await Task.Delay(1000 * 3);

            await SetNameOnlyAsync(player, oldName);
            await oldOutfit.DressUpAsync(player, player.Client.Id);
        }
    }
}
