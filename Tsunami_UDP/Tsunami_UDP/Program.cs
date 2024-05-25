using System;

namespace Tsunami_UDP
{
    class Program
    {
        static void Main(string[] args)
        {
            TsunamiServer server = new TsunamiServer("127.0.0.1", 12345);
            server.Start();
            Console.ReadKey();
        }
    }
}
