using System;
using System.Net.Sockets;

namespace ServerThreading.Core
{
    public sealed class ReceiveEventArgs : EventArgs
    {
        public string Message { get; }
        public TcpClient TcpClient { get; }


        public ReceiveEventArgs(TcpClient tcpClient, string message)
        {
            Message = message;
            TcpClient = tcpClient;
        }
    }
}
