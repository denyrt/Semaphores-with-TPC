using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerThreading.Core
{
    public class Server
    {
        private readonly static Semaphore _semaphore = new Semaphore(3, 3);
        private readonly TcpListener _tcpListener;
        private readonly List<Thread> _threads;
        private readonly string _password;

        public event EventHandler<ReceiveEventArgs> MessageReceived;

        public Server(string password)
        {
            _tcpListener = TcpListener.Create(20);
            _threads = new List<Thread>();
            _password = password;              
        }

        ~Server()
        {
            foreach (var thread in _threads)
            {
                thread.Abort();
            }
        }

        public void Start()
        {
            _tcpListener.Start();
            Console.WriteLine(_tcpListener.Server.LocalEndPoint.ToString());

            Console.WriteLine("Waiting for connections ...");
            while (true)
            {                
                var client = _tcpListener.AcceptTcpClient();                
                var thread = new Thread(() => ProcessClient(client));
                _threads.Add(thread);
                thread.Start();
            }
        }

        private void ProcessClient(TcpClient client)
        {
            var removePoint = client.Client.RemoteEndPoint.ToString();

            if (!_semaphore.WaitOne(TimeSpan.FromSeconds(3)))
            {
                client.Close();
                Console.WriteLine("{0} was blocked", removePoint);
                _threads.Remove(Thread.CurrentThread);
                return;
            }


            Console.WriteLine("{0} was connected", removePoint);           

            using (var stream = client.GetStream())
            {               
                try
                {
                    stream.Write(Encoding.UTF8.GetBytes("Send password to get access."));

                    stream.Flush();

                    // receive pass
                    while (true)
                    {
                        var msg = ReceiveMessage(stream);
                        if (msg == _password) break;
                        Console.WriteLine("Invalid password");
                    }

                    // reveice messages
                    while (true)
                    {
                        var msg = ReceiveMessage(stream);
                        MessageReceived?.Invoke(this, new ReceiveEventArgs(client, msg));
                    }
                }
                catch (System.IO.IOException ex)
                {
                    
                }                
            }

            Console.WriteLine("{0} was blocked", removePoint);
            _threads.Remove(Thread.CurrentThread);
            _semaphore.Release();
        }

        private string ReceiveMessage(NetworkStream stream)
        {
            var bytes = new List<byte>();
            var buffer = new byte[1024];

            while (stream.CanRead)
            {
                var count = stream.Read(buffer, 0, buffer.Length);
                buffer = buffer.TakeWhile(@byte => @byte != '\0').ToArray();

                if (count < 1024)
                {
                    bytes.AddRange(buffer);
                    return Encoding.UTF8.GetString(bytes.ToArray());
                }

                if (count == buffer.Length)
                {
                    bytes.AddRange(buffer);
                }
            }

            return string.Empty;
        }
    }
}
