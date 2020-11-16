using System;
using ServerThreading.Core;

namespace ServerThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("1111");
            server.MessageReceived += OnMessageReceived;
            server.Start();
        }

        private static void OnMessageReceived(object sender, ReceiveEventArgs e)
        {
            Console.WriteLine(string.Format("[{0}] {1}: {2}", DateTime.Now.ToShortTimeString(),
                e.TcpClient.Client.RemoteEndPoint.ToString(), e.Message));
        }
    }
}
