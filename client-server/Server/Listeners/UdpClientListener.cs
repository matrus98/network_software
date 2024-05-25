using Server.Communicators;
using Server.Interfejsy;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server.Listeners
{
    class UdpClientListener : IListener
    {
        public IPEndPoint EndPoint;
        CommunicatorD onConnect;

        public UdpClientListener(string ipAddress, int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }

        /*
         for UdpClient.Send(byte[]....)

        Serwer:             '\n'
            byte[] UdpClient.Receive(...)

            jeśli mamy kompletną informację to obsługujemy zapytanie, jeśli nie to odkładamy do kolekcji datagramów oczekujących

        IPEndpoint ---- > List<byte[]>
         */

        public void Start(CommunicatorD onConnect)
        {
            this.onConnect += onConnect;

            UdpCommunicator udp = new UdpCommunicator(EndPoint);

            this.onConnect(udp);
        }

        public void Stop()
        {
            
        }
    }
}
