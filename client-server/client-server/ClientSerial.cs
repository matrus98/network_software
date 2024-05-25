using Shared;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client_server
{
    class ClientSerial
    {
        public SerialPort serialPort;
        bool should_terminate;
        public ClientSerial(string portName)
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

        public void ReadCommands()
        {
            string message = null;
            should_terminate = false;

            serialPort.Open();

            Task.Run(() => ListenForAnswers());

            do
            {
                message = Console.ReadLine();
                if (message.Split(' ')[0].Equals("ping"))
                    message += $" {DateTime.Now.Millisecond}";

                serialPort.WriteLine(message);

            } while (message != "0" && !should_terminate);
            should_terminate = true;
        }

        public Task ListenForAnswers()
        {
            string data;
            
            do
            {
                data = serialPort.ReadLine();

                Console.WriteLine(data);

            } while (!should_terminate);

            return Task.CompletedTask;
        }
    }
}
