using Server.Interfejsy;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TcpCommunicator : ICommunicator
    {
        TcpClient client;
        CommandD onCommand;
        CommunicatorD onDisconnect;

        public TcpCommunicator(TcpClient client)
        {
            this.client = client;
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            //wybieram usługę w zależności od pierwszego słowa
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;

            Task.Run(() => Listen_for_client_request());
        }

        public void Stop()
        {
            client.GetStream().Close();
            client.Close();
        }

        private Task Listen_for_client_request()
        {
            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[256];
            string data = null;
            int len, nl;
            string answer;

            try
            {
                while (client.Connected)
                {
                    while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        data += Encoding.ASCII.GetString(bytes, 0, len);

                        while ((nl = data.IndexOf("\n")) != -1)
                        {
                            string line = data.Substring(0, nl);
                            data = data.Substring(nl + 1);

                            answer = onCommand(line);
                            Console.WriteLine($"Client has sent request: {line}");

                            Task.Run(() => Send_answer(answer));
                        }
                    }
                }
            }
            catch (Exception)
            {
                Send_answer("Connection closed");
            }

            onDisconnect(this);

            return Task.CompletedTask;
        }

        public Task Send_answer(string answer)
        {
            NetworkStream stream = client.GetStream();

            byte[] data = Encoding.ASCII.GetBytes(answer);
            stream.Write(data, 0, data.Length);

            return Task.CompletedTask;
        }
    }
}