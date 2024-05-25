using System;

namespace TsunamiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TsunamiClient client = new TsunamiClient("127.0.0.1", 12345);
            client.ReadCommands();
            client.Stop();
        }
    }
}
