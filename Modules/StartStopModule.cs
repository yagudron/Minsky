﻿using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Minsky.Helpers;
using Minsky.Services;

namespace Minsky.Modules
{
    public sealed class StartStopModule : SlashCommandModuleBase
    {
        private readonly StatusService _checkerService;
        private readonly ConfigurationService _configService;
        private readonly StartStopService _startStopService;

        public StartStopModule(StatusService checkerService, ConfigurationService configurationService, StartStopService startStopService)
            : base(configurationService)
        {
            _checkerService = checkerService;
            _startStopService = startStopService;
            _configService = configurationService;
        }

        [SlashCommand("start", "Start server.")]
        public async Task StartServer() => await StartInternalAsync();

        [SlashCommand("restart", "Restart server.")]
        public async Task RestartServer() => await RestartInternalAsync();

        [SlashCommand("stop", "Stop server")]
        public async Task StopServer() => await StopInternalAsync();

        private async Task StartInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await RespondAsync(Resources.CantComplyMessage);
                return;
            }
            var dcsOnline = await StatusService.IsDcsOnline(_configService.Server);
            var srsOnline = await StatusService.IsSrsOnline(_configService.Server);
            if (dcsOnline && srsOnline)
            {
                await RespondAsync($"{Resources.AllRunning} {Resources.PleaseUseRestart}");
                return;
            }
            else if (dcsOnline || srsOnline)
            {
                await RespondAsync($"{string.Format(Resources.SingleRunningTemplate, dcsOnline ? "DCS" : "SRS")} {Resources.PleaseUseRestart}");
                return;
            }

            try
            {
                await RespondAsync(Resources.StartingMessage);
                _startStopService.StartServer();
            }
            catch (Exception)
            {
                await RespondAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            //NOTE: Wait for the server to start. (AK)
            var repliedStatus = false;
            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(5000);

                if (await StatusService.IsDcsOnline(_configService.Server) && await StatusService.IsSrsOnline(_configService.Server))
                {
                    repliedStatus = true;
                    var statusMessage = await _checkerService.GetServerStatusMessageAsync(_configService.Server);
                    await RespondAsync($"{statusMessage}", Resources.StartedMessage);
                    break;
                }
            }

            if (!repliedStatus)
            {
                await Task.Delay(5000);
                var statusMessage = await _checkerService.GetServerStatusMessageAsync(_configService.Server);
                await RespondAsync($"{statusMessage}", Resources.StartedMessage);
            }
        }

        private async Task RestartInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await RespondAsync(Resources.CantComplyMessage);
                return;
            }

            try
            {
                await RespondAsync(Resources.RestartingMessage);
                await _startStopService.RestartServerAsync();
            }
            catch (Exception)
            {
                await ReplyAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await Task.Delay(50000);
            await RespondAsync(Resources.RestartedMessage);
            await RespondAsync(await _checkerService.GetServerStatusMessageAsync(_configService.Server));
        }

        private async Task StopInternalAsync()
        {
            if (!Context.User.IsUserDevStaff(_configService.DevStaffRoleId))
            {
                await RespondAsync(Resources.CantComplyMessage);
                return;
            }
            var dcsOnline = await StatusService.IsDcsOnline(_configService.Server);
            var srsOnline = await StatusService.IsSrsOnline(_configService.Server);
            if (!dcsOnline && !srsOnline)
            {
                await RespondAsync(Resources.NothingToStopMessage);
                return;
            }

            try
            {
                await RespondAsync(Resources.StoppingMessage);
                StartStopService.StopServer();
            }
            catch (Exception)
            {
                await RespondAsync(Resources.SomethingWentWrongMessage);
                return;
            }

            await RespondAsync(Resources.StoppedMessage);
        }
    }
}
