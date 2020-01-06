using System;

namespace AIMP.YandexPlay
{
    internal class MessageQueue
    {
        private static MessageQueue _instance;

        public static MessageQueue Instance => _instance ?? (_instance = new MessageQueue());

        public event EventHandler<string> TrackChanged;

        public void RaiseTrackChanged(string link)
        {
            this.TrackChanged?.Invoke(this, link);
        }
    }
}
