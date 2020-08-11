using Microsoft.Extensions.Configuration;
using Minsky.Helpers;

namespace Minsky.Services
{
    public class ConfigurationService
    {
        private const string Port = "Port";
        private const string Ip = "IP";

        private readonly IConfiguration _configuration;

        public string ServerName => _configuration.GetStrValue("ServerName");
        public string ServerPass => _configuration.GetStrValue("ServerPassword");

        private IConfigurationSection ServerSection => _configuration.GetSection("Server");
        private IConfigurationSection SrsSection => ServerSection.GetSection("Srs");
        private IConfigurationSection DcsSection => ServerSection.GetSection("Dcs");

        public string SrsServer => SrsSection.GetStrValue(Ip);
        public int SrsPort => SrsSection.GetIntValue(Port);

        public string DcsServer => DcsSection.GetStrValue(Ip);
        public int DcsPort => DcsSection.GetIntValue(Port);

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
