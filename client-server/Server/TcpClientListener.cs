using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TcpClientListener : IListener
    {

        public int Port { get; private set; }
        TcpListener serverListener;
        bool _should_terminate = false;
        //List<TcpClient> clients;
        CommunicatorD onConnect;

        public TcpClientListener(string ipAddress, int port)
        {
            Port = port;

            serverListener = new TcpListener(IPAddress.Parse(ipAddress), port);

            //clients = new List<TcpClient>();

            //services = new Dictionary<string, IServiceModule>();

            //new thread with "listen for new connection" loop
        }

        private void Listen_for_new_clients()
        {
            while (!_should_terminate)
            {
                TcpClient client = serverListener.AcceptTcpClient();

                //clients.Add(client);
                onConnect(new CommunicatorListener(client));

                //Task.Run(() => { Listen_for_client_request(client); });
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            this.onConnect += onConnect;

            serverListener.Start();

            Task.Run(() => { Listen_for_new_clients(); });
        }

        public void Stop()
        {
            _should_terminate = true;

            serverListener.Stop();
            //foreach (var client in clients)
            //{
            //    client.GetStream().Close();
            //    client.Close();
            //    clients.Remove(client);
            //}
        }
    }
}
