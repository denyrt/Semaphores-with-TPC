using System;
using System.Net.Sockets;
using System.Text;

namespace MyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcpClient = new TcpClient();

            tcpClient.Connect("192.168.0.103", 20);
            using (var stream = tcpClient.GetStream())
            {
                while (true)
                {
                    Console.Write("Message: ");
                    var bytes = Encoding.UTF8.GetBytes(Console.ReadLine());
                    stream.Write(bytes);
                }
            }                
        }
    }
}
