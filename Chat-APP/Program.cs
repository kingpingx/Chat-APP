using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Threading.Tasks;

namespace TCP_Cient
{
    class Program
    {
        private static bool isClientActive = true;

        static void send()
        {

            while (isClientActive)
            {
                TcpClient client = null;
                NetworkStream stream = null;
                try
                {
                    client = new TcpClient("192.168.2.37", 1302);
                    stream = client.GetStream();
                    while (isClientActive)
                    {
                        Console.WriteLine("Write a message: ");
                        string messageToSend = Console.ReadLine();
                        int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                        byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);

                        stream.Write(sendData, 0, sendData.Length);
                        if (messageToSend == "exit")
                        {
                            isClientActive = false;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("failed to connect...");
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    if (client != null)
                    {
                        client.Close();
                    }
                }
            }
        }

        static void recieve()
        {
            while (isClientActive)
            {
                TcpListener listener = null;
                TcpClient client = null;
                NetworkStream stream = null;
                try
                {
                    listener = new TcpListener(IPAddress.Any, 1301);
                    listener.Start();
                    //Console.WriteLine("Waiting for a connection.");
                    client = listener.AcceptTcpClient();
                    //Console.WriteLine("User2 accepted.....\n");
                    stream = client.GetStream();
                    StreamReader sr = new StreamReader(client.GetStream());
                    StreamWriter sw = new StreamWriter(client.GetStream());

                    while (isClientActive)
                    {
                        byte[] buffer = new byte[1024];
                        int recv = stream.Read(buffer, 0, buffer.Length);
                        if (recv > 0)
                        {
                            string request = Encoding.UTF8.GetString(buffer, 0, recv);
                            if (request == "exit")
                            {
                                isClientActive = false;
                                Console.WriteLine("User2 has left the chat.....");
                            }
                            else Console.WriteLine("User2 >> " + request);
                        }

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong in stream.");

                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }
                    if (client != null)
                    {
                        client.Close();
                    }
                    if (listener != null)
                    {
                        listener.Stop();
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            Thread sendingThread = new Thread(send);
            Thread recievingThread = new Thread(recieve);
            sendingThread.Start();
            recievingThread.Start();

        }

    }
}