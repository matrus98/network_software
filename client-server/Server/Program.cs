using Server.Listeners;
using Server.Modules;
using Shared;
using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(SharedConstants.SERVER_IP);

            var tcplistener = new TcpClientListener(server.IP_Address, SharedConstants.TCP_PORT);
            var udplistener = new UdpClientListener(server.IP_Address, SharedConstants.UDP_PORT);
            var fileListener = new FileClientListener();
            //var seriallistener = new SerialClientListener(SharedConstants.SERIAL_PORT_NAME_SERVER); //There have to be at least 2 ports
            var netremotelstener = new NetRemotingClientListener(SharedConstants.SERVER_IP, SharedConstants.NETREMOTING_PORT, server.get_and_perform_Service);

            server.AddListener(tcplistener);
            server.AddListener(udplistener);
            server.AddListener(fileListener);
            //server.AddListener(seriallistener);
            server.AddListener(netremotelstener);

            server.CreateConfigurationService();
            server.AddService("ping", new PingService());
            server.AddService("chat", new ChatService());
            server.AddService("file", new FileTransferService());

            server.Start();
            Console.ReadKey();
            server.Stop();
        }
    }
}
