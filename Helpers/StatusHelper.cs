namespace Minsky.Helpers
{
    public static class StatusHelper
    {
        public static string StatusToText(this bool isDcsOnline)
        {
            return isDcsOnline ? Resources.OnlineMessage : Resources.OfflineMessage;
        }

        public static string StatusToEmoji(this bool isDcsOnline)
        {
            return isDcsOnline ? Resources.OnlineEmoji : Resources.OfflineEmoji;
        }
    }
}
