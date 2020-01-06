using NLog;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AIMP.YandexPlay
{
    internal class TrackHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public TrackHandler()
        {
            MessageQueue.Instance.TrackChanged += Instance_TrackChanged;
        }

        public event EventHandler<string> FileDownloaded;

        private void Instance_TrackChanged(object sender, string e)
        {
            _logger.Info($"Track changed: {e}");
            var tempDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "YandexMusic");
            var path = Path.Combine(
                tempDir,
                $"{Guid.NewGuid()}.mp3");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            var client = new HttpClient();
            client.GetAsync(e).ContinueWith(t =>
            {
                var response = t.Result;
                response.EnsureSuccessStatusCode();

                response.Content.ReadAsStreamAsync().ContinueWith(tr =>
                {
                    using (var file = File.Create(path))
                    {
                        tr.Result.CopyTo(file);
                        this.FileDownloaded?.Invoke(this, path);
                        _logger.Info($"Track downloaded to: {path}");
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
}
