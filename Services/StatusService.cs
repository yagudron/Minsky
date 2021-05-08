using Minsky.Entities;
using Minsky.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Minsky.Services
{
    public class StatusService
    {
        private readonly ConfigurationService _configService;

        public StatusService(ConfigurationService configurationService)
        {
            _configService = configurationService;
        }

        public async Task<bool> IsDcsOnline(ServerConfiguration server)
        {
            return await IsPortOnline(server.DcsPort.Ip, server.DcsPort.Port);
        }

        public async Task<bool> IsSrsOnline(ServerConfiguration server)
        {
            return await IsPortOnline(server.SrsPort.Ip, server.SrsPort.Port);
        }

        public async Task<string> GetStatusMessageAsync()
        {
            return $"{await GetServerStatusMessageAsync(_configService.MainServer)}" +
                $"{Environment.NewLine}{Environment.NewLine}" +
                $"{await GetAddititionalServerStatusesAsync(_configService.AdditionalServers)}";
        }

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

        private async Task<string> GetAddititionalServerStatusesAsync(IEnumerable<ServerConfiguration> additionalServers)
        {
            var tasks = additionalServers.Select(s => GetServerStatusMessageAsync(s));
            await Task.WhenAll(tasks);
            return string.Join(Environment.NewLine, tasks.Select(t => t.Result));
        }
    }
}