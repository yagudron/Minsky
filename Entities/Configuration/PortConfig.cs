using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Entities
{
    public sealed class PortConfig
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        public PortConfig(IConfigurationSection section)
        {
            Ip = section.GetStrValue(nameof(Ip));
            Port = section.GetIntValue(nameof(Port));
        }
    }
}