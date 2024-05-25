using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public delegate string GetService(string command);

    public class RemoteObject : MarshalByRefObject
    {
        /*ConcurrentQueue<string> Commands;
        ConcurrentQueue<string> Responses;*/
        public static GetService CommandPasser;


        public string SendCommandAndGetResponse(string command)
        {
            return CommandPasser(command);
        }
        /*public RemoteObject()
        {
            Commands = new ConcurrentQueue<string>();
            Responses = new ConcurrentQueue<string>();

            Console.WriteLine("Object alive");

        }

        /*public void SendCommand(string command)
        {
            Commands.Enqueue(command);
        }

        public bool TryGetCommand(out string command)
        {
            if (Commands.TryDequeue(out command))
                return true;

            return false;
        }

        public void SendAnswer(string answer)
        {
            Responses.Enqueue(answer);
        }

        public bool TryGetAnswer(out string answer)
        {
            if (Responses.TryDequeue(out answer))
                return true;

            return false;
        }*/
    }
}
