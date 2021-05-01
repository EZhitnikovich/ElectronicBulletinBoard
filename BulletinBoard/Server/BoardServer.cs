using System;
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

        public readonly int MaxBacklog = 100;

        private readonly IPEndPoint _serverEndPoint;
        private Socket _listenSocket;

        private CancellationTokenSource _cancellationToken;
        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);

        public BoardServer(IPAddress address, int port)
        {
            _serverEndPoint = new IPEndPoint(address, port);
        }

        public void StartServer()
        {
            OnLog("Server starting...\n");
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(_serverEndPoint);
            _listenSocket.Listen(MaxBacklog);
            _cancellationToken = new CancellationTokenSource();
            OnLog("Server started.\n");

            Task.Run(StartServerLoop);
        }

        private void StartServerLoop()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _allDone.Reset();

                _listenSocket.BeginAccept(AcceptCallback, _listenSocket);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _allDone.Set();

            var listener = (Socket) ar.AsyncState;
            var handler = listener?.EndAccept(ar);
            var state = new StateObject {workSocket = handler};

            handler?.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                ReceiveCallback, state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var content = string.Empty;

            var state = (StateObject)ar.AsyncState;
            var handler = state?.workSocket;

            var bytesRead = 0;
            if (handler != null)
            {
                bytesRead = handler.EndReceive(ar);
            }

            if (bytesRead > 0)
            {
                state?.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

            }
        }

        public void StopServer()
        {
            _cancellationToken.Cancel();
            _listenSocket.Close();
            OnLog("Server stopped\n");
        }

        private void OnLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}