using Discord.Rest;

namespace Minsky.Entities
{
    public sealed class ServerStatus
    {
        public bool DcsOnline { get; private set; }
        public bool SrsOnline { get; private set; }

        public ServerStatus(bool dcsOnline, bool srsOnline)
        {
            DcsOnline = dcsOnline;
            SrsOnline = srsOnline;
        }
    }
}
