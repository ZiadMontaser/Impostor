using System;
using System.Collections.Generic;
using Impostor.Api.Net;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Events.Player;
using Impostor.Api.Events;
using System.Threading.Tasks;
using System.Threading;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using System.Numerics;
using System.Linq;

namespace Impostor.Plugins.Example
{
    class VireusPlugin : Plugin
    {
        const int UPDATE_RATE = 100;
        const int ROUND_LONG = 20 * 1000;

        private readonly Random Random;
        private readonly Outfit viresdOutfit;
        private readonly Outfit deadPlayerOutfit;

        IClientPlayer viresed;

        List<IClientPlayer> eleminatedPlyers = new List<IClientPlayer>();

        Outfit oldOutfit;

        Thread updateThread;

        bool isInCoolDown = false;

        int timer = 0;

        public VireusPlugin()
        {
            Random = new Random();
            viresdOutfit = new Outfit(HatType.Plague, ColorType.Yellow, SkinType.Tarmac, PetType.NoPet);
            deadPlayerOutfit = new Outfit(HatType.NoHat , ColorType.Black , SkinType.None , PetType.NoPet);
        }

        public async Task SetVirusedAsync(IClientPlayer player)
        {
            //restore old player
            if(viresed != null && !eleminatedPlyers.Contains(viresed)) {
                await oldOutfit.DressUpAsync(viresed);
            }
            if (viresed != null) SetCrewmatOptions(viresed);
            //set up the new player
            viresed = player;
            oldOutfit = new Outfit(player.Character.PlayerInfo);
            await viresdOutfit.DressUpAsync(player);
            SetViresedOptions(player);
            //start cool down
            StartCoolDown(3);
        }

        public override void OnGameStarted(IGameStartedEvent e)
        {
            SetVirusedAsync(GetRandomAlivePLayer(e.Game));

            updateThread = new Thread(async () => await UpdateAsync(e.Game));

            updateThread.Start();
        }

        //public override void OnPlayerLeft(IGamePlayerLeftEvent e)
        //{
        //    if ( == viresed)
        //    {
        //        var next = GetNearestAlivePLayer(player.Game);
        //        if (next == null)
        //        {
        //            next = GetRandomAlivePLayer(player.Game);
        //        }

        //         SetVirusedAsync(next);
        //    }
        //}
        public override void OnGameEnded(IGameEndedEvent e)
        {
            eleminatedPlyers.Clear();
            viresed = null;
            isInCoolDown = false;
            timer = 0;
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
                        if (isDead(player)) continue;

                        if (Vector2.Distance(viresedPos, c.NetworkTransform.TargetPosition) < 0.66f)
                        {
                            await SetVirusedAsync(player);
                            break;
                        }
                    }
                }

                timer += UPDATE_RATE;
                if(timer % 5000 == 0) Console.WriteLine(timer);
                if(timer >= ROUND_LONG)
                {
                    await KillPLayerAsync(viresed);
                    if(ShouldEndGame(game))
                    {
                        await EndGameAsync(game);
                    }
                    else
                    {
                        await SetVirusedAsync(GetNearestAlivePLayer(game));
                    } 
                    timer = 0;
                }

                Thread.Sleep(UPDATE_RATE);
            }
        }

        public void StartCoolDown(int seconds)
        {
            Task.Run(async ()=> {
                isInCoolDown = true;
                await Task.Delay(seconds * 1000);
                isInCoolDown = false;
            });
        }

        private bool ShouldEndGame(IGame game)
        {
            int count = 0;
            foreach (var p in game.Players)
            {
                if (isDead(p)) continue;
                count++;
            }
            if (count <= 1) return true;
            return false;
        }

        private async Task EndGameAsync(IGame game) {
            await Task.Delay(1 * 1000);
            foreach(var p in game.Players)
            {
                await p.Character.SetMurderedAsync();
            }
        }

        private async Task KillPLayerAsync(IClientPlayer player)
        {
            //await player.Character.NetworkTransform.SnapToAsync(new Vector2(1, 1));
            eleminatedPlyers.Add(player);
            await deadPlayerOutfit.DressUpAsync(player);
        }

        private IClientPlayer GetRandomAlivePLayer(IGame game)
        {
            foreach(var p in game.Players)
            {
                if (p.Character.PlayerInfo.IsImpostor) return p;
            }
            var alivePlayers = game.Players.ToList();
            alivePlayers.ForEach((p) => {
                var c = p.Character;
                if (c.PlayerInfo.IsDead || eleminatedPlyers.Contains(p)) alivePlayers.Remove(p);
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
                if (isDead(p))
                {
                    continue;
                }
                float playerDistance = Vector2.Distance(viresPos, p.Character.NetworkTransform.TargetPosition);
                if (playerDistance < leastDis)
                {
                    leastDis = playerDistance;
                    player = p;
                }
            }
            return player;
        }

        private bool isDead(IClientPlayer pLayer)
        {
            var c = pLayer.Character;
            return c.PlayerInfo.IsDead || c.NetId == viresed.Character.NetId || eleminatedPlyers.Contains(pLayer);
        }
    
        private void SetViresedOptions(IClientPlayer player)
        {
            player.Game.Options.IsDefaults = false;
            player.Game.Options.PlayerSpeedMod = 1f;
            player.Game.Options.CrewLightMod = 0.30f;
            player.Game.Options.ImpostorLightMod = 0.30f;

            player.Game.SyncSettingsAsync(player.Client.Id);
        }

        private void SetCrewmatOptions(IClientPlayer player)
        {
            player.Game.Options.IsDefaults = false;
            player.Game.Options.PlayerSpeedMod = 1.25f;
            player.Game.Options.CrewLightMod = 0.75f;
            player.Game.Options.ImpostorLightMod = 0.75f;

            player.Game.SyncSettingsAsync(player.Client.Id);
        }

        public void SetGameOptions(IGame game)
        {
            game.Options.IsDefaults = false;
            game.Options.EmergencyCooldown = int.MaxValue;
            game.Options.NumEmergencyMeetings = 0;
            game.Options.NumCommonTasks = 2;
            game.Options.NumLongTasks = 3;
            game.Options.NumShortTasks = 5;
            game.Options.KillCooldown = float.MaxValue;
            game.Options.PlayerSpeedMod = 1.5f;
            game.Options.VotingTime = 1;
            game.Options.DiscussionTime = 0;
            game.SyncSettingsAsync();
        }
    }
}
