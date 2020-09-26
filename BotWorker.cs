using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minsky.Handlers;
using Minsky.Services;

namespace Minsky
{
    public class BotWorker : BackgroundService
    {
        #region readonly properties
        
        private readonly ILogger<BotWorker> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        
        #endregion
        
        private bool _initialized;

        public BotWorker(ILogger<BotWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(provider => configuration)
                .AddSingleton<CheckerService>()
                .AddSingleton<ConfigurationService>()
                .AddSingleton<StartStopService>()
                .BuildServiceProvider();

            _client.Log += Log;
            _commands.Log += Log;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_initialized)
            {
                var token = _configuration.GetSection("Auth").GetValue<string>("DiscordToken");
                await InitClient(token);
                await InitCommandHanler();

                _initialized = true;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(-1);
            }
        }

        private async Task InitClient(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        private async Task InitCommandHanler()
        {
            var handler = new CommandHandler(_client, _commands, _services);
            await handler.InitializeAsync();
        }

        private Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    _logger.LogError($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    _logger.LogDebug($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
                    break;
                default:
                    _logger.LogInformation($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
