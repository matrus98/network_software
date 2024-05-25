using Shared;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class ClientFile
    {
        bool should_terminate;
        public string CommunicationFileName { get; set; }

        public ClientFile(string communication_file_name)
        {
            CommunicationFileName = communication_file_name;
            Thread.Sleep(3000);
        }

        public void ReadCommands()
        {
            string message = null;
            should_terminate = false;

            Task.Run(() => ListenForAnswers());

            do
            {
                message = Console.ReadLine();
                if (message.Split(' ')[0].Equals("ping"))
                    message += $" {DateTime.Now.Millisecond}";

                using (StreamWriter stream = new StreamWriter(SharedConstants.DIRECTORY_FOR_COMMUNICATION + CommunicationFileName + ".in"))
                {
                    stream.Write($"{message}\n");
                }

            } while (message != "0" && !should_terminate);
            should_terminate = true;
        }

        public Task ListenForAnswers()
        {
            string data;
            int nl;
            string file = SharedConstants.DIRECTORY_FOR_COMMUNICATION + CommunicationFileName + ".out";

            do
            {
                if (File.Exists(file))
                {
                    try
                    {
                        using (StreamReader stream = new StreamReader(file))
                        {
                            data = stream.ReadToEnd();
                        }
                    }
                    catch(Exception)
                    {
                        continue;
                    }

                    File.Delete(file);

                    while ((nl = data.IndexOf("\n")) != -1)
                    {
                        string line = data.Substring(0, nl);
                        data = data.Substring(nl + 1);

                        Console.WriteLine(line);
                    }
                }

            } while (!should_terminate);

            return Task.CompletedTask;
        }
    }
}
