using Microsoft.Extensions.Configuration;
using Minsky.Entities.Enums;
using Minsky.Helpers;

namespace Minsky.Entities
{
    public sealed class ServerConfiguration
    {
        public ServerType Type { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public PortConfig DcsPort { get; private set; }
        public PortConfig SrsPort { get; private set; }

        public ServerConfiguration(IConfigurationSection section)
        {
            Type = (ServerType)section.GetIntValue(nameof(Type));
            Name = section.GetStrValue(nameof(Name));
            Password = section.GetStrValue(nameof(Password));
            DcsPort = new PortConfig(section.GetSection(nameof(DcsPort)));
            SrsPort = new PortConfig(section.GetSection(nameof(SrsPort)));
        }
    }
}
