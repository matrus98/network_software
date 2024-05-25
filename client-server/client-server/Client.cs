using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        TcpClient server;
        bool should_terminate;

        public Client(string ipAddress, int port)
        {
            Thread.Sleep(3000);

            server = new TcpClient(ipAddress, port);
        }

        public void ReadCommands()
        {
            string message = null;
            should_terminate = false;

            Task.Run(() => ListenForAnswers());

            NetworkStream stream = server.GetStream();

            do
            {
                message = Console.ReadLine();
                message += "\n";

                byte[] data = Encoding.ASCII.GetBytes(message);

                stream.Write(data, 0, data.Length);

            } while (message != "0" && !should_terminate);
            should_terminate = true;

            stream.Close();
            server.Close();
        }

        public Task ListenForAnswers()
        {
            NetworkStream stream = server.GetStream();
            byte[] bytes = new byte[256];
            string data = null;
            int len, nl;

            do
            {
                try
                {
                    while (server.Connected)
                    {
                        while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            data += Encoding.ASCII.GetString(bytes, 0, len);

                            while ((nl = data.IndexOf("\n")) != -1)
                            {
                                string line = data.Substring(0, nl);
                                data = data.Substring(nl + 1);

                                Console.WriteLine(line);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    should_terminate = true;
                }

            } while (!should_terminate);

            return Task.CompletedTask;
        }
    }
}
