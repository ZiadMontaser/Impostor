using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Plugins.Example.Standerd;

namespace Impostor.Plugins.Example.Gamemode
{
    static class GamemodeManager
    {
        const Gamemodes DEFAULT_GAMEMODE = Gamemodes.Standerd;

        public static Dictionary<IGame, Plugin> games;

        static GamemodeManager()
        {
            games = new Dictionary<IGame, Plugin>();
        }

        public static void SetGamemode(IGame game , Gamemodes gamemodes)
        {
            switch (gamemodes)
            {
                case Gamemodes.Standerd:
                    games[game] = new StanderdPlugin();
                    break;
                case Gamemodes.HotPotato:
                    games[game] = new VireusPlugin();
                    break;
            }
        }

        public static void CreateGame(IGame game)
        {
            games.Add(game, null);
            SetGamemode(game, DEFAULT_GAMEMODE);
        }

        public static void DeleteGame(IGame game)
        {
            games.Remove(game);
        }
    }
}
