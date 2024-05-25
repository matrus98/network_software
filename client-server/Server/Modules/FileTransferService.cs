using Server.Interfejsy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    class FileTransferService : IServiceModule
    {
        //użyj base64

        public string AnswerCommand(string command)
        {
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
                return "Inapropriate request";
            }

            StringBuilder response = new StringBuilder();

            switch (command_type)
            {
                case "dir":
                    string[] files = Directory.GetFiles(command_data, "*.txt");

                    if (files.Length == 0)
                        response.Append("Empty directory");

                    foreach(string f in files)
                        response.Append(f);
                    break;
                case "get":
                    try
                    {
                        using (StreamReader stream = new StreamReader(command_data))
                        {
                            response.Append(stream.ReadToEnd());
                        }
                    }
                    catch(Exception)
                    {
                        response.Append("File does not exist");
                    }

                    break;
                case "put":
                    try
                    {
                        first_space_index = command.IndexOf(' ');
                        string filename = command_data.Substring(0, first_space_index);
                        string filedata = command_data.Substring(first_space_index + 1);
                        using (StreamWriter stream = new StreamWriter(filename, true))
                        {
                            stream.WriteLine(filedata);
                        }
                        response.Append("File data added");
                    }
                    catch(Exception)
                    {
                        response.Append("Fail during writing the file");
                    }
                    break;
            }

            return response.ToString();
        }
    }
}
