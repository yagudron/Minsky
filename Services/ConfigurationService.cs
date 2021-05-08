using Microsoft.Extensions.Configuration;
using Minsky.Entities;
using Minsky.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Minsky.Services
{
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ulong MasterUserId => ulong.Parse(_configuration.GetStrValue(nameof(MasterUserId)));
        public ulong DevStaffRoleId => ulong.Parse(_configuration.GetStrValue(nameof(DevStaffRoleId)));

        public MainServerConfiguration MainServer => new(_configuration.GetSection(nameof(MainServer)));
        public IEnumerable<ServerConfiguration> AdditionalServers => _configuration.GetSection(nameof(AdditionalServers)).GetChildren().Select(s => new ServerConfiguration(s));

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
