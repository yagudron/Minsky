using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Minsky.Entities;
using Minsky.Helpers;

namespace Minsky.Services
{
    public class StatusService
    {
        private readonly ConfigurationService _configService;

        public StatusService(ConfigurationService configurationService)
        {
            _configService = configurationService;
        }

        public static async Task<bool> IsDcsOnline(ServerConfiguration server) =>
            await IsPortOnline(server.DcsPort.Ip, server.DcsPort.Port);

        public static async Task<bool> IsSrsOnline(ServerConfiguration server) =>
            await IsPortOnline(server.SrsPort.Ip, server.SrsPort.Port);

        public async Task<string> GetStatusMessageAsync() =>
            $"{await GetServerStatusMessageAsync(_configService.Server)}";

        public async Task<string> GetServerStatusMessageAsync(ServerConfiguration server)
        {
            var dcsPing = IsDcsOnline(server);
            var srsPing = IsSrsOnline(server);
            await Task.WhenAll(dcsPing, srsPing);

            return ComposeStatusMessage(server.Name, dcsPing.Result, srsPing.Result);
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

        private static string ComposeStatusMessage(string serverName, bool isDcsOnline, bool isSrsOnline)
        {
            return $"**{serverName}**{Environment.NewLine}" +
                string.Format(Resources.ServerStatusMessageTemplate,
                isDcsOnline.StatusToEmoji(), isDcsOnline.StatusToText(),
                isSrsOnline.StatusToEmoji(), isSrsOnline.StatusToText());
        }
    }
}