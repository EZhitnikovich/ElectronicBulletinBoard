using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BulletinBoard.Model;
using CSC = BulletinBoard.Model.ClientServerConfig;

namespace BulletinBoard.Client
{
    public class BoardClient
    {
        public delegate void ClientObjectHandler(string message);
        public event ClientObjectHandler Log;

        public readonly IPEndPoint IpEndPoint;
        public TcpClient tcpClient;
        private NetworkStream _stream;

        public bool Connected => tcpClient.Connected;

        public BoardClient(IPAddress ipAddress, int port)
        {
            IpEndPoint = new IPEndPoint(ipAddress, port);
        }

        public BoardClient(string hostName, int port)
        {
            IpEndPoint = new IPEndPoint(Dns.GetHostAddresses(hostName).FirstOrDefault() ?? IPAddress.Parse("localhost"),
                port);
        }

        public void StartClient()
        {
            if(tcpClient != null && tcpClient.Connected)
                return;
            
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IpEndPoint);
                _stream = tcpClient.GetStream();
            }
            catch(Exception ex)
            {
                OnLog($"{ex.Message}\n");
            }
        }

        private string SendRequest(string command, string nickname, string password, string jsonObject)
        {
            string sendCommand = $"{command}{CSC.CommandSeparator}{nickname}{CSC.CommandSeparator}{password}{CSC.CommandSeparator}{jsonObject}{CSC.CommandSeparator}";
            var bytes = Encoding.UTF8.GetBytes(sendCommand);
            _stream.Write(bytes, 0, bytes.Length);

            return ReadStream(_stream);
        }

        public List<Bulletin> GetAllBulletins()
        {
            if (!Connected)
                return null;

            var request = SendRequest("get", null, null, null);
            List<Bulletin> bulletins = null;
            if(request != string.Empty)
                bulletins = JsonSerializer.Deserialize<List<Bulletin>>(request);

            return bulletins;
        }

        private string ReadStream(NetworkStream stream)
        {
            try
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);

                return builder.ToString();
            }
            catch (Exception e)
            {
                OnLog(e.Message+"\n");
            }
            return String.Empty;
        }

        public void StopClient()
        {
            if (_stream != null)
                _stream.Close();
            if(tcpClient != null)
                tcpClient.Close();
        }

        protected virtual void OnLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}