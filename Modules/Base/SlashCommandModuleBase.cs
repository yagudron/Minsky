﻿using System.Threading.Tasks;
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
                Color = new Color(252, 68, 92),
            };

            await Context.Interaction.RespondAsync(embed: builder.Build());
        }
    }
}