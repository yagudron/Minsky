using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Entities
{
    public sealed class ServerConfiguration
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Password { get; private set; }
        public PortConfig DcsPort { get; private set; }
        public PortConfig SrsPort { get; private set; }

        public ServerConfiguration(IConfigurationSection section)
        {
            Name = section.GetStrValue(nameof(Name));
            DisplayName = section.GetStrValue(nameof(DisplayName));
            Password = section.GetStrValue(nameof(Password));
            DcsPort = new PortConfig(section.GetSection(nameof(DcsPort)));
            SrsPort = new PortConfig(section.GetSection(nameof(SrsPort)));
        }
    }
}
