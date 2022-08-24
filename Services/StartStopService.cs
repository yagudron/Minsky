using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Minsky.Modules;

namespace Minsky.Services
{
    public sealed class StartStopService : SlashCommandModuleBase
    {
        private readonly ConfigurationService _configService;

        public StartStopService(ConfigurationService configurationService)
        {
            _configService = configurationService;
        }

        public void StartServer()
        {
            Process.Start(_configService.Server.DcsBinaryLocaion);
            Process.Start(_configService.Server.SrsBinaryLocation);
        }

        public async Task RestartServerAsync()
        {
            StopServer();
            await Task.Delay(1000);
            StartServer();
        }

        public static void StopServer()
        {
            //TODO: Improve to be able to stop non-ob server. (AK)
            var processes = Process.GetProcesses();
            var dcsProcess = processes.Where(p => p.MainWindowTitle == "DCS.openbeta_server").FirstOrDefault();
            dcsProcess?.Kill();
            var srsProcess = processes.Where(p => p.MainWindowTitle.StartsWith("DCS-SRS Server")).FirstOrDefault();
            srsProcess?.Kill();
        }
    }
}
