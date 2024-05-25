using Server.Communicators;
using Server.Interfejsy;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Listeners
{
    class NetRemotingClientListener : IListener
    {
        CommunicatorD onConnect;
        public int Port { get; set; }
        public string IP_Address { get; set; }
        GetService CommandPasser;

        public NetRemotingClientListener(string ip_address, int port, GetService commandPasser)
        {
            Port = port;
            IP_Address = ip_address;
            CommandPasser = commandPasser;
        }

        public void Start(CommunicatorD onConnect)
        {
            this.onConnect += onConnect;

            NETRemotingCommunicator client = new NETRemotingCommunicator(IP_Address, Port, CommandPasser);

            this.onConnect(client);
        }

        public void Stop()
        {
            
        }
    }
}
