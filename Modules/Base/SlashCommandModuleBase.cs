using Discord;
using Discord.Interactions;
using Minsky.Services;
using System.Drawing;
using System.Threading.Tasks;

namespace Minsky.Modules
{
    public class SlashCommandModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ConfigurationService _configService;

        public SlashCommandModuleBase(ConfigurationService configurationService)
        {
            _configService = configurationService;
        }

        protected async Task RespondAsync(string text, string title = default)
        {
            var embedColor = (System.Drawing.Color)new ColorConverter().ConvertFromString(_configService.Miscellaneous.EmbedColor);
            var builder = new EmbedBuilder
            {
                Title = title,
                Description = text,
                Color = new Discord.Color(embedColor.R, embedColor.G, embedColor.B),
            };

            await Context.Interaction.RespondAsync(embed: builder.Build());
        }
    }
}
