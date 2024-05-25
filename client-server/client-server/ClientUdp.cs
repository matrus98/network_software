using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class ClientUdp
    {
        UdpClient server;
        IPEndPoint EndPoint;
        bool should_terminate;

        public ClientUdp(string ipAddress, int port)
        {
            Thread.Sleep(3000);

            //Make some difference between ports values. 
            //In opposite situation You send message to Yourself
            EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port + 10);
            server = new UdpClient("127.0.0.1", EndPoint.Port - 10);
        }

        public void ReadCommands()
        {
            string message = null;
            should_terminate = false;

            Task.Run(() => ListenForAnswers());

            do
            {
                message = Console.ReadLine();
                if (message.Split(' ')[0].Equals("ping"))
                    message += $" {DateTime.Now.Millisecond}";
                message += "\n";

                byte[] data = Encoding.ASCII.GetBytes(message);
                server.Send(data, data.Length);

            } while (message != "0" && !should_terminate);
            should_terminate = true;

            server.Close();
        }

        public Task ListenForAnswers()
        {
            byte[] bytes = new byte[256];
            string data = null;

            do
            {
                bytes = server.Receive(ref EndPoint);
                data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.WriteLine(data);

            } while (!should_terminate);

            return Task.CompletedTask;
        }
    }
}
