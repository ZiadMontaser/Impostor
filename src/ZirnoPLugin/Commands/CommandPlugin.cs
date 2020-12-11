using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Customization;
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
            uint server;
            string oldName;
            byte oldColor;

            if (player.IsHost && player.Game.PlayerCount > 1)
            {
                var serverClient = player.Game.Players.ToList()[1];
                server = serverClient.Character.NetId;
                oldName = serverClient.Character.PlayerInfo.PlayerName;
                oldColor = serverClient.Character.PlayerInfo.ColorId;
            }
            else
            {
                server = player.Game.Host.Character.NetId;
                oldName = player.Game.Host.Character.PlayerInfo.PlayerName;
                oldColor = player.Game.Host.Character.PlayerInfo.ColorId;
            }

            using (var w = MessageWriter.Get(MessageType.Reliable))
            {
                w.StartMessage(MessageFlags.GameData);
                w.Write(player.Game.Code);

                StartRpc(w, server, (byte)RpcCalls.SetName);
                w.Write($"{Colors.Red}Server");
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SetColor);
                w.Write((byte)ColorType.Red);
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SendChat);
                w.Write(message);
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SetName);
                w.Write(oldName);
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SetColor);
                w.Write(oldColor);
                EndRpc(w);

                w.EndMessage();

                player.Game.SendToAsync(w , player.Client.Id);
            }
        }

        static void StartRpc(IMessageWriter writer, uint senderId, byte rpcFlag)
        {
            writer.StartMessage(GameDataTag.RpcFlag);
            writer.WritePacked(senderId);
            writer.Write((byte)rpcFlag);
        }

        static void EndRpc(IMessageWriter writer) => writer.EndMessage();

        public static void ReplayToAll(IGame game, string message)
        {
            var server = game.Host.Character.NetId;
            var oldName = game.Host.Character.PlayerInfo.PlayerName;
            var oldColor = game.Host.Character.PlayerInfo.ColorId;

            using (var w = MessageWriter.Get(MessageType.Reliable))
            {
                w.StartMessage(MessageFlags.GameData);
                w.Write(game.Code);

                StartRpc(w, server, (byte)RpcCalls.SetName);
                w.Write($"{Colors.Red}Server");
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SetColor);
                w.Write((byte)ColorType.Red);
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SendChat);
                w.Write(message);
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SetName);
                w.Write(oldName);
                EndRpc(w);

                StartRpc(w, server, (byte)RpcCalls.SetColor);
                w.Write(oldColor);
                EndRpc(w);

                w.EndMessage();

                game.SendToAllExceptAsync(w ,game.Host.Client.Id);
            }
            Reply(game.Host , message);
        }

    }
}
