using Discord;
using Discord.Commands;
using Minsky.Helpers;
using Minsky.Services;
using System;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public class HelpModule : Base.ModuleBase
    {
        private readonly CheckerService _checkerService;
        private readonly ConfigurationService _configService;

        public HelpModule(CheckerService checkerService, ConfigurationService configurationService)
        {
            _checkerService = checkerService;
            _configService = configurationService;
        }

        [Command("server")]
        [Summary("Get server info")]
        public Task GetServerAsync() => GetServerInfoAsync();

        private async Task GetServerInfoAsync()
        {
            await SendMessageAsync(
                $"`ip:   {_configService.DcsServer}:{_configService.DcsPort}`{Environment.NewLine}" +
                $"`pass: {_configService.ServerPass}`{Environment.NewLine}" +
                $"`srs:  {_configService.SrsServer}:{_configService.SrsPort}`",
                _configService.ServerName);
        }

        [Command("status")]
        [Summary("Get server status")]
        public async Task GetStatusAsync() => await SendMessageAsync(await _checkerService.GetStatusMessageAsync());

        [Command("zulu")]
        [Summary("Get current ZULU time")]
        public async Task GetZuluTimeAsync() => await GetUniversalTime();

        [Command("utc")]
        [Summary("Get current UTC time")]
        public async Task GetUtcTimeAsync() => await GetUniversalTime();

        [Command("help")]
        [Summary("Get help")]
        public Task GetHelpAsync() => GetHelpInternalAsync();

        private async Task GetUniversalTime()
        {
            await SendMessageAsync($"UTC/ZULU time - {DateTime.UtcNow:HH:mm}");
        }

        private async Task GetHelpInternalAsync()
        {
            var message = Context.User.IsUserDevStaff(_configService.DevStaffRoleId)
                ? $"{Resources.HelpText}{Environment.NewLine}{Resources.AdminHelp}"
                : $"{Resources.HelpText}.";
            await SendMessageAsync(message, "**I can help!**");
        }
    }
}
