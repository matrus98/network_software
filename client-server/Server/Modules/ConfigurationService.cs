using Server.Interfejsy;
using Server.Listeners;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Modules
{
    class ConfigurationService : IServiceModule
    {
        private Server server;

        public ConfigurationService(Server server)
        {
            this.server = server;
        }

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

            switch(command_type)
            {
                case "start-service":
                    IServiceModule service = null;
                    switch (command_data)
                    {
                        case "ping":
                            service = new PingService();
                            break;

                        case "chat":
                            service = new ChatService();
                            break;

                        case "file":
                            service = new FileTransferService();
                            break;
                    }
                    if (service != null)
                    {
                        response.Append("Service started");
                        server.AddService(command_data, service);
                    }
                    else
                    {
                        response.Append("Server does not have this service");
                    }
                    break;

                case "stop-service":
                    server.RemoveService(command_data);
                    response.Append("Service removed");
                    break;

                case "start-medium":
                    IListener listener = null;
                    switch (command_data)
                    {
                        case "tcp":
                            listener = new TcpClientListener(server.IP_Address, SharedConstants.TCP_PORT);
                            response.Append("Medium TCP has been switch on");
                            break;

                        case "udp":
                            listener = new UdpClientListener(server.IP_Address, SharedConstants.UDP_PORT);
                            response.Append("Medium UDP has been switch on");
                            break;

                        case "file":
                            listener = new FileClientListener();
                            response.Append("Medium FILE has been switch on");
                            break;

                        case "serial":
                            listener = new SerialClientListener(SharedConstants.SERIAL_PORT_NAME_SERVER);
                            response.Append("Medium SERIAL has been switch on");
                            break;
                    }
                    if (listener != null)
                        server.AddListener(listener);
                    else
                    {
                        response.Append("Server does not have this medium");
                    }
                    break;

                case "stop-medium":
                    switch (command_data)
                    {
                        case "tcp":
                            server.RemoveListener(typeof(TcpClientListener));
                            response.Append("Medium TCP has been switch off");
                            break;

                        case "udp":
                            server.RemoveListener(typeof(UdpClientListener));
                            response.Append("Medium UDP has been switch off");
                            break;

                        case "file":
                            server.RemoveListener(typeof(FileClientListener));
                            response.Append("Medium FILE has been switch off");
                            break;

                        case "serial":
                            server.RemoveListener(typeof(SerialClientListener));
                            response.Append("Medium SERIAL has been switch off");
                            break;
                    }
                    break;
            }

            return response.ToString();
        }
    }
}
