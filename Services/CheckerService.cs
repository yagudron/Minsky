using Minsky.Helpers;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Minsky.Services
{
    public class CheckerService
    {
        private readonly ConfigurationService _configService;

        public CheckerService(ConfigurationService configurationService)
        {
            _configService = configurationService;
        }

        public async Task<string> GetStatusMessageAsync()
        {
            var dcsPing = IsDcsOnline();
            var srsPing = IsSrsOnline();
            await Task.WhenAll(dcsPing, srsPing);

            return ComposeStatusMessage(dcsPing.Result, srsPing.Result);
        }

        public async Task<bool> IsDcsOnline()
        {
            return await IsPortOnline(_configService.DcsServer, _configService.DcsPort);
        }

        public async Task<bool> IsSrsOnline()
        {
            return await IsPortOnline(_configService.SrsServer, _configService.SrsPort);
        }

        private static Task<bool> IsPortOnline(string host, int port)
        {
            bool success;
            using var client = new TcpClient();

            var result = client.BeginConnect(host, port, null, null);

            success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
            if (!success)
                return Task.FromResult(false);

            client.EndConnect(result);

            return Task.FromResult(success);
        }

        private static string ComposeStatusMessage(bool isDcsOnline, bool isSrsOnline)
        {
            return string.Format(Resources.ServerStatusMessageTemplate,
                isDcsOnline.StatusToEmoji(), isDcsOnline.StatusToText(),
                isSrsOnline.StatusToEmoji(), isSrsOnline.StatusToText());
        }
    }
}
