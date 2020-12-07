using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Server.Net.Inner;

namespace ZirnoPlugin.Commands
{
    class CommandPlugin : Plugin
    {
        List<ICommand> commands = new List<ICommand>();

        public CommandPlugin() {
        
            var asmbly = Assembly.GetExecutingAssembly();
            var typeList = asmbly.GetTypes().Where(
                    t => t.GetCustomAttributes(typeof(CommandAttribute), true).Length > 0
            ).ToList();
            foreach (Type type in typeList)
            {
                ICommand command = (ICommand)Activator.CreateInstance(type);
                commands.Add(command);
            }
            Console.WriteLine("Commands Lodded");
        }

        public void OnPLayerChat(IPlayerChatEvent e)
        {

            if (!e.Message.StartsWith('/')) return;

            var param = e.Message.Substring(1).Split(' ');
            
            foreach(var command in commands)
            {
                if (command.Names.ToLower() != param[0].ToLower()) continue;

                command.excute(e.ClientPlayer, param);
            }
        }

        public static void Reply(IClientPlayer player , string message)
        {
            var server = player.Game.Host.Character.NetId;
            var oldName = player.Character.PlayerInfo.PlayerName;

            if (player.IsHost && player.Game.PlayerCount > 1)
            {
                var serverClient = player.Game.Players.ToList()[1];
                server = serverClient.Character.NetId;
                oldName = serverClient.Character.PlayerInfo.PlayerName;
            }
            else
            {
                server = player.Game.Host.Character.NetId;
                oldName = player.Character.PlayerInfo.PlayerName;
            }

            using(var w = MessageWriter.Get(MessageType.Reliable))
            {
                w.StartMessage(MessageFlags.GameData);
                w.Write(player.Game.Code);

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(server);
                w.Write((byte)RpcCalls.SetName);
                w.Write($"{Colors.Red}Server");
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(server);
                w.Write((byte)RpcCalls.SendChat);
                w.Write(message);
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(server);
                w.Write((byte)RpcCalls.SetName);
                w.Write(oldName);
                w.EndMessage();

                w.EndMessage();

                player.Game.SendToAsync(w , player.Client.Id);
            }
        }

        public static void ReplayToAll(IGame game, string message)
        {
            var server = game.Host.Character.NetId;
            var oldName = game.Host.Character.PlayerInfo.PlayerName;

            using (var w = MessageWriter.Get(MessageType.Reliable))
            {
                w.StartMessage(MessageFlags.GameData);
                w.Write(game.Code);

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(server);
                w.Write((byte)RpcCalls.SetName);
                w.Write($"{Colors.Red}Server");
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(server);
                w.Write((byte)RpcCalls.SendChat);
                w.Write(message);
                w.EndMessage();

                w.StartMessage(GameDataTag.RpcFlag);
                w.WritePacked(server);
                w.Write((byte)RpcCalls.SetName);
                w.Write(oldName);
                w.EndMessage();

                w.EndMessage();

                game.SendToAllExceptAsync(w ,game.Host.Client.Id);
            }
            Reply(game.Host , message);
        }

    }
}
