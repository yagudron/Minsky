using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Interactions;
using Minsky.Entities.Integration.Sneaker;
using Minsky.Helpers;
using Minsky.Integaration;
using Minsky.Services;

namespace Minsky.Modules
{
    public sealed class HelpModule : SlashCommandModuleBase
    {
        private readonly StatusService _statusService;
        private readonly ConfigurationService _configService;

        public HelpModule(StatusService statusService, ConfigurationService configurationService) : base(configurationService)
        {
            _statusService = statusService;
            _configService = configurationService;
        }

        [SlashCommand("server", "Get server info.")]
        public async Task GetServerAsync()
        {
            var sneakerApiClient = new SneakerApiClient();

            var sneakerInfo = sneakerApiClient.GetServerInfoAsync();
            var serverStaus = _statusService.GetServerStatusAsync(_configService.Server);
            await Task.WhenAll(sneakerInfo, serverStaus);

            var serverInfo = _configService.Server;
            var serverName = !string.IsNullOrEmpty(sneakerInfo.Result?.name) ? $"**{sneakerInfo?.Result.name}**" : $"**{serverInfo.Name}**";
            var ip = $"{serverStaus.Result.dcsOnline.StatusToEmoji()} **DSC:**   {serverInfo.DcsPort.Ip}:{serverInfo.DcsPort.Port}";
            var srs = $"{serverStaus.Result.srsOnline.StatusToEmoji()} **SRS:**  {serverInfo.SrsPort.Ip}:{serverInfo.SrsPort.Port}";
            var password = !string.IsNullOrEmpty(serverInfo.Password) ? $"**PASS:** {serverInfo.Password}{Environment.NewLine}" : string.Empty;

            var hasGci = !string.IsNullOrEmpty(serverInfo.GciLinkUri) && !string.IsNullOrEmpty(serverInfo.GciLinkTitle);
            var gci = hasGci ? $"{Environment.NewLine}{Environment.NewLine}[{serverInfo.GciLinkTitle}]({serverInfo.GciLinkUri})" : string.Empty;
            var players = ComposeGetPlayerList(sneakerInfo.Result);

            var result = $"{serverName}{Environment.NewLine}{ip}{Environment.NewLine}{password}{srs}{gci}{players}";
            await RespondAsync(result);
        }

        private static string ComposeGetPlayerList(ServerInfoContract sneakerInfo)
        {
            if (sneakerInfo?.players == null)
                return string.Empty;

            var result = $"{Environment.NewLine}{Environment.NewLine}**Players:**";
            if (!sneakerInfo.players.Any())
                return $"{result}{Environment.NewLine}No active players.";
            else
                sneakerInfo.players.ForEach(p => result = result + $"{Environment.NewLine}{p.name} ({p.type})");

            return result;
        }
    }
}
