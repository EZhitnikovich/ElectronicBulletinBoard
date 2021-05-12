using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BulletinBoard.Model;
using CSC = BulletinBoard.Model.ClientServerConfig;

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
        private ManualResetEvent _allDone = new ManualResetEvent(false);

        public BoardServer(IPAddress address, int port)
        {
            _serverEndPoint = new IPEndPoint(address, port);
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void StartServer()
        {
            if (_listenSocket != null)
            {
                OnLog("The server is already started\n");
                return;
            }
            OnLog("Server starting...\n");
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(_serverEndPoint);
            _listenSocket.Listen(MaxBacklog);
            _cancellationToken = new CancellationTokenSource();
            OnLog("Server started.\n");

            Task.Run(StartLoop);
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void StopServer()
        {
            _cancellationToken.Cancel();
            _listenSocket.Close();
            _listenSocket = null;
            OnLog("Server stopped\n");
        }

        /// <summary>
        /// Start main loop which accepts and processes requests
        /// </summary>
        private void StartLoop()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                _allDone.Reset();
                _listenSocket.BeginAccept(AcceptCallback, _listenSocket);
                _allDone.WaitOne();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _allDone.Set();
            if(_cancellationToken.IsCancellationRequested)
                return;

            var listener = (Socket)ar.AsyncState;
            var handler = listener?.EndAccept(ar);
            OnLog($"Client {handler.RemoteEndPoint} connected");
            if (handler == null) return;
            var state = new StateObject { WorkSocket = handler };

            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var content = string.Empty;

            var state = (StateObject) ar.AsyncState;
            var handler = state?.WorkSocket;

            var bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state?.Sb.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesRead));

                content = state.Sb.ToString();
                if (content.EndsWith(CSC.EndOfFilePoint))
                {
                    OnLog($"Received message from {handler.RemoteEndPoint}");
                    var result = ProcessCommand(content);
                    Send(handler, result);
                }
                else
                {
                    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        ReadCallback, state);
                }
            }
        }

        private string ProcessCommand(string content)
        {
            var elements = content.Replace(CSC.EndOfFilePoint, "").Split(CSC.CommandSeparator);

            var result = CSC.EndOfFilePoint;
            var commandHandler = CommandHandler.GetInstance();

            switch (elements[0].ToLower())
            {
                case "get":
                    result = commandHandler.GetBulletins();
                    break;
                case "add":
                    commandHandler.AddBulletin(elements[1..^1]);
                    break;
                case "edit":
                    commandHandler.EditBulletin(elements[1..^1]);
                    break;
                case "delete":
                    commandHandler.DeleteBulletin(elements[1..^1]);
                    break;
            }

            return result;
        }

        private void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                SendCallback, handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            var handler = (Socket) ar.AsyncState;

            var bytesSend = handler.EndSend(ar);
            OnLog($"Sent {bytesSend} bytes to client {handler.RemoteEndPoint}");

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        /// <summary>
        /// Event handler, writes the log
        /// </summary>
        /// <param name="message">String in log</param>
        private void OnLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}