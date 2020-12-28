using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ZirnoPlugin.Viresed;
using ZirnoPlugin;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Events.Player;
using Impostor.Api.Events;
using Impostor.Api.Net;
using ZirnoPlugin.Gamemode;

namespace ZirnoPLugin.Sherif
{
    class Sherif : Plugin
    {
        private readonly Outfit _outfit;

        private readonly Random _random;
        private readonly ILogger<Sherif> _logger;

        IClientPlayer sherif;

        public Sherif(ILogger<Sherif> logger ,  Random random)
        {
            mode = Gamemodes.Sherif;
            _random = random;
            _logger = logger;
            _outfit = new Outfit(HatType.TenGallonHat, ColorType.Yellow, SkinType.Astro, PetType.NoPet);
        }

        public override void OnGameStarted(IGameStartedEvent e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(30 * 1000);
                List<IClientPlayer> impostors = e.Game.Players.Where((x)=>x.Character.PlayerInfo.IsImpostor).ToList();
                _logger.LogDebug("{0} is the Impostor", impostors[0].Character.PlayerInfo.PlayerName);

                var crewmates = e.Game.Players.Where((x) => !x.Character.PlayerInfo.IsImpostor).ToList();

                sherif =  crewmates[_random.Next(crewmates.Count())];
                _logger.LogDebug("{0} is the Sherif Yahoo" , sherif.Character.PlayerInfo.PlayerName);

                foreach(var player in e.Game.Players)
                {
                    if(player == sherif)
                    {
                        player.Character.SetImpostor();
                        _logger.LogDebug("Hi Sherif Again i am {0}", player.Character.PlayerInfo.IsImpostor);
                    }
                    else
                    {
                        player.Character.SetCrewmate();
                    }
                }
                    //await Task.Delay(5 * 1000);
                foreach (var palyer in e.Game.Players) _logger.LogDebug("{0} is {1}" , palyer.Character.PlayerInfo.PlayerName ,palyer.Character.PlayerInfo.IsImpostor);
                await e.Game.GameNet.GameData.UpdateGameDataAsync(sherif.Character.OwnerId);
                foreach(var impostor in impostors)
                {
                    impostor.Character.SetImpostor();
                }
                    foreach (var palyer in e.Game.Players) _logger.LogDebug("{0} is {1}", palyer.Character.PlayerInfo.PlayerName, palyer.Character.PlayerInfo.IsImpostor);


                     await YouAreSherifAsync(sherif);
            });

        }

        public override void OnPlayerMurder(IPlayerMurderEvent e)
        {
            Task.Run(async () =>
            {

                if (e.ClientPlayer == sherif)
                {
                    if (e.Victim.PlayerInfo.IsImpostor)
                    {
                        await e.Victim.SetMurderedByAsync(e.Game.GetClientPlayer(e.Victim.OwnerId));
                        _logger.LogDebug("Good Job Sherif You Got Him");
                    }
                    else
                    {

                        await e.Game.GameNet.GameData.UpdateGameDataAsync();
                        await e.Victim.SetMurderedByAsync(e.Game.GetClientPlayer(e.Victim.OwnerId));
                        await sherif.Character.SetMurderedByAsync(e.Game.GetClientPlayer(e.Victim.OwnerId));

                        sherif.Character.SetCrewmate(); // Set Sherif To Crewmate State to Crewmate POV
                        await e.Game.GameNet.GameData.UpdateGameDataAsync();

                        sherif.Character.SetImpostor(); // Set Sherif To Impostor in his POV
                        await e.Game.GameNet.GameData.UpdateGameDataAsync(sherif.Client.Id);

                        _logger.LogDebug("Good bye Sherif Try Again Later");
                    }
                }
                else
                {
                    _logger.LogDebug("Calm Down Normal Kill");
                }
            });
        }

        async Task YouAreSherifAsync(IClientPlayer player)
        {
            string oldName = player.Character.PlayerInfo.PlayerName;
            Outfit oldOutfit = new Outfit(player.Character.PlayerInfo);

            await _outfit.DressUpAsync(player , player.Client.Id);
            await SetNameOnlyAsync(player ,"Yahoo Your the Sherif");

            await Task.Delay(1000 * 3);
            
            await SetNameOnlyAsync(player, "Your gool is to murder the impostor");

            await Task.Delay(1000 * 3);

            //await player.Character.SetNameAsync(oldName);
            //await oldOutfit.DressUpAsync(player);
        }
    }
}
