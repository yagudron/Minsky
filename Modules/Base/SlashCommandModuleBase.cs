using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Minsky.Modules
{
    public class SlashCommandModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected async Task RespondAsync(string text, string title = default)
        {
            var builder = new EmbedBuilder
            {
                Title = title,
                Description = text,
                Color = Color.DarkMagenta,
            };

            await Context.Interaction.RespondAsync(embed: builder.Build());
        }
    }
}
