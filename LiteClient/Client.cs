using LiteNetLib;
using System;
using System.Threading;

namespace LiteClient
{
    class Client
    {
        static int Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.Write("Usage: LiteClient IP PORT NUM_CLIENTS:");
                Console.WriteLine("Example:  LiteClient 127.0.0.1 5001 5");
                return 1;
            }

            string ip = args[0];
            int port = int.Parse(args[1]);
            int numClients = int.Parse(args[2]);

            NetManager[] clients = new NetManager[numClients];

            Console.WriteLine($"Connecting clients to {ip}:{port}");

            for (int i=0; i<numClients;i++)
            {
                Console.WriteLine($"Connecting client {i} to {ip}:{port}");

                EventBasedNetListener listener = new EventBasedNetListener();
                NetManager client = new NetManager(listener);
                clients[i] = client;
                client.Start();
                client.Connect(ip, port, "SomeConnectionKey");

                listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
                {
                    Console.WriteLine($"Client {i} got response!"); // dataReader.GetString(1024);
                    dataReader.Recycle();
                };
            }

            // Read data
            while (!Console.KeyAvailable)
            {
                for (int i = 0; i < numClients; i++)
                {
                    var client = clients[i];
                    client.PollEvents();
                }

                Thread.Sleep(15);
            }

            // Stop all clients
            for (int i = 0; i < numClients; i++)
            {
                var client = clients[i];
                client.Stop();
            }

            return 0;
        }
    }
}
