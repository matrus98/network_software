using client_server;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = new ClientTcp(SharedConstants.SERVER_IP, SharedConstants.TCP_PORT);
            //var client = new ClientUdp(SharedConstants.SERVER_IP, SharedConstants.UDP_PORT);
            //var client = new ClientFile("test");
            //var client = new ClientSerial(SharedConstants.SERIAL_PORT_NAME_CLIENT); //There have to be at least 2 ports
            //var client = new ClientNET_Remoting(SharedConstants.SERVER_IP, SharedConstants.NETREMOTING_PORT);
            //client.ReadCommands();

            ClientCS client = new ClientCS();
            string medium_choice;
            string command;
            string response = string.Empty;
            bool is_ping = false;
            List<int> ping_test = new List<int>();
            bool should_terminate = false;

            do
            {
                Console.WriteLine("Choose medium: tcp, udp, file, serial, .net, exit (Close program)");
                medium_choice = Console.ReadLine().ToLower();

                if (!medium_choice.Equals("exit"))
                {
                    switch (medium_choice)
                    {
                        case "tcp":
                            command = ask_for_command();
                            is_ping = command.Split(' ')[0].ToLower().Equals("ping");

                            for (int i = 0; i < get_iteration_num(command); i++)
                            {
                                response = client.Send_TCP_Request_And_Get_Response(SharedConstants.SERVER_IP, SharedConstants.TCP_PORT, command);

                                Console.WriteLine(response);

                                if (is_ping)
                                    ping_test.Add(int.Parse(response.Split(' ')[1]));
                            }

                            if(is_ping)
                            {
                                Console.WriteLine($"Ping average response time - {ping_test.Average()} ms");
                                ping_test.Clear();
                            }

                            break;

                        case "udp":
                            command = ask_for_command();
                            is_ping = command.Split(' ')[0].ToLower().Equals("ping");

                            for (int i = 0; i < get_iteration_num(command); i++)
                            {
                                response = client.Send_UDP_Request_And_Get_Response(SharedConstants.SERVER_IP, SharedConstants.UDP_PORT, command);

                                Console.WriteLine(response);

                                if (is_ping)
                                    ping_test.Add(int.Parse(response.Split(' ')[1]));
                            }

                            if (is_ping)
                            {
                                Console.WriteLine($"Ping average response time - {ping_test.Average()} ms");
                                ping_test.Clear();
                            }

                            break;

                        case "file":
                            command = ask_for_command();
                            is_ping = command.Split(' ')[0].ToLower().Equals("ping");

                            Console.WriteLine("Type the name of file");
                            string file_name = Console.ReadLine();

                            for (int i = 0; i < get_iteration_num(command); i++)
                            {
                                response = client.Send_FILE_Request_And_Get_Response(file_name, command);

                                Console.WriteLine(response);

                                if (is_ping && !response.Equals(string.Empty))
                                    ping_test.Add(int.Parse(response.Split(' ')[1]));
                            }

                            if (is_ping)
                            {
                                try
                                {
                                    Console.WriteLine($"Ping average response time - {ping_test.Average()} ms");
                                    ping_test.Clear();
                                }
                                catch (Exception) { }
                            }

                            break;

                        case "serial":
                            command = ask_for_command();
                            is_ping = command.Split(' ')[0].ToLower().Equals("ping");

                            for (int i = 0; i < get_iteration_num(command); i++)
                            {
                                response = client.Send_SERIAL_Request_And_Get_Response(SharedConstants.SERIAL_PORT_NAME_CLIENT, command);

                                Console.WriteLine(response);

                                if (is_ping)
                                    ping_test.Add(int.Parse(response.Split(' ')[1]));
                            }

                            if (is_ping)
                            {
                                Console.WriteLine($"Ping average response time - {ping_test.Average()} ms");
                                ping_test.Clear();
                            }

                            break;

                        case ".net":
                            command = ask_for_command();
                            is_ping = command.Split(' ')[0].ToLower().Equals("ping");

                            for (int i = 0; i < get_iteration_num(command); i++)
                            {
                                response = client.Send_DOT_NET_REMOTING_Request_And_Get_Response(SharedConstants.SERVER_IP, SharedConstants.NETREMOTING_PORT, command);

                                Console.WriteLine(response);

                                if (is_ping)
                                    ping_test.Add(int.Parse(response.Split(' ')[1]));
                            }

                            if (is_ping)
                            {
                                Console.WriteLine($"Ping average response time - {ping_test.Average()} ms");
                                ping_test.Clear();
                            }

                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    should_terminate = true;
                }

                medium_choice = string.Empty;

            } while (!should_terminate);
        }

        static string ask_for_command()
        {
            Console.WriteLine("Type command: ");
            return Console.ReadLine();
        }

        static int get_iteration_num(string command)
        {
            try
            {
                var command_data = command.Split(' ');
                if (command_data[0].ToLower().Equals("ping"))
                    return int.Parse(command_data[1]);
            }
            catch (Exception) { }

            return 1;
        }
    }
}
