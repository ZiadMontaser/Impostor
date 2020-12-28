using Impostor.Api.Events;
using Impostor.Api.Plugins;
using ZirnoPlugin.Discord;
using ZirnoPlugin.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZirnoPlugin.Commands;
using ZirnoPlugin.Gamemode;
using System;

namespace ZirnoPlugin
{
    public class MainStartup : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Random>();
            services.AddSingleton<GamemodeManager>();
            services.AddSingleton<CommandPlugin>();

            services.AddSingleton<IEventListener, GameEventListener>();
            services.AddSingleton<IEventListener, PlayerEventListener>();
            services.AddSingleton<IEventListener, MeetingEventListener>();
            services.AddSingleton<IEventListener, DiscordBot>();
        }
    }
}
