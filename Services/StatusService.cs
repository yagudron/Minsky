using Minsky.Entities;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Minsky.Services
{
    public class StatusService
    {
        public static async Task<ServerStatus> GetServerStatusAsync(ServerConfiguration server)
        {
            var dcsPing = IsPortOnline(server.DcsPort.Ip, server.DcsPort.Port);
            var srsPing = IsPortOnline(server.SrsPort.Ip, server.SrsPort.Port);
            await Task.WhenAll(dcsPing, srsPing);

            return new ServerStatus(dcsPing.Result, srsPing.Result);
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
    }
}