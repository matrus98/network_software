using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Communicators
{
    class UdpCommunicator : ICommunicator
    {
        public IPEndPoint EndPoint;
        CommandD onCommand;
        CommunicatorD onDisconnect;
        bool _should_terminate = false;
        //List<TcpClient> clients;
        UdpClient udpServerListener;

        public UdpCommunicator(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            udpServerListener = new UdpClient(endPoint.Port);
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;

            Task.Run(() => Listen_for_new_clients());
        }

        public void Stop()
        {
            _should_terminate = true;
            onDisconnect(this);
        }

        private void Listen_for_new_clients()
        {
            string answer;
            byte[] response;
            int nl;
            string data = null;

            while (!_should_terminate)
            {
                byte[] bytes = udpServerListener.Receive(ref EndPoint);

                if (bytes != null)
                {
                    data += Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    while ((nl = data.IndexOf("\n")) != -1)
                    {
                        string line = data.Substring(0, nl);
                        data = data.Substring(nl + 1);
                        answer = onCommand(line);

                        Console.WriteLine($"Client has sent request: {line}");

                        response = Encoding.ASCII.GetBytes(answer);
                        udpServerListener.Send(response, response.Length, EndPoint);
                    }
                }

            }
        }
    }
}
