using Impostor.Api.Events;
using Impostor.Api.Plugins;
using ZirnoPlugin.Discord;
using ZirnoPlugin.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZirnoPlugin
{
    public class ExamplePluginStartup : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEventListener, GameEventListener>();
            services.AddSingleton<IEventListener, PlayerEventListener>();
            services.AddSingleton<IEventListener, MeetingEventListener>();
            services.AddSingleton<IEventListener, DiscordBot>();
        }
    }
}
