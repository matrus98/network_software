using Server.Interfejsy;
using Shared;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Communicators
{
    class SerialCommunicator : ICommunicator
    {
        SerialPort serialPort;
        CommandD onCommand;
        CommunicatorD onDisconnet;
        bool _should_terminate = false;

        public SerialCommunicator(string portName)
        {
            serialPort = new SerialPort(portName);

            //https://docs.microsoft.com/pl-pl/dotnet/api/system.io.ports.serialport?view=dotnet-plat-ext-6.0
            serialPort.BaudRate = SharedConstants.SERIAL_PORT_BAUD_RATE;
            serialPort.Parity = SharedConstants.SERIAL_PORT_PARITY;
            serialPort.DataBits = SharedConstants.SERIAL_PORT_DATA_BITS;
            serialPort.StopBits = SharedConstants.SERIAL_PORT_STOP_BITS;
            serialPort.Handshake = SharedConstants.SERIAL_PORT_HANDSHAKE;

            serialPort.ReadTimeout = SharedConstants.SERIAL_PORT_READ_TIMEOUT;
            serialPort.WriteTimeout = SharedConstants.SERIAL_PORT_WRITE_TIMEOUT;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand += onCommand;
            this.onDisconnet += onDisconnect;

            //czytaj i odpowiadaj w osobnym wątku
            serialPort.Open();

            Task.Run(() => Listen_for_new_clients());
        }

        private void Listen_for_new_clients()
        {
            string data = null;
            string answer;

            do
            {
                data = serialPort.ReadLine();
                
                answer = onCommand(data);
                Console.WriteLine($"Client has sent request: {data}");

                serialPort.WriteLine(answer);
                

            } while (!_should_terminate);
        }

        public void Stop()
        {
            _should_terminate = true;
            onDisconnet(this);
        }
    }
}
