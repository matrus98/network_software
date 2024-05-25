using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class SharedConstants
    {
        public static readonly string SERVER_IP = "127.0.0.1";
        public static readonly int TCP_PORT = 12345;
        public static readonly int UDP_PORT = 12346;
        public static readonly string DIRECTORY_FOR_COMMUNICATION = "../../../cs/";

        public static readonly string SERIAL_PORT_NAME_SERVER = "COM1";//SerialPort.GetPortNames()[0];
        public static readonly string SERIAL_PORT_NAME_CLIENT = "COM2";//SerialPort.GetPortNames()[1];
        public static readonly int SERIAL_PORT_BAUD_RATE = 9600;
        public static readonly Parity SERIAL_PORT_PARITY = Parity.None; //None, Odd, Even, Mark, Space
        public static readonly int SERIAL_PORT_DATA_BITS = 8;
        public static readonly StopBits SERIAL_PORT_STOP_BITS = StopBits.One; //None, One, Two, OnePointFive
        public static readonly Handshake SERIAL_PORT_HANDSHAKE = Handshake.None; //None, XOnXOff, RequestToSend, RequestToSendXOnXOff

        public static readonly int SERIAL_PORT_READ_TIMEOUT = 50000;
        public static readonly int SERIAL_PORT_WRITE_TIMEOUT = 50000;

        public static readonly int NETREMOTING_PORT = 12350;

        public static readonly int FILE_COMMUNICATION_DELAY_IN_MILIS = 1000;
    }
}
