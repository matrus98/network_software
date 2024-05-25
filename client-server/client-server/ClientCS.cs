using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace client_server
{
    class ClientCS
    {
        //public ClientFile(string communication_file_name) - można podmienić na dynamiczne
        //public ClientUdp(string ipAddress, int port)
        //public ClientNET_Remoting(string ip_Address, int port)
        //public ClientSerial(string portName)
        //public ClientTcp(string ipAddress, int port)
        public int RequestTimeout { get; set; }

        public ClientCS(int timeout_in_milis = 5000)
        {
            RequestTimeout = timeout_in_milis;
        }

        public string Send_TCP_Request_And_Get_Response(string ipAddress, int port, string command)
        {
            TcpClient server = new TcpClient(ipAddress, port);

            NetworkStream stream = server.GetStream();

            //send message
            command += "\n";
            byte[] data = Encoding.ASCII.GetBytes(command);

            var start = DateTime.Now.Millisecond;
            stream.Write(data, 0, data.Length);

            //listen for response
            string response = "";
            bool data_received = false;
            byte[] bytes = new byte[256];
            int len;

            do 
            {
                if (server.Connected && (len = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    response += Encoding.ASCII.GetString(bytes, 0, len);
                    data_received = true;
                }
            } while (!data_received && DateTime.Now.Millisecond - start > RequestTimeout);

            if (response.Split('\n')[0].ToLower().Equals("pong"))
                response = response.Insert(4, $" {DateTime.Now.Millisecond - start} ms");

            return response;
        }

        public string Send_UDP_Request_And_Get_Response(string ipAddress, int port, string command)
        {
            IPEndPoint EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port + 10);
            UdpClient server = new UdpClient("127.0.0.1", EndPoint.Port - 10);

            command += "\n";
            byte[] data = Encoding.ASCII.GetBytes(command);

            var start = DateTime.Now.Millisecond;
            server.Send(data, data.Length);

            byte[] bytes = server.Receive(ref EndPoint);
            string response = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

            if (response.Split('\n')[0].ToLower().Equals("pong"))
                response = response.Insert(4, $" {DateTime.Now.Millisecond - start} ms");

            return response;
        }

        public string Send_FILE_Request_And_Get_Response(string file_name, string command)
        {
            string path = SharedConstants.DIRECTORY_FOR_COMMUNICATION + file_name;

            bool successfully_saved = false;
            var start = DateTime.Now.Millisecond;
            do
            {
                try
                {
                    using (StreamWriter stream = new StreamWriter(path + ".in"))
                    {
                        stream.Write($"{command}\n");
                        successfully_saved = true;
                    }
                }
                catch (Exception) { }
            } while (!successfully_saved);

            Thread.Sleep(SharedConstants.FILE_COMMUNICATION_DELAY_IN_MILIS * 2);

            string response = "";
            bool data_received = false;

            string answer_file = path + ".out";
            do
            {
                if (File.Exists(answer_file))
                {
                    try
                    {
                        using (StreamReader stream = new StreamReader(answer_file))
                        {
                            response = stream.ReadToEnd();
                            data_received = true;
                        }

                        File.Delete(answer_file);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            } while (!data_received && DateTime.Now.Millisecond - start > RequestTimeout);

            if (response.Split('\n')[0].ToLower().Equals("pong"))
                response = response.Insert(4, $" {DateTime.Now.Millisecond - start} ms");

            return response;
        }

        public string Send_SERIAL_Request_And_Get_Response(string port_name, string command)
        {
            SerialPort serialPort = new SerialPort(port_name);

            //https://docs.microsoft.com/pl-pl/dotnet/api/system.io.ports.serialport?view=dotnet-plat-ext-6.0
            serialPort.BaudRate = SharedConstants.SERIAL_PORT_BAUD_RATE;
            serialPort.Parity = SharedConstants.SERIAL_PORT_PARITY;
            serialPort.DataBits = SharedConstants.SERIAL_PORT_DATA_BITS;
            serialPort.StopBits = SharedConstants.SERIAL_PORT_STOP_BITS;
            serialPort.Handshake = SharedConstants.SERIAL_PORT_HANDSHAKE;

            serialPort.ReadTimeout = SharedConstants.SERIAL_PORT_READ_TIMEOUT;
            serialPort.WriteTimeout = SharedConstants.SERIAL_PORT_WRITE_TIMEOUT;

            serialPort.Open();

            var start = DateTime.Now.Millisecond;
            serialPort.WriteLine(command);

            string response = serialPort.ReadLine();

            if (response.Split('\n')[0].ToLower().Equals("pong"))
                response = response.Insert(4, $" {DateTime.Now.Millisecond - start} ms");

            return response;
        }

        public string Send_DOT_NET_REMOTING_Request_And_Get_Response(string ip_Address, int port, string command)
        {
            RemoteObject service = (RemoteObject)Activator.GetObject(typeof(RemoteObject), $"http://{ip_Address}:{port}/RemoteObject");

            var start = DateTime.Now.Millisecond;
            string response = service.SendCommandAndGetResponse(command);

            if (response.Split('\n')[0].ToLower().Equals("pong"))
                response = response.Insert(4, $" {DateTime.Now.Millisecond - start} ms");

            return response;
        }
    }
}
