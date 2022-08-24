using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Minsky.Handlers
{
    public sealed class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;

        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
        {
            _client = client;
            _handler = handler;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            _client.Ready += ReadyAsync;
            _handler.Log += LogAsync;

            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _client.InteractionCreated += OnInteractionCreatedAsync;
        }

        private async Task LogAsync(LogMessage log) => Console.WriteLine(log);//fix

        private async Task ReadyAsync()
        {
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(new ApplicationCommandProperties[] { });
            await _handler.RegisterCommandsGloballyAsync(true);
        }

        private async Task OnInteractionCreatedAsync(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_client, interaction);
                var result = await _handler.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
