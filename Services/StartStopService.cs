using System.Diagnostics;
using System.Linq;
using System.Threading;
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
            Process.Start(_configService.DcsBinaryLocation);
            Process.Start(_configService.SrsBinaryLocation);
        }

        public async Task RestartServerAsync()
        {
            StopServer();
            await Task.Delay(1000);
            StartServer();
        }

        public void StopServer()
        {
            //TODO: Improve to be able to stop non-ob server. (AL)
            var processes = Process.GetProcesses();
            var dcsProcess = processes.Where(x => x.MainWindowTitle == "DCS.openbeta_server").FirstOrDefault();
            dcsProcess?.Kill();
            var srsProcess = processes.Where(x => x.MainWindowTitle.StartsWith("DCS-SRS Server")).FirstOrDefault();
            srsProcess?.Kill();
        }
    }
}
