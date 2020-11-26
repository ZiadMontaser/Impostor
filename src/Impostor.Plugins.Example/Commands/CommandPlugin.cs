using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;

namespace Impostor.Plugins.Example.Commands
{
    class CommandPlugin
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

        [EventListener]
        public void OnPLayerChat(IPlayerChatEvent e)
        {

            if (!e.Message.StartsWith('/')) return;

            var param = e.Message.Substring(1).Split(' ');
            
            foreach(var command in commands)
            {
                if (command.Names != param[0]) continue;

                command.excute(e.ClientPlayer, param);
            }
        }

    }
}
