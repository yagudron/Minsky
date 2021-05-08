using Discord.Commands;
using Discord.WebSocket;
using Minsky.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Minsky.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider  services)
        {
            _commands = commands;
            _client = client;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message)
                return;

            var argPos = 0;
            var isValidCall = message.HasCharPrefix('?', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos);
            if (!isValidCall || message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
#if DEBUG
            var configurationService = _services.GetService(typeof(ConfigurationService)) as ConfigurationService;
            var isMasterUser = context.User.Id == configurationService?.MasterUserId;
            if (!isMasterUser)
            {
                await message.Channel.SendMessageAsync(Resources.WontComplyMessage);
                return;
            }
#endif
            _ = RunCommandAsync(message, argPos, context);
        }

        private async Task RunCommandAsync(SocketUserMessage message, int argPos, SocketCommandContext context)
        {
            if (message.Content.Length == 1)
                return;

            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
                await message.Channel.SendMessageAsync(Resources.UknownCommandMessage);
            else if (!result.IsSuccess)
                await message.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
