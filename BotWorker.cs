using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minsky.Handlers;
using Minsky.Integaration;
using Minsky.Services;

namespace Minsky
{
    public class BotWorker : BackgroundService
    {
        private readonly ILogger<BotWorker> _logger;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        private bool _initialized;

        public BotWorker(ILogger<BotWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _services = new ServiceCollection()
                .AddSingleton(provider => configuration)
                .AddSingleton<StatusService>()
                .AddSingleton<ConfigurationService>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(_ => new InteractionService(_.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .AddTransient<SneakerApiClient>()
                .BuildServiceProvider();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_initialized)
            {
                var token = _configuration.GetSection("Auth").GetValue<string>("DiscordToken");

                var handler = _services.GetRequiredService<InteractionHandler>();
                await handler.InitializeAsync();

                var client = _services.GetRequiredService<DiscordSocketClient>();
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                client.Log += OnLogAsync;

                _initialized = true;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(-1, stoppingToken);
            }
        }

        private Task OnLogAsync(LogMessage message)
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
