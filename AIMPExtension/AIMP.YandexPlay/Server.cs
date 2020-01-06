using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AIMP.YandexPlay
{
    internal class Server : IDisposable
    {
        private TcpListener _tcpListener;
        private bool _isListening;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public readonly List<WebSocketSession> Clients = new List<WebSocketSession>();

        public event EventHandler<WebSocketSession> ClientConnected;

        public event EventHandler<WebSocketSession> ClientDisconnected;

        public void Start()
        {
            _logger.Info("Server start");

            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, Properties.Settings.Default.Port);
                _tcpListener.Start();
                _isListening = true;
                Listen();
                _logger.Info("Server started");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetBaseException(), "Error while starting server");
            }
        }

        public void Dispose()
        {
            _isListening = false;
        }

        private void Listen()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (_isListening)
                {
                    var session = new WebSocketSession(_tcpListener.AcceptTcpClient());
                    session.HandshakeCompleted += (__, ___) =>
                    {
                        Console.WriteLine($"{session.Id}| Handshake Valid.");
                        Clients.Add(session);
                    };

                    session.Disconnected += (__, ___) =>
                    {
                        Console.WriteLine($"{session.Id}| Disconnected.");
                        Clients.Remove(session);

                        ClientDisconnected?.Invoke(this, session);
                        session.Dispose();
                    };

                    Console.WriteLine($"{session.Id}| Connected.");
                    ClientConnected?.Invoke(this, session);
                    session.Start();
                }

                _tcpListener.Stop();
            });
        }
    }
}
