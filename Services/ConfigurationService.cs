using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Services
{
    public class ConfigurationService
    {
        private const string Port = "Port";
        private const string Ip = "IP";
        private const string BinaryLocation = "BinaryLocation";

        private readonly IConfiguration _configuration;

        public ulong MasterUserId => ulong.Parse(_configuration.GetStrValue("MasterUserId"));
        public ulong DevStaffRoleId => ulong.Parse(_configuration.GetStrValue("DevStaffRoleId"));

        private IConfigurationSection ServerSection => _configuration.GetSection("Server");
        public string ServerName => ServerSection.GetStrValue("Name");
        public string ServerPass => ServerSection.GetStrValue("Password");

        private IConfigurationSection SrsSection => ServerSection.GetSection("Srs");
        public string SrsServer => SrsSection.GetStrValue(Ip);
        public int SrsPort => SrsSection.GetIntValue(Port);
        public string SrsBinaryLocation => SrsSection.GetStrValue(BinaryLocation);

        private IConfigurationSection DcsSection => ServerSection.GetSection("Dcs");
        public string DcsServer => DcsSection.GetStrValue(Ip);
        public int DcsPort => DcsSection.GetIntValue(Port);
        public string DcsBinaryLocation => DcsSection.GetStrValue(BinaryLocation);

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
