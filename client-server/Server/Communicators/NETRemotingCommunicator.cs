using Server.Interfejsy;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;

namespace Server.Communicators
{
    class NETRemotingCommunicator : ICommunicator
    {
        HttpChannel serverChannel;
        public int Port { get; set; }
        public string IP_Address { get; set; }

        CommandD onCommand;
        CommunicatorD onDisconnect;
        GetService CommandPasser;

        public NETRemotingCommunicator(string ip_Address, int port, GetService commandPasser)
        {
            Port = port;
            IP_Address = ip_Address;
            CommandPasser = commandPasser;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;

            serverChannel = new HttpChannel(Port);
            ChannelServices.RegisterChannel(serverChannel, false);

            RemoteObject.CommandPasser += CommandPasser;
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), "RemoteObject", WellKnownObjectMode.Singleton);

            //Task.Run(() => Listen_for_client_request());
        }

        /*private void Listen_for_client_request()
        {
            string command;
            string answer;

            while (!_should_terminate)
            {
                if(service.TryGetCommand(out command))
                {
                    Console.WriteLine($".NET Remoting - Client has sent request: {command}");

                    answer = onCommand(command);

                    service.SendAnswer(answer);
                }
            }
        }*/

        public void Stop()
        {
            
        }
    }
}
