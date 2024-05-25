using Server.Communicators;
using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Listeners
{
    class SerialClientListener : IListener
    {
        public string PortName { get; private set; }
        CommunicatorD onConnect;

        public SerialClientListener(string portName)
        {
            this.PortName = portName;
        }

        public void Start(CommunicatorD onConnect)
        {
            //stworzenie jednego odpowiadacza
            this.onConnect += onConnect;

            SerialCommunicator serial = new SerialCommunicator(PortName);

            this.onConnect(serial);
        }

        public void Stop()
        {
            //właściwie nic nie musi robić, bo jest jeden wątek
            
        }
    }
}
