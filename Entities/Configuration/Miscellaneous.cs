using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Entities.Configuration
{
    public sealed class Miscellaneous
    {
        public string EmbedColor { get; private set; }

        public Miscellaneous(IConfigurationSection section)
        {
            EmbedColor = section.GetStrValue(nameof(EmbedColor));
        }
    }
}
