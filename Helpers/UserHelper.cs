using Discord.WebSocket;
using System.Linq;

namespace Minsky.Helpers
{
    public static class UserHelper
    {
        public static bool IsUserDevStaff(this SocketUser user, ulong devStaffRoleId)
        {
            var userRoles = user.MutualGuilds?.FirstOrDefault()?.GetUser(user.Id)?.Roles;
            var isDevStaff = userRoles?.Where(r => r.Id == devStaffRoleId)?.Any();
            return isDevStaff == true;
        }
    }
}
