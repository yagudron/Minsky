using Discord.Commands;
using Discord.WebSocket;
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
            if (!(messageParam is SocketUserMessage message))
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
            _ = RunCommand(message, argPos, context);
        }

        private async Task RunCommand(SocketUserMessage message, int argPos, SocketCommandContext context)
        {
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess || result.Error == CommandError.UnknownCommand)
                await message.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
