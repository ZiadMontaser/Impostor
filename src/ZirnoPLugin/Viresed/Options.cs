using System;
using Impostor.Api.Net;
using ZirnoPlugin.Commands;
using ZirnoPlugin.Gamemode;

namespace ZirnoPlugin.Viresed
{
    [Command]
    class CRound : ICommand
    {
        public string Names => "round";

        public string Discription => "Set round long";

        public void excute(IClientPlayer player, string[] param)
        {
            Plugin plugin = GamemodeManager.Get(player.Game);
            if (plugin.GetType() == typeof(VireusPlugin))
            {
                VireusPlugin hotPotao = ((VireusPlugin)plugin);
                if (int.TryParse(param[1], out int seconds))
                {
                    Console.WriteLine(seconds);
                    hotPotao.RoundLong = seconds;
                    CommandPlugin.ReplayToAll(player.Game, $"{Colors.DarkGrey}The {plugin.mode} round is now {hotPotao.RoundLong}s long.");
                }
                else
                {
                    CommandPlugin.Reply(player, $"{Colors.Red}Error:{Colors.DarkGrey} Please enter a valid number");
                }
            }
            else
            {
                CommandPlugin.Reply(player, $"{Colors.Red}Error:{Colors.DarkGrey} You have to change gamemode to {Gamemode.Gamemodes.HotPotato} firstly");
            }
        }
    }
}
