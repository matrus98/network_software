using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tsunami_UDP
{
    class TsunamiServer
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public int PacketDataSize { get; set; }
        bool _should_terminate = false;

        TcpListener serverListener;
        public List<TcpClient> clients;
        public Dictionary<TcpClient, string> sessions;

        Random r = new Random();

        public TsunamiServer(string ipAddress, int port, int data_bytes_size = 128)
        {
            Address = IPAddress.Parse(ipAddress);
            Port = port;
            PacketDataSize = data_bytes_size;

            serverListener = new TcpListener(Address, Port);

            clients = new List<TcpClient>();
            sessions = new Dictionary<TcpClient, string>();
        }

        public void Start()
        {
            serverListener.Start();
            Task.Run(listen_for_new_clients);
            Console.WriteLine("Server has started");
            Task.Run(listen_for_download_request);
        }

        public void Stop()
        {
            _should_terminate = true;

            foreach (var client in clients.ToArray())
                clients.Remove(client);
        }

        public Task listen_for_new_clients()
        {
            while (!_should_terminate)
            {
                TcpClient client = serverListener.AcceptTcpClient();

                clients.Add(client);

                string hello_message = $"Hello client! Avaible files to download:\n";

                foreach (string f in Directory.GetFiles(Directory.GetCurrentDirectory()))
                    hello_message += $"{f.Split("\\").Last()}\n";
                hello_message += '\n';

                Send_message(client.GetStream(), hello_message);

                Task.Run(() => Listen_for_client_request(client));
            }

            return Task.CompletedTask;
        }

        private Task Listen_for_client_request(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[256];
            string data = null;
            int len, nl;
            string[] command_data;

            try
            {
                while (client.Connected)
                {
                    while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        data += Encoding.ASCII.GetString(bytes, 0, len);

                        while ((nl = data.IndexOf("\n")) != -1)
                        {
                            string line = data.Substring(0, nl);
                            try
                            {
                                data = data.Substring(nl + 1);
                            }
                            catch (Exception)
                            {
                                data = string.Empty;
                            }

                            command_data = line.Split(":");
                            if (!command_data[0].Equals("resend packet"))
                            {
                                Console.WriteLine("Client has sent request to download file: " + line + "\n");

                                if (line != string.Empty)
                                {
                                    sessions.Add(client, line);
                                }
                            }
                            else
                            {
                                resend_lost_packet(client, command_data[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Send_message(stream, "Connection closed");
            }

            clients.Remove(client);

            return Task.CompletedTask;
        }

        private Task listen_for_download_request()
        {
            while(!_should_terminate)
            {
                Thread.Sleep(100);

                if(sessions.Count > 0)
                {
                    KeyValuePair<TcpClient, string> request = sessions.First();
                    sessions.Remove(request.Key);

                    string file = $"{Directory.GetCurrentDirectory()}\\{request.Value}";
                    if (File.Exists(file))
                        Task.Run(() => start_upload(request));
                    else Send_message(request.Key.GetStream(), "There is no such file"); // nie wysyła informacji !!!!!!!!!!!
                }
            }

            return Task.CompletedTask;
        }

        private void start_upload(KeyValuePair<TcpClient, string> request)
        {
            Send_message(request.Key.GetStream(), "Starting download...");
            UdpClient client = new UdpClient();

            string file = $"{Directory.GetCurrentDirectory()}\\{request.Value}";
            FileStream fileStream = File.OpenRead(file);

            long file_size = fileStream.Length;
            int bytes_to_read = (PacketDataSize - 4) / 8;
            decimal packets_count = Math.Ceiling((decimal)(file_size / bytes_to_read));

            byte[] transfer_id_byte = new byte[1];
            r.NextBytes(transfer_id_byte);
            Send_message(request.Key.GetStream(), $"There will be packets amount: {packets_count} - transfer id: {transfer_id_byte[0]}, packets byte size: {bytes_to_read + 5}");

            Thread.Sleep(1000);

            BinaryReader reader = new BinaryReader(fileStream);

            byte[] header_bytes, file_bytes, bytes_to_send;
            header_bytes = new byte[5];
            bytes_to_send = new byte[header_bytes.Length + bytes_to_read];
            int packet_counter = 1;
            byte[] counter_bytes;

            int packet_lost_simmulation = 2;// r.Next(1, decimal.ToInt32(packets_count) - 5);
            Console.WriteLine($"Packet {packet_lost_simmulation} will be lost");

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                file_bytes = reader.ReadBytes(bytes_to_read);
                counter_bytes = BitConverter.GetBytes(packet_counter);

                if (packet_counter == packet_lost_simmulation)
                {
                    Console.WriteLine($"Dropped packet: {Encoding.ASCII.GetString(file_bytes, 0, file_bytes.Length)}");
                    packet_counter++;
                    continue;
                }

                Buffer.BlockCopy(transfer_id_byte, 0, header_bytes, 0, transfer_id_byte.Length);
                Buffer.BlockCopy(counter_bytes, 0, header_bytes, transfer_id_byte.Length, counter_bytes.Length);

                Buffer.BlockCopy(header_bytes, 0, bytes_to_send, 0, header_bytes.Length);
                Buffer.BlockCopy(file_bytes, 0, bytes_to_send, header_bytes.Length, file_bytes.Length);

                client.Send(bytes_to_send, bytes_to_send.Length, request.Key.Client.LocalEndPoint as IPEndPoint);
                packet_counter++;
            }

            fileStream.Close();
            reader.Close();
            client.Close();
        }

        private void resend_lost_packet(TcpClient demander, string command_data)
        {
            string[] lost_packet_parameters = command_data.Split(",");

            UdpClient client = new UdpClient();

            FileStream fileStream = File.OpenRead(lost_packet_parameters[0]);

            int bytes_to_read = (PacketDataSize - 4) / 8;

            byte[] transfer_id_byte = new byte[1];
            r.NextBytes(transfer_id_byte);
            Send_message(demander.GetStream(), $"There will be packets amount: 1 - transfer id: {transfer_id_byte[0]}, packets byte size: {bytes_to_read + 5}");

            Thread.Sleep(1000);

            BinaryReader reader = new BinaryReader(fileStream);

            int packet_size = int.Parse(lost_packet_parameters[2]);
            int packet_number = int.Parse(lost_packet_parameters[1]);
            reader.BaseStream.Seek((packet_number - 1) * (packet_size - 5), SeekOrigin.Begin);
            byte[] packet_data = reader.ReadBytes(packet_size - 5);

            byte[] header_bytes, bytes_to_send;
            header_bytes = new byte[5];
            bytes_to_send = new byte[header_bytes.Length + packet_data.Length];

            byte[] packet_number_bytes = BitConverter.GetBytes(packet_number);
            Buffer.BlockCopy(transfer_id_byte, 0, header_bytes, 0, transfer_id_byte.Length);
            Buffer.BlockCopy(packet_number_bytes, 0, header_bytes, transfer_id_byte.Length, packet_number_bytes.Length);

            Buffer.BlockCopy(header_bytes, 0, bytes_to_send, 0, header_bytes.Length);
            Buffer.BlockCopy(packet_data, 0, bytes_to_send, header_bytes.Length, packet_data.Length);
            //Potrzebny jeszcze IPEndPoint, bo nie znam usera, który wysłał prośbę o ponowienie zapytania
            client.Send(bytes_to_send, bytes_to_send.Length, demander.Client.LocalEndPoint as IPEndPoint);

            fileStream.Close();
            reader.Close();
            client.Close();
        }

        private void Send_message(NetworkStream stream, string message)
        {
            message += '\n';
            byte[] buff = Encoding.ASCII.GetBytes(message);

            stream.Write(buff, 0, buff.Length);
        }
    }
}
