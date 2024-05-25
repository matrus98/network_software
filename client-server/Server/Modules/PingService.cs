using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    class PingService : IServiceModule
    {
        public string AnswerCommand(string command)
        {
            /*string[] data = command.Split(' ');

            if (data.Length < 2)
                return "No data to pong";

            int data_count;
            string data_content = "";
            try
            {
                data_count = int.Parse(data[0]);
            }
            catch(Exception)
            {
                return "Inapropriate data count";
            }

            for (int i = 1; i < data.Length - 1; i++)
            {
                data_content += $"{data[i]} ";
            }

            var answer = new StringBuilder();
            answer.Append($"Pong");

            for (int i = 0; i < data_count; i++)
            {
                answer.Append(data_content);
                answer.Append(" ");
            }

            return answer.ToString();*/
            return "Pong";
        }
    }
}
