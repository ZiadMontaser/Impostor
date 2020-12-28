using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Microsoft.Extensions.DependencyInjection;
using ZirnoPlugin.Standerd;
using ZirnoPlugin.Viresed;
using ZirnoPLugin.RandomImostor;
using ZirnoPLugin.Sherif;

namespace ZirnoPlugin.Gamemode
{
    internal class GamemodeManager
    {
        const Gamemodes DEFAULT_GAMEMODE = Gamemodes.HotPotato;

        private IServiceProvider _serviceProvider;

        public static Dictionary<IGame, Plugin> games = new();

        public GamemodeManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetGamemode(IGame game , Gamemodes gamemodes)
        {
            games[game] = (Plugin)ActivatorUtilities.CreateInstance(_serviceProvider, (gamemodes switch
            {
                Gamemodes.Standerd       => typeof(StanderdPlugin),
                Gamemodes.HotPotato      => typeof(VireusPlugin),
                Gamemodes.RandomImpostor => typeof(RandomImpostor),
                Gamemodes.Sherif         => typeof(Sherif),
                _ => typeof(StanderdPlugin),
            })); ;
            //switch (gamemodes)
            //{
            //    case Gamemodes.Standerd:
            //        games[game] = new StanderdPlugin();
            //        break;
            //    case Gamemodes.HotPotato:
            //        games[game] = new VireusPlugin();
            //        break;
            //}
        }

        public void CreateGame(IGame game)
        {
            games.Add(game, null);
            SetGamemode(game, DEFAULT_GAMEMODE);
        }

        public void DeleteGame(IGame game)
        {
            games.Remove(game);
        }

        public static Gamemodes GetGamemode(IGame game)
        {
            return games[game].mode;
        }

        public static Plugin Get(IGame game)
        {
            return games[game];
        }

    }
}
