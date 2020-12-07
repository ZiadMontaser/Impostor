using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.API;
using Discord.Rest;
using Impostor.Api.Events;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using ZirnoPlugin.Handlers;

namespace ZirnoPlugin.Discord
{
    class DiscordBot : IEventListener
    {
        const string TOKEN = "NzgxNDAxODM5MDY5Mjk4NzA4.X79HLA.emIi-FEhJHBzStOy45aYvIVVnHw";

        private readonly ILogger<PlayerEventListener> _logger;

        DiscordSocketClient client;
        SocketGuild server;

        public DiscordBot(ILogger<PlayerEventListener> logger)
        {
            _logger = logger;
            TaskAsync();
        }

        async Task TaskAsync()
        {
            client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, TOKEN, true);
            await client.StartAsync();

            client.Log += Log;
            client.MessageReceived += OnMessageReceived;
            client.Ready += OntReady;
        }

        Task Log(LogMessage log)
        {
            Console.WriteLine($"Discord Bot : {log.Message}");
            return Task.CompletedTask;
        }

        async Task OntReady()
        {
            server = client.GetGuild(779606708916846592);
        }

        async Task OnMessageReceived(SocketMessage message)
        {
            //client.

            if (message.Content.StartsWith('!'))
            {
                var param = message.Content.Substring(1).Split(' ');
                switch (param[0])
                {
                    case "join":
                        await JoinAsync(message.Author.Id, param);
                        break;
                }
            }
            return;
        }

        async Task JoinAsync(ulong userId , string[] pram)
        {
            SocketGuildChannel channal = server.Channels.ToList().Find((x) => x.Name.ToLower() == pram[1].ToLower());
            if (channal == null)
            {
                var restChannel = await server.CreateVoiceChannelAsync(pram[1].ToUpper(), SetSettings);
                channal = server.GetVoiceChannel(restChannel.Id);
            }
            
            await server.GetUser(userId).ModifyAsync((x) => x.ChannelId = channal.Id);

        }

        void SetSettings(VoiceChannelProperties properties)
        {
            properties.UserLimit = 10;
            properties.CategoryId = 779606708916846596;
        }
    }
}
