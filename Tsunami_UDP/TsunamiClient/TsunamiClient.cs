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

namespace TsunamiClient
{
    class TsunamiClient
    {
        TcpClient serverTCP;
        UdpClient serverUDP;
        IPEndPoint endPoint;
        bool _should_terminate;

        string file_to_read;
        (int transfer_id, int packet_count, int packets_size) transfer_info;
        ConcurrentQueue<byte[]> packets;
        ConcurrentQueue<(string file_to_upload, int packet_number, int size_of_packets)> lost_packets;

        public TsunamiClient(string ipAddress, int port)
        {
            Thread.Sleep(3000);

            serverTCP = new TcpClient(ipAddress, port);
            endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            serverUDP = new UdpClient(endPoint);

            packets = new ConcurrentQueue<byte[]>();
            lost_packets = new ConcurrentQueue<(string file_to_upload, int packet_number, int size_of_packets)>();
        }

        public void Stop()
        {
            serverTCP.GetStream().Close();
            serverTCP.Close();
            serverUDP.Close();
        }

        public void ReadCommands()
        {
            string message = null;
            _should_terminate = false;

            Task.Run(ListenForTcpAnswers);
            Task.Run(ListenForUdpPackets);
            Task.Run(ProcessReceivedBytes);

            NetworkStream stream = serverTCP.GetStream();

            do
            {
                message = Console.ReadLine();
                file_to_read = message;
                message += "\n";

                byte[] data = Encoding.ASCII.GetBytes(message);

                string file = $"{Directory.GetCurrentDirectory()}\\{file_to_read}";
                if (File.Exists(file))
                    File.Delete(file);

                stream.Write(data, 0, data.Length);

            } while (message != "0" && !_should_terminate);
            _should_terminate = true;

            stream.Close();
            serverTCP.Close();
        }

        public Task ListenForTcpAnswers()
        {
            NetworkStream stream = serverTCP.GetStream();
            byte[] bytes = new byte[256];
            string data = null;
            int len, nl;
            string line;
            string[] transfer_messages;

            do
            {
                try
                {
                    while (serverTCP.Connected)
                    {
                        while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            data += Encoding.ASCII.GetString(bytes, 0, len);

                            while ((nl = data.IndexOf("\n")) != -1)
                            {
                                line = data.Substring(0, nl);
                                data = data.Substring(nl + 1);

                                if(line.Contains("There will be packets amount"))
                                {
                                    transfer_messages = line.Split(": ");

                                    int id = int.Parse(transfer_messages[2].Split(",")[0]);
                                    int amount = int.Parse(transfer_messages[1].Split(" ")[0]);
                                    int packets_size = int.Parse(transfer_messages[3]);
                                    transfer_info = (id, amount, packets_size);
                                }

                                Console.WriteLine(line);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    _should_terminate = true;
                }

            } while (!_should_terminate);

            return Task.CompletedTask;
        }

        public Task ListenForUdpPackets()
        {
            byte[] bytes;

            do
            {
                bytes = serverUDP.Receive(ref endPoint);

                /*Console.Write("Bytes \'");
                foreach (byte b in bytes)
                    Console.Write($"{b} ");
                Console.Write("\' \treceived as message\t");*/
                //string message = Encoding.ASCII.GetString(bytes);
                //Console.WriteLine($"\"{message}\"");

                packets.Enqueue(bytes);

            } while (!_should_terminate);

            return Task.CompletedTask;
        }

        public Task ProcessReceivedBytes()
        {
            byte transfer_id;
            int packet_counter;
            int current_packet_id;
            string message;

            do
            {
                /*while(!packets.IsEmpty)
                {
                    using(StreamWriter writer = new StreamWriter(file_to_read))
                    {
                        while(packets.TryDequeue(out byte[] bytes))
                        {
                            message = Encoding.ASCII.GetString(bytes);

                            writer.Write(message);
                        }
                    }
                }*/
                packet_counter = 0;
                while (packets.TryDequeue(out byte[] bytes))
                {
                    Console.Write("Bytes \'");
                    foreach (byte b in bytes)
                        Console.Write($"{b} ");
                    Console.Write("\' \treceived as message\t");

                    transfer_id = bytes[0];
                    current_packet_id = BitConverter.ToInt32(bytes, 1);
                    message = Encoding.ASCII.GetString(bytes, 5, bytes.Length - 5);

                    Console.WriteLine($"\"Packet {current_packet_id}: {message}\"");

                    
                    if (transfer_info.transfer_id == transfer_id)
                    {
                        packet_counter++;

                        if (current_packet_id != packet_counter)
                        {
                            while (packet_counter != current_packet_id)
                            {
                                lost_packets.Enqueue((file_to_read, packet_counter, transfer_info.packets_size));
                                packet_counter++;
                            };
                        }

                        using (Stream file_stream = new FileStream(file_to_read, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            using (StreamWriter writer = new StreamWriter(file_stream))
                            {
                                writer.BaseStream.Seek((current_packet_id - 1) * (transfer_info.packets_size - 5), SeekOrigin.Begin);

                                writer.Write(message);
                            }
                        }
                    }
                }

                if (packet_counter > 0)
                {
                    if (lost_packets.Count == 0)
                    {
                        Console.WriteLine("All packets have been received");
                    }
                    else
                    {
                        Console.WriteLine($"There are {lost_packets.Count} lost packets");


                        while (lost_packets.TryDequeue(out (string file_name, int packet_number, int packets_size) result))
                        {
                            message = $"resend packet:{result.file_name},{result.packet_number},{result.packets_size}\n";

                            byte[] data = Encoding.ASCII.GetBytes(message);

                            serverTCP.GetStream().Write(data, 0, data.Length);
                        }
                    }
                }

            } while (!_should_terminate);

            return Task.CompletedTask;
        }
    }
}
