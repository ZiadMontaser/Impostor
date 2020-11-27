using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Plugins.Example.Commands;

namespace Impostor.Plugins.Example.Gamemode
{
    [Command]
    class CGamemode : ICommand
    {
        public string Names => "gamemode";

        public string Discription => "Changes Gamemode using: /gamemode <gamemode>";

        public void excute(IClientPlayer player, string[] param)
        {
            if(!player.IsHost)
            {
                CommandPlugin.Reply(player, "[FF0000]Error: [a9a9a9] Your not allowd to do that. ");
                return;
            }
            if(player.Game.GameState == GameStates.Started)
            {
                CommandPlugin.Reply(player, "[FF0000]Error: [a9a9a9] Your Cant change Gamemode mid game.");
                return;
            }

            if (param.Length < 2)
            {
                CommandPlugin.Reply(player, GetAllGamemodes());
            }
            else if(int.TryParse(param[1] , out int g))
            {
                if(g < Enum.GetValues(typeof(Gamemodes)).Length)
                {
                    GamemodeManager.SetGamemode(player.Game, (Gamemodes)g);
                    return;
                }
            }
            else if(Enum.TryParse(param[1] , out Gamemodes ga))
            {
                GamemodeManager.SetGamemode(player.Game, ga);
                return;
            }
            else
            {
                CommandPlugin.Reply(player, "[FF0000]Error: [a9a9a9] Pleas enter valid data ");
                return;
            }
            CommandPlugin.Reply(player, "[FF0000]Error: [a9a9a9] Pleas enter valid data ");
        }

        string GetAllGamemodes()
        {
            var builder = new StringBuilder();
            builder.Append("Avilible Gamemodes");
            for(int i = 0; i < Enum.GetValues(typeof(Gamemodes)).Length; i++)
            {
                var gamemode = (Gamemodes)i;
                builder.Append($"{i}- {gamemode} \n");
            }
            return builder.ToString();
        }
    }
}
