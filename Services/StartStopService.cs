using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Minsky.Services
{
    public class StartStopService
    {
        private readonly ConfigurationService _configService;

        public StartStopService(ConfigurationService configurationService)
        {
            _configService = configurationService;
        }

        public void StartServer()
        {
            Process.Start(_configService.MainServer.DcsBinaryLocaion);
            Process.Start(_configService.MainServer.SrsBinaryLocation);
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
            var dcsProcess = processes.Where(x => x.MainWindowTitle == "DCS.openbeta_server").FirstOrDefault();
            dcsProcess?.Kill();
            var srsProcess = processes.Where(x => x.MainWindowTitle.StartsWith("DCS-SRS Server")).FirstOrDefault();
            srsProcess?.Kill();
        }
    }
}
