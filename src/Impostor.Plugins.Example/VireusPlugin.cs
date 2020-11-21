using System;
using System.Collections.Generic;
using System.Text;
using Impostor.Api.Net;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Events.Player;
using Impostor.Api.Net.Inner.Objects;
using System.Threading.Tasks;
using System.Threading;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using System.Numerics;
using System.Linq;

namespace Impostor.Plugins.Example
{
    class VireusPlugin
    {
        const int UPDATE_RATE = 100;
        const int ROUND_LONG = 60 * 1000;

        private readonly Random Random;
        private readonly Outfit viresdOutfit;

        IClientPlayer viresed;

        Outfit oldOutfit;

        Thread updateThread;

        bool isInCoolDown = false;

        int timer = 0;

        public VireusPlugin(Random random)
        {
            Random = random;
            viresdOutfit = new Outfit(HatType.Plague, ColorType.Yellow, SkinType.Tarmac, PetType.NoPet);
        }

        public async Task SetVirusedAsync(IClientPlayer player)
        {
            //restore old player
            if(viresed != null) {
                await oldOutfit.DressUpAsync(viresed);
            }
            //set up the new player
            viresed = player;
            oldOutfit = new Outfit(player.Character.PlayerInfo);
            await viresdOutfit.DressUpAsync(player);
            //start cool down
            StartCoolDown(3);
        }

        public void OnStartGame(IGame game)
        {
            SetVirusedAsync(GetRandomAlivePLayer(game));

            updateThread = new Thread(async()=> await UpdateAsync(game));

            updateThread.Start();
        }

        public void OnPLayerJoin(IPlayerSpawnedEvent e)
        {
            e.Game.Host.Character.SendChatAsync("Welcome");
        }

        public void OnPlayerChat(IPlayerChatEvent e)
        {
            viresed.Character.SetMurderedAsync();
        }

        private async Task UpdateAsync(IGame game)
        {
            Console.WriteLine("Update thread started");
            while(game.GameState == GameStates.Started)
            {
                if (!isInCoolDown)
                {
                    Vector2 viresedPos = viresed.Character.NetworkTransform.TargetPosition;
                    //check for collosin
                    foreach (var player in game.Players)
                    {
                        var c = player.Character;
                        if (c.PlayerInfo.IsDead || c.NetId == viresed.Character.NetId) continue;

                        if (Vector2.Distance(viresedPos, c.NetworkTransform.TargetPosition) < 0.66f)
                        {
                            
                            await SetVirusedAsync(player);
                            break;
                        }
                    }
                }

                timer += UPDATE_RATE;
                if(timer >= ROUND_LONG)
                {
                    await viresed.Character.SetMurderedAsync();
                    await SetVirusedAsync(GetNearestAlivePLayer(game));
                    timer = 0;
                }

                Console.WriteLine("Thread Compleated a loob");
                Thread.Sleep(UPDATE_RATE);
            }
            Console.WriteLine("Update thread finished");
        }

        public void StartCoolDown(int seconds)
        {
            Task.Run(async ()=> {
                isInCoolDown = true;
                await Task.Delay(seconds * 1000);
                isInCoolDown = false;
            });
        }

        private IClientPlayer GetRandomAlivePLayer(IGame game)
        {
            var alivePlayers = game.Players.ToList();
            alivePlayers.ForEach((item) => {
                if (item.Character.PlayerInfo.IsDead) alivePlayers.Remove(item);
            });
            return alivePlayers[Random.Next(alivePlayers.Count)];
        }

        private IClientPlayer GetNearestAlivePLayer(IGame game)
        {
            IClientPlayer player = null;
            var viresPos = viresed.Character.NetworkTransform.TargetPosition;
            var leastDis = 100f;
            foreach (var p in game.Players)
            {
                if (p.Character.PlayerInfo.IsDead) continue;
                float playerDistance = Vector2.Distance(viresPos, p.Character.NetworkTransform.TargetPosition);
                if (leastDis > playerDistance)
                {
                    leastDis = playerDistance;
                    player = p;
                }
            }
            return player;
        }
    }
}
