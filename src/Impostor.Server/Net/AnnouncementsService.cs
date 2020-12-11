using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Server.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net
{
    internal class AnnouncementsService : IHostedService
    {
        private readonly ILogger<AnnouncementsService> _logger;
        private readonly ServerConfig _serverConfig;
        private readonly ServerRedirectorConfig _redirectorConfig;
        private readonly Announcements _announcements;

        public AnnouncementsService(
            ILogger<AnnouncementsService> logger,
            IOptions<ServerConfig> serverConfig,
            IOptions<ServerRedirectorConfig> redirectorConfig,
            Announcements matchmaker)
        {
            _logger = logger;
            _serverConfig = serverConfig.Value;
            _redirectorConfig = redirectorConfig.Value;
            _announcements = matchmaker;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var endpoint = new IPEndPoint(IPAddress.Any, 22024);

            await _announcements.StartAsync(endpoint);

            _logger.LogInformation(
                "Announcements is listening on {0}:{1}, the public server ip is {2}:{3}.",
                endpoint.Address,
                endpoint.Port,
                _serverConfig.ResolvePublicIp(),
                _serverConfig.PublicPort);

            if (_redirectorConfig.Enabled)
            {
                _logger.LogWarning(_redirectorConfig.Master
                    ? "Server redirection is enabled as master, this instance will redirect clients to other nodes."
                    : "Server redirection is enabled as node, this instance will accept clients.");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Announcements is shutting down!");
            await _announcements.StopAsync();
        }
    }
}
