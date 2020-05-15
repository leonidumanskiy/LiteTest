using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LiteServer
{
    class Server
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Write("Usage: LiteServer 9001");
                return 1;
            }

            int port = int.Parse(args[0]);


            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener);
            server.Start(port);

            List<NetPeer> peers = new List<NetPeer>();

            listener.ConnectionRequestEvent += request =>
            {
                request.Accept();
            };

            listener.PeerConnectedEvent += peer =>
            {
                peers.Add(peer);
                Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
            };

            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                Console.WriteLine($"Peer {peer.EndPoint} disconnected: {disconnectInfo.Reason}");
                peers.Remove(peer);
            };
            Console.WriteLine("Listening on port " + port);
            ulong numTick = 0;
            while (true)
            {
                server.PollEvents();

                for (int numPeer = 0; numPeer < peers.Count; numPeer++)
                {
                    string message = $"Hello peer number! {numPeer} Tick number {numTick} Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. ";
                    byte[] bytes = Encoding.UTF8.GetBytes(message);

                    NetPeer peer = peers[numPeer];
                    
                    peer.Send(bytes, DeliveryMethod.Unreliable);
                }

                numTick++;

                Thread.Sleep(33);
            }

            server.Stop();

            return 0;
        }
    }
}
