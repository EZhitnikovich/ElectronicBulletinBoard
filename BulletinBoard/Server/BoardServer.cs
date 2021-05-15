using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinBoard.Server
{
    public class BoardServer
    {
        public delegate void BoardServerHandler(string message);
        public event BoardServerHandler Log;

        private TcpListener _listener;
        private readonly IPEndPoint _ipEndPoint;
        protected internal CancellationTokenSource _cancellationToken;

        public BoardServer(IPAddress ipAddress, int port)
        {
            _ipEndPoint = new IPEndPoint(ipAddress, port);
        }

        public BoardServer(string hostName, int port)
        {
            _ipEndPoint = new IPEndPoint(Dns.GetHostAddresses(hostName).FirstOrDefault() ?? IPAddress.Any, port);
        }

        public void StartServer()
        {
            if(_listener != null)
                return;
            
            OnLog("Starting server...\n");
            _listener = new TcpListener(_ipEndPoint);
            _cancellationToken = new CancellationTokenSource();
            _listener.Start();
            OnLog("Server started.\n");

            Task.Run(StartListenerLoop);
        }

        public void StopServer()
        {
            if(_listener == null)
                return;
            
            _cancellationToken.Cancel();
            Disconnect();
            OnLog("Server stopped.\n");
        }

        private void StartListenerLoop()
        {
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    var client = _listener.AcceptTcpClient();
                    OnLog($"{client.Client.RemoteEndPoint} connected.\n");
                    var clientObject = new ClientObject(client, this);
                    clientObject.Log += OnLog;
                    var clientTread = new Thread(clientObject.Process);
                    clientTread.Start();
                }
            }
            catch (Exception e)
            {
                OnLog(e.Message + "\n");
            }
            finally
            {
                Disconnect();
            }
        }

        private void Disconnect()
        {
            if (_listener != null)
            {
                _listener.Stop();
                OnLog("Server disconnected.\n");
            }

            _listener = null;
        }

        protected virtual void OnLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}
