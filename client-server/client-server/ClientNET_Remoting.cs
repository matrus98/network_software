using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;
using System.Runtime.Remoting;
using Shared;

namespace client_server
{
    class ClientNET_Remoting
    {
        HttpChannel server;
        bool should_terminate;
        RemoteObject service;

        public ClientNET_Remoting(string ip_Address, int port)
        {
            Thread.Sleep(2000);
            should_terminate = false;

            server = new HttpChannel();
               
            WellKnownClientTypeEntry remoteType = new WellKnownClientTypeEntry(typeof(RemoteObject), $"http://{ip_Address}:{port}/RemoteObject");
            RemotingConfiguration.RegisterWellKnownClientType(remoteType);

            service = new RemoteObject();
        }

        public void ReadCommands()
        {
            string message = null;
            string answer;
            should_terminate = false;

            do
            {
                message = Console.ReadLine();
                if (message.Split(' ')[0].Equals("ping"))
                    message += $" {DateTime.Now.Millisecond}";

                answer = service.SendCommandAndGetResponse(message);
                Console.WriteLine(answer);

            } while (message != "0" && !should_terminate);
            should_terminate = true;
        }
    }
}
