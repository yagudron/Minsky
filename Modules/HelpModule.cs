using Discord.Interactions;
using Minsky.Entities;
using Minsky.Entities.Integration.Sneaker;
using Minsky.Helpers;
using Minsky.Integaration;
using Minsky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public sealed class HelpModule : SlashCommandModuleBase
    {
        private readonly StatusService _statusService;
        private readonly ConfigurationService _configService;
        private readonly SneakerApiClient _sneakerApiClient;

        public HelpModule(
            StatusService statusService,
            ConfigurationService configurationService,
            SneakerApiClient sneakerApiClient) : base(configurationService)
        {
            _statusService = statusService;
            _configService = configurationService;
            _sneakerApiClient = sneakerApiClient;
        }

        [SlashCommand("server", "Get server info.")]
        public async Task GetServerAsync2()
        {
            var sneakerInfos = await _sneakerApiClient.GetServerInfosAsync(_configService.SneakerApiAdress);
            var serverConfigs = _configService.Servers.OrderByDescending(c => c.Type).ToList();
            var embedBuilder = GetDefaultEmbedBuilder();

            foreach (var serverConfig in serverConfigs)
            {
                var sneakerInfo = GetServerSneakerInfo(serverConfig, sneakerInfos);
                var portStatus = await _statusService.GetServerStatusAsync(serverConfig);

                var serverName = !string.IsNullOrEmpty(sneakerInfo.name) ? $"**{sneakerInfo.name}**" : $"**{serverConfig.Name}**";
                var ip = $"{portStatus.DcsOnline.StatusToEmoji()} **DSC:**   {serverConfig.DcsPort.Ip}:{serverConfig.DcsPort.Port}";
                var srs = $"{portStatus.SrsOnline.StatusToEmoji()} **SRS:**  {serverConfig.SrsPort.Ip}:{serverConfig.SrsPort.Port}";
                var password = !string.IsNullOrEmpty(serverConfig.Password) ? $"**PASS:** {serverConfig.Password}{Environment.NewLine}" : string.Empty;
                var players = ComposeGetPlayerList(sneakerInfo);

                embedBuilder.AddField(serverName, $"{ip}{Environment.NewLine}{password}{srs}{players}{Environment.NewLine}");
            }

            var hasGci = !string.IsNullOrEmpty(_configService.GciLinkUri) && !string.IsNullOrEmpty(_configService.GciLinkTitle);
            if (hasGci)
                embedBuilder.AddField("GCI", $"[{_configService.GciLinkTitle}]({_configService.GciLinkUri})");

            await RespondAsync(embedBuilder);
        }

        private ServerInfoContract GetServerSneakerInfo(ServerConfiguration serverConfig, IEnumerable<ServerInfoContract> infos)
        {
            var serverType = serverConfig.Type;
            if (serverType == Entities.Enums.ServerType.Pvp)
                return infos.FirstOrDefault(i => i.name.ToLowerInvariant().Contains(serverType.ToString().ToLowerInvariant()));
            else if (serverType == Entities.Enums.ServerType.Pve)
                return infos.FirstOrDefault(i => i.name.ToLowerInvariant().Contains(serverType.ToString().ToLowerInvariant()));

            throw new InvalidOperationException("Server configs are not matching API response.");
        }

        private static string ComposeGetPlayerList(ServerInfoContract sneakerInfo)
        {
            if (sneakerInfo?.players == null)
                return string.Empty;

            var result = $"{Environment.NewLine}{Environment.NewLine}**Players:**";
            if (!sneakerInfo.players.Any())
                return $"{result}{Environment.NewLine}No active players.";
            else
                sneakerInfo.players.ForEach(p => result += $"{Environment.NewLine}{p.name} ({p.type})");

            return result;
        }
    }
}
