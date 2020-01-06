using AIMP.SDK;
using AIMP.SDK.Playlist;
using NLog;

namespace AIMP.YandexPlay
{
    [AimpPlugin(name: "YandexPlay", author: "Avalan4er", version: "0.0.1", AimpPluginType = AimpPluginType.Addons)]
    public class Plugin : AimpPlugin, IAimpExtension
    {
        private readonly Server _server;
        private readonly TrackHandler _trackHandler;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Plugin()
        {
            _logger.Info("Plugin start");
            _server = new Server();
            _trackHandler = new TrackHandler();
            _trackHandler.FileDownloaded += _trackHandler_FileDownloaded;
        }

        private void _trackHandler_FileDownloaded(object sender, string e)
        {
            var actionResult = this.Player.PlaylistManager.GetLoadedPlaylistByName("Yandex", out var playlist);
            if (playlist == null)
            {
                var actionResult2 = this.Player.PlaylistManager.CreatePlaylist("Yandex", true, out playlist);
            }

            playlist.Add(e, SDK.Playlist.PlaylistFlags.NOCHECKFORMAT, SDK.Playlist.PlaylistFilePosition.EndPosition);
            var item = playlist.GetItem(playlist.GetItemCount() - 1);
            this.Player.Play(item);
        }

        public override void Dispose()
        {
            _logger.Info("Plugin dispose");
            _server.Dispose();
        }

        public override void Initialize()
        {
            _logger.Info("Plugin initialize");

            var listener = new ExtensionPlaylistManagerListener();
            var result = Player.Core.RegisterExtension(listener);

            _server.Start();
            _server.ClientConnected += (sender, session) =>
            {
                session.TextMessageReceived += (s, text) =>
                {
                    MessageQueue.Instance.RaiseTrackChanged(text);
                };
            };
        }
    }

    public class ExtensionPlaylistManagerListener : IAimpExtension, IAimpExtensionPlaylistManagerListener
    {
        public AimpActionResult OnPlaylistActivated(IAimpPlaylist playlist)
        {
            return AimpActionResult.Ok;
        }

        public AimpActionResult OnPlaylistAdded(IAimpPlaylist playlist)
        {
            return AimpActionResult.Ok;
        }

        public AimpActionResult OnPlaylistRemoved(IAimpPlaylist playlist)
        {
            return AimpActionResult.Ok;
        }
    }
}
