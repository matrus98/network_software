using Server.Interfejsy;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server.Modules
{
    class ChatService : IServiceModule
    {
        Dictionary<string, List<string>> data;

        public ChatService()
        {
            data = new Dictionary<string, List<string>>();
        }

        public string AnswerCommand(string command)
        {
            //msg, get, get now, who

            int first_space_index = command.IndexOf(' ');

            string command_type = "";
            string command_data = "";
            if (first_space_index > 0)
            {
                command_type = command.Substring(0, first_space_index);
                command_data = command.Substring(first_space_index + 1);
            }
            else
            {
                command_type = command;
            }

            StringBuilder response = new StringBuilder();
            List<string> messages;

            switch (command_type)
            {
                case "msg":
                    first_space_index = command_data.IndexOf(' ');
                    string msg_target = command_data.Substring(0, first_space_index);
                    string my_nick_with_text = command_data.Substring(first_space_index + 1);
                    first_space_index = my_nick_with_text.IndexOf(' ');

                    string my_nick = my_nick_with_text.Substring(0, first_space_index);
                    

                    if (!data.ContainsKey(msg_target))
                    {
                        messages = new List<string>();

                        data.Add(msg_target, messages);
                    }
                    else
                    {
                        data.TryGetValue(msg_target, out messages);
                    }

                    messages.Add($"<{my_nick}># {my_nick_with_text.Substring(first_space_index + 1)}\n");

                    break;

                case "get": //komenda oczekuje
                    while (!data.ContainsKey(command_data))
                        Thread.Sleep(1000);

                    data.TryGetValue(command_data, out messages);

                    foreach(string s in messages)
                        response.Append(s);

                    break;

                case "getnow": //komenda zwraca odrazu dane, nawet jeśli są puste

                    bool any_messages = data.TryGetValue(command_data, out messages);

                    if(any_messages)
                    {
                        foreach (string s in messages)
                            response.Append(s);
                    }

                    break;

                case "who":
                    foreach(string nick in data.Keys)
                        response.Append($"{nick}\n");
                    break;
            }

            return response.ToString();
        }
    }
}
