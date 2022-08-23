using Microsoft.Extensions.Configuration;
using Minsky.Entities;
using Minsky.Helpers;

namespace Minsky.Services
{
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ulong MasterUserId => ulong.Parse(_configuration.GetStrValue(nameof(MasterUserId)));
        public ulong DevStaffRoleId => ulong.Parse(_configuration.GetStrValue(nameof(DevStaffRoleId)));

        public ServerConfiguration Server => new(_configuration.GetSection(nameof(Server)));
        
        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
