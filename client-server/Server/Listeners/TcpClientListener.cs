using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Listeners
{
    class TcpClientListener : IListener
    {

        public int Port { get; private set; }
        TcpListener tcpServerListener;
        bool _should_terminate = false;
        //List<TcpClient> clients;
        CommunicatorD onConnect;

        public TcpClientListener(string ipAddress, int port)
        {
            Port = port;

            tcpServerListener = new TcpListener(IPAddress.Parse(ipAddress), port);

            //clients = new List<TcpClient>();

            //services = new Dictionary<string, IServiceModule>();

            //new thread with "listen for new connection" loop
        }

        private void Listen_for_new_clients()
        {
            while (!_should_terminate)
            {
                TcpClient client = tcpServerListener.AcceptTcpClient();

                //clients.Add(client);
                onConnect(new TcpCommunicator(client));

                //Task.Run(() => { Listen_for_client_request(client); });
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            this.onConnect += onConnect;

            tcpServerListener.Start();

            Task.Run(() => { Listen_for_new_clients(); });
        }

        public void Stop()
        {
            _should_terminate = true;

            tcpServerListener.Stop();
            //foreach (var client in clients)
            //{
            //    client.GetStream().Close();
            //    client.Close();
            //    clients.Remove(client);
            //}
        }
    }
}
