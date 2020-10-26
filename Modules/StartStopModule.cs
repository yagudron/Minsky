using Discord.Commands;
using Minsky.Helpers;
using Minsky.Services;
using System;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public class StartStopModule : Base.ModuleBase
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
                await SendMessageAsync(Resources.CantComplyMessage);
                return;
            }
            var dcsOnline = await _checkerService.IsDcsOnline();
            var srsOnline = await _checkerService.IsSrsOnline();
            if (dcsOnline && srsOnline)
            {
                await SendMessageAsync($"{Resources.AllRunning} {Resources.PleaseUseRestart}");
                return;
            }
            else if (dcsOnline || srsOnline)
            {
                await SendMessageAsync($"{string.Format(Resources.SingleRunningTemplate, dcsOnline ? "DCS" : "SRS")} {Resources.PleaseUseRestart}");
                return;
            }

            try
            {
                await SendMessageAsync(Resources.StartingMessage);
                _startStopService.StartServer();
            }
            catch (Exception)
            {
                await SendMessageAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            //NOTE: Wait for the server to start. (AK)
            var repliedStatus = false;
            for (var i = 0; i < 3; i++)
            {
                await Task.Delay(6000);

                if (await _checkerService.IsDcsOnline() && await _checkerService.IsSrsOnline())
                {
                    repliedStatus = true;
                    var statusMessage = await _checkerService.GetStatusMessageAsync();
                    await SendMessageAsync($"{statusMessage}", Resources.StartedMessage);
                }
            }

            if (!repliedStatus)
            {
                await Task.Delay(4000);
                var statusMessage = await _checkerService.GetStatusMessageAsync();
                await SendMessageAsync($"{statusMessage}", Resources.StartedMessage);
            }
        }

        private async Task RestartInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await SendMessageAsync(Resources.CantComplyMessage);
                return;
            }

            try
            {
                await SendMessageAsync(Resources.RestartingMessage);
                await _startStopService.RestartServerAsync();
            }
            catch (Exception)
            {
                await ReplyAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await Task.Delay(16000);
            await SendMessageAsync(Resources.RestartedMessage);
            await SendMessageAsync(await _checkerService.GetStatusMessageAsync());
        }

        private async Task StopInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await SendMessageAsync(Resources.CantComplyMessage);
                return;
            }
            var dcsOnline = await _checkerService.IsDcsOnline();
            var srsOnline = await _checkerService.IsSrsOnline();
            if (!dcsOnline && !srsOnline)
            {
                await SendMessageAsync(Resources.NothingToStopMessage);
                return;
            }

            try
            {
                await SendMessageAsync(Resources.StoppingMessage);
                _startStopService.StopServer();
            }
            catch (Exception)
            {
                await SendMessageAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await SendMessageAsync(Resources.StoppedMessage);
        }
    }
}
