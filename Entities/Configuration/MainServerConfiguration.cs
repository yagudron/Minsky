using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Entities
{
    public sealed class MainServerConfiguration : ServerConfiguration
    {
        public bool IsOpenBeta { get; private set; }

        public string DcsLogsLocation { get; private set; }
        public string DcsBinaryLocaion { get; private set; }

        public string SrsBinaryLocation { get; private set; }

        public MainServerConfiguration(IConfigurationSection section) : base(section)
        {
            DcsLogsLocation = section.GetStrValue(nameof(DcsLogsLocation));
            DcsBinaryLocaion = section.GetStrValue(nameof(DcsBinaryLocaion));
            SrsBinaryLocation = section.GetStrValue(nameof(SrsBinaryLocation));
            IsOpenBeta = section.GetBoolValue(nameof(IsOpenBeta));
        }
    }
}
