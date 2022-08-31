using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Minsky.Services;

namespace Minsky.Modules
{
    public sealed class HelpModule : SlashCommandModuleBase
    {
        private readonly StatusService _checkerService;
        private readonly ConfigurationService _configService;

        public HelpModule(StatusService checkerService, ConfigurationService configurationService) : base(configurationService)
        {
            _checkerService = checkerService;
            _configService = configurationService;
        }

        [SlashCommand("server", "Get server info.")]
        public async Task GetServerAsync() => await RespondAsync($"{GetServerInfo()}");

        [SlashCommand("status", "Get servers status.")]
        public async Task GetStatusAsync() => await RespondAsync(await _checkerService.GetStatusMessageAsync());

        private string GetServerInfo()
        {
            var serverInfo = _configService.Server;
            var server = $"**{serverInfo.Name}**";
            var password = !string.IsNullOrEmpty(serverInfo.Password) ? $"`pass: {serverInfo.Password}`{Environment.NewLine}" : string.Empty;
            var ip = $"`ip:   {serverInfo.DcsPort.Ip}:{serverInfo.DcsPort.Port}`";
            var srs = $"`srs:  {serverInfo.SrsPort.Ip}:{serverInfo.SrsPort.Port}`";

            var hasGci = !string.IsNullOrEmpty(serverInfo.GciLinkUri) && !string.IsNullOrEmpty(serverInfo.GciLinkTitle);
            var gci = hasGci ? $"{Environment.NewLine}{Environment.NewLine}[{serverInfo.GciLinkTitle}]({serverInfo.GciLinkUri})" : string.Empty;
            
            return $"{server}{Environment.NewLine}{ip}{Environment.NewLine}{password}{srs}{gci}";
        }
    }
}
