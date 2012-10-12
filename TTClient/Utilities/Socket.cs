using System;
using System.Net.Sockets;

namespace Utilities
{
    public class Socket
    {
        private TcpClient _client;

        public NetworkStream Stream { get; private set; }

        public bool IsConnected { get; private set; }

        public string HostName { get; private set; }

        public int Port { get; private set; }

        public Socket(string hostname, int port)
        {
            HostName = hostname;
            Port = port;
        }

        public NetworkStream Connect()
        {
            _client = new TcpClient();
            try
            {
                _client.Connect(HostName, Port);
                Stream = _client.GetStream();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            IsConnected = true;
            return Stream;
        }

        public void Disconnect()
        {
            if (_client == null) return;
            if (!IsConnected) return;

            try
            {
                _client.Close();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }

            IsConnected = false;
            _client = null;
        }

    }

}

