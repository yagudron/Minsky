using System;
using System.Threading.Tasks;
using Discord.Commands;
using Minsky.Entities;
using Minsky.Helpers;
using Minsky.Services;

namespace Minsky.Modules
{
    public class HelpModule : Base.ModuleBase
    {
        private readonly StatusService _checkerService;
        private readonly ConfigurationService _configService;

        public HelpModule(StatusService checkerService, ConfigurationService configurationService)
        {
            _checkerService = checkerService;
            _configService = configurationService;
        }

        [Command("server")]
        [Summary("Get server info")]
        public Task GetServerAsync() => GetServerInfosAsync();

        [Command("status")]
        [Summary("Get servers status")]
        public async Task GetStatusAsync() => await SendMessageAsync(await _checkerService.GetStatusMessageAsync());

        [Command("zulu")]
        [Summary("Get current UTC/ZULU time")]
        public async Task GetZuluTimeAsync() => await GetUniversalTime();

        [Command("utc")]
        [Summary("Get current UTC/ZULU time")]
        public async Task GetUtcTimeAsync() => await GetUniversalTime();

        [Command("help")]
        [Summary("Get help")]
        public Task GetHelpAsync() => GetHelpInternalAsync();

        private async Task GetServerInfosAsync() => await SendMessageAsync($"{GetServerInfo()}");

        private string GetServerInfo()
        {
            var server = _configService.Server;
            return $"**{server.Name}**{Environment.NewLine}" +
                $"`ip:   {server.DcsPort.Ip}:{server.DcsPort.Port}`{Environment.NewLine}" +
                $"`pass: {server.Password}`{Environment.NewLine}" +
                $"`srs:  {server.SrsPort.Ip}:{server.SrsPort.Port}`";
        }

        private async Task GetUniversalTime() => await SendMessageAsync($"UTC/ZULU time - {DateTime.UtcNow:HH:mm}");

        private async Task GetHelpInternalAsync()
        {
            var message = Context.User.IsUserDevStaff(_configService.DevStaffRoleId)
                ? $"{Resources.HelpText}{Environment.NewLine}{Resources.AdminHelp}"
                : $"{Resources.HelpText}.";
            await SendMessageAsync(message, "**I can help!**");
        }
    }
}
