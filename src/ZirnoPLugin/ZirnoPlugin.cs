using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace ZirnoPlugin
{
    [ImpostorPlugin(
        package: "ZirnoPlugin",
        name: "ZirnoServerPlugin",
        author: "Ziad",
        version: "1.0.0")]
    public class ZirnoPlugin : PluginBase
    {
        private readonly ILogger<ZirnoPlugin> _logger;

        public ZirnoPlugin(ILogger<ZirnoPlugin> logger)
        {
            _logger = logger;
        }

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");
            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            return default;
        }
    }
}
