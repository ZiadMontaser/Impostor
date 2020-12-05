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
    class CGm : CGamemode
    {
        public override string Names => "gm";
    }

    [Command]
    class CGamemode : ICommand
    {
        public virtual string Names => "gamemode";

        public string Discription => "Changes Gamemode using: /gamemode <gamemode>";

        public void excute(IClientPlayer player, string[] param)
        {
            if(!player.IsHost)
            {
                CommandPlugin.Reply(player, $"{Colors.Red}Error: {Colors.DarkGrey} Your not allowd to do that. ");
                return;
            }
            if(player.Game.GameState == GameStates.Started)
            {
                CommandPlugin.Reply(player, $"{Colors.Red}Error: {Colors.DarkGrey} Your Cant change Gamemode mid game.");
                return;
            }

            if (param.Length < 2)
            {
                CommandPlugin.Reply(player, GetAllGamemodes());
                return;
            }
            else if(int.TryParse(param[1] , out int g))
            {
                g--;
                if(g < Enum.GetValues(typeof(Gamemodes)).Length)
                {
                    GamemodeManager.SetGamemode(player.Game, (Gamemodes)g);
                    onGamemodeChange(player);
                    return;
                }
            }
            else if(Enum.TryParse(param[1] , out Gamemodes ga))
            {
                GamemodeManager.SetGamemode(player.Game, ga);
                onGamemodeChange(player);
                return;
            }
            else
            {
                CommandPlugin.Reply(player, $"{Colors.Red}Error: {Colors.DarkGrey} Pleas enter valid data ");
                return;
            }
            CommandPlugin.Reply(player, $"{Colors.Red}Error: {Colors.DarkGrey} Pleas enter valid data ");
        }

        void onGamemodeChange(IClientPlayer player)
        {
            CommandPlugin.ReplayToAll(player.Game, $"{Colors.DarkGrey}Gamemode Changed to {Colors.Green}{GamemodeManager.GetGamemode(player.Game)}{Colors.DarkGrey} .");
        }

        string GetAllGamemodes()
        {
            var builder = new StringBuilder();
            builder.Append("Avilible Gamemodes \n");
            for(int i = 0; i < Enum.GetValues(typeof(Gamemodes)).Length; i++)
            {
                var gamemode = (Gamemodes)i;
                builder.Append($"{i+1}- {gamemode} \n");
            }
            return builder.ToString();
        }
    }
}
