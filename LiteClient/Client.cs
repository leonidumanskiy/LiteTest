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
                Console.WriteLine("Example:  LiteClient 127.0.0.1 9001 5");
                return 1;
            }

            string ip = args[0];
            int port = int.Parse(args[1]);
            int numClients = int.Parse(args[2]);

            NetManager[] clients = new NetManager[numClients];

            for(int i=0; i<numClients;i++)
            {
                EventBasedNetListener listener = new EventBasedNetListener();
                NetManager client = new NetManager(listener);
                clients[i] = client;
                client.Start();
                client.Connect(ip, port, "SomeConnectionKey");

                listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
                {
                    Console.WriteLine("We got: {0}", dataReader.GetString(1024));
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
