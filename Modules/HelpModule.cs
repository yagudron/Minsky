using Discord.Commands;
using Minsky.Services;
using System;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CheckerService _checkerService;
        private readonly ConfigurationService _configService;

        public HelpModule(CheckerService checkerService, ConfigurationService configurationService)
        {
            _checkerService = checkerService;
            _configService = configurationService;
        }

        [Command("server")]
        [Summary("Get server address")]
        public Task GetServerAsync() => ReplyAsync($"{_configService.ServerName}{Environment.NewLine}`{_configService.DcsServer}:{_configService.DcsPort}`");

        [Command("pass")]
        [Summary("Get server password")]
        public Task GetPasswordAsync() => ReplyAsync($"`{_configService.ServerPass}`");

        [Command("srs")]
        [Summary("Get SRS address")]
        public Task GetSrsAsync() => ReplyAsync($"`{_configService.SrsServer}:{_configService.SrsPort}`");
        
        [Command("status")]
        [Summary("Get server status")]
        public async Task GetStatusAsync() => await ReplyAsync(await _checkerService.GetStatusMessage());

        [Command("help")]
        [Summary("Get help")]
        public Task GetHelpAsync() => ReplyAsync(Resources.HelpText);
    }
}
