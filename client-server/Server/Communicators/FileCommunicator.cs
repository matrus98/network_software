using Server.Interfejsy;
using Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communicators
{
    class FileCommunicator : ICommunicator
    {
        CommandD onCommand;
        CommunicatorD onDisconnect;
        bool _should_terminate = false;

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;

            Task.Run(() => Listen_for_client_request());
        }

        public void Stop()
        {
            _should_terminate = true;
            onDisconnect(this);
        }

        private void Listen_for_client_request()
        {
            string data, answer;
            string[] files;
            string file;
            int nl;

            while (!_should_terminate)
            {
                files = Directory.GetFiles(SharedConstants.DIRECTORY_FOR_COMMUNICATION, "*.in");

                if (files.Length > 0)
                {
                    foreach (string path in files)
                    {
                        Thread.Sleep(SharedConstants.FILE_COMMUNICATION_DELAY_IN_MILIS);

                        var tmp = path.Split('\\');
                        file = tmp[tmp.Length - 1];

                        using (StreamReader stream = new StreamReader(file))
                        {
                            data = stream.ReadToEnd();
                        }

                        File.Delete(path);

                        while ((nl = data.IndexOf("\n")) != -1)
                        {
                            string line = data.Substring(0, nl);

                            data = data.Substring(nl + 1);

                            answer = onCommand(line);
                            Console.WriteLine($"Client has sent request: {line}");

                            Send_answer(path, answer);
                        }

                        Thread.Sleep(1000);
                    }
                }
            }
        }

        private void Send_answer(string path, string answer)
        {
            var out_file = path.Substring(0, path.Length - 3) + ".out";
            using (StreamWriter stream = new StreamWriter(out_file))
            {
                stream.Write($"{answer}\n");
            }
        }
    }
}
