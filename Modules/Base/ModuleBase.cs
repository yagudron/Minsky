using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Minsky.Modules.Base
{
    public class ModuleBase : ModuleBase<SocketCommandContext>
    {
        protected async Task SendMessageAsync(string text, string title = null)
        {
            var builder = new EmbedBuilder
            {
                Title = title,
                Description = text,
                Color = Color.DarkMagenta,
            };

            await Context.Channel.SendMessageAsync(string.Empty, false, builder.Build());
        }
    }
}
