using Microsoft.Extensions.Configuration;

namespace Minsky.Modules
{
    public static class ConfigurationHelper
    {
        public static string GetStrValue(this IConfiguration section, string value) => section.GetValue<string>(value);

        public static int GetIntValue(this IConfiguration section, string value) => section.GetValue<int>(value);
    }
}
