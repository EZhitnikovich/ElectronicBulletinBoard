using System;
using System.Net.Sockets;
using System.Text;
using CSC = BulletinBoard.Model.ClientServerConfig;

namespace BulletinBoard.Server
{
    internal class ClientObject
    {
        public delegate void ClientObjectHandler(string message);
        public event ClientObjectHandler Log;

        public TcpClient Client;
        private BoardServer server;

        public ClientObject(TcpClient client, BoardServer boardServer)
        {
            Client = client;
            server = boardServer;
        }

        public void Process()
        {
            if (Client == null)
            {
                return;
            }

            var stream = Client.GetStream();

            try
            {
                while (!server._cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = ReadStream(stream);
                        var command = ProcessCommand(message);
                        if (message == string.Empty && command == string.Empty)
                            break;
                        var data = Encoding.UTF8.GetBytes(command);
                        stream.Write(data, 0, data.Length);
                        OnLog($"{data.Length} bytes sent to {Client.Client.RemoteEndPoint}\n");
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                OnLog(e.Message+"\n");
            }
            finally
            {
                OnLog($"{Client.Client.RemoteEndPoint} disconnected.\n");
                Disconnect(stream);
            }
        }

        private string ProcessCommand(string content)
        {
            var elements = content.Split(CSC.CommandSeparator);
            var result = "NULL";
            var commandHandler = CommandHandler.GetInstance();

            switch (elements[0].ToLower())
            {
                case "get":
                    result = commandHandler.GetBulletins();
                    break;
                case "add":
                    commandHandler.AddBulletin(elements[1..]);
                    break;
                case "delete":
                    commandHandler.DeleteBulletin(elements[1..]);
                    break;
                case "admin":
                    result = commandHandler.CheckAdministrator(elements[1..]);
                    break;
            }

            return result;
        }

        private string ReadStream(NetworkStream stream)
        {
            StringBuilder builder = new StringBuilder();
            var data = new byte[64];
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            } while (stream.DataAvailable);

            return builder.ToString();
        }

        private void Disconnect(NetworkStream stream)
        {
            if (stream != null)
            {
                stream.Close();
            }

            if (Client != null)
            {
                Client.Close();
            }
        }

        protected virtual void OnLog(string message)
        {
            Log?.Invoke(message);
        }
    }
}