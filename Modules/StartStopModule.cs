using Discord.Commands;
using Minsky.Helpers;
using Minsky.Services;
using System;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public class StartStopModule : ModuleBase<SocketCommandContext>
    {
        private readonly CheckerService _checkerService;
        private readonly ConfigurationService _configService;
        private readonly StartStopService _startStopService;

        public StartStopModule(CheckerService checkerService, ConfigurationService configurationService, StartStopService watchDogService)
        {
            _checkerService = checkerService;
            _startStopService = watchDogService;
            _configService = configurationService;
        }

        [Command("start")]
        [Summary("Start server")]
        public async Task StartServer() => await StartInternalAsync();

        [Command("restart")]
        [Summary("Restart server")]
        public async Task RestartServer() => await RestartInternalAsync();

        [Command("stop")]
        [Summary("Stop server")]
        public async Task StopServer() => await StopInternalAsync();

        private async Task StartInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await ReplyAsync(Resources.CantComplyMessage);
                return;
            }
            var dcsOnline = await _checkerService.IsDcsOnline();
            var srsOnline = await _checkerService.IsSrsOnline();
            if (dcsOnline && srsOnline)
            {
                await ReplyAsync($"{Resources.AllRunning} {Resources.PleaseUseRestart}");
                return;
            }
            else if (dcsOnline || srsOnline)
            {
                await ReplyAsync($"{string.Format(Resources.SingleRunningTemplate, dcsOnline ? "DCS" : "SRS")} {Resources.PleaseUseRestart}");
                return;
            }

            try
            {
                await ReplyAsync(Resources.StartingMessage);
                _startStopService.StartServer();
            }
            catch (Exception)
            {
                await ReplyAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await Task.Delay(16000);
            await ReplyAsync(Resources.StartedMessage);
            await ReplyAsync(await _checkerService.GetStatusMessageAsync());
        }

        private async Task RestartInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await ReplyAsync(Resources.CantComplyMessage);
                return;
            }

            try
            {
                await ReplyAsync(Resources.RestartingMessage);
                await _startStopService.RestartServerAsync();
            }
            catch (Exception)
            {
                await ReplyAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await Task.Delay(16000);
            await ReplyAsync(Resources.RestartedMessage);
            await ReplyAsync(await _checkerService.GetStatusMessageAsync());
        }

        private async Task StopInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await ReplyAsync(Resources.CantComplyMessage);
                return;
            }
            var dcsOnline = await _checkerService.IsDcsOnline();
            var srsOnline = await _checkerService.IsSrsOnline();
            if (!dcsOnline && !srsOnline)
            {
                await ReplyAsync(Resources.NothingToStopMessage);
                return;
            }

            try
            {
                await ReplyAsync(Resources.StoppingMessage);
                _startStopService.StopServer();
            }
            catch (Exception)
            {
                await ReplyAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await ReplyAsync(Resources.StoppedMessage);
        }
    }
}
