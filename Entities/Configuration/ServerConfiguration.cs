using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Entities
{
    public sealed class ServerConfiguration
    {
        public string Name { get; private set; }
        public string Password { get; private set; }
        public PortConfig DcsPort { get; private set; }
        public PortConfig SrsPort { get; private set; }

        public bool IsOpenBeta { get; private set; }

        public string DcsLogsLocation { get; private set; }
        public string DcsBinaryLocaion { get; private set; }
        public string GciLinkTitle { get; private set; }
        public string GciLinkUri { get; private set; }

        public string SrsBinaryLocation { get; private set; }

        public ServerConfiguration(IConfigurationSection section)
        {
            Name = section.GetStrValue(nameof(Name));
            Password = section.GetStrValue(nameof(Password));
            DcsPort = new PortConfig(section.GetSection(nameof(DcsPort)));
            SrsPort = new PortConfig(section.GetSection(nameof(SrsPort)));
            DcsLogsLocation = section.GetStrValue(nameof(DcsLogsLocation));
            DcsBinaryLocaion = section.GetStrValue(nameof(DcsBinaryLocaion));
            SrsBinaryLocation = section.GetStrValue(nameof(SrsBinaryLocation));
            GciLinkUri = section.GetStrValue(nameof(GciLinkUri));
            GciLinkTitle = section.GetStrValue(nameof(GciLinkTitle));
            IsOpenBeta = section.GetBoolValue(nameof(IsOpenBeta));
        }
    }
}
