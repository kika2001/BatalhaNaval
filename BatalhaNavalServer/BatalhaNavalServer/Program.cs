using System;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;
using ProtocolBatalhaNaval;
namespace BatalhaNavalServer
{
    class Program
    {
        private static NetManager server;
        private static NetPacketProcessor processor;
        private static List<NetPeer> clients = new List<NetPeer>();
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            EventBasedNetListener listener = new EventBasedNetListener();
            server = new NetManager(listener);
            processor = new NetPacketProcessor();
            server.UnconnectedMessagesEnabled = true;
            server.BroadcastReceiveEnabled = true;
            server.Start(9000);
            processor.RegisterNestedType<BN_Action>();
            processor.RegisterNestedType<BN_Information>();
            processor.SubscribeReusable<Protocol_BN, NetPeer>(OnReceive);
            listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkReceiveUnconnectedEvent += ListenerOnNetworkReceiveUnconnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;

            Log.Information("Server Started");
            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(100);
            }
            Log.Information("Server Terminated");
        }

        private static void OnReceive(Protocol_BN protocol, NetPeer peer)
        {
            Log.Information ($"Server received information: tag= {protocol.info.tag} | message: {protocol.info.message}");

        }

        private static void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            Log.Information("Event: OnPeerDisconnectedEvent");
        }

        private static void ListenerOnNetworkReceiveUnconnectedEvent(IPEndPoint remoteendpoint, NetPacketReader reader, UnconnectedMessageType messagetype)
        {
            Log.Information("Event: OnNetworkReceiveUnconnectedEvent");
            Log.Information("Payload: {Payload}", reader.GetString());
        }

        private static void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
        {
            //NetDataWriter writer = new NetDataWriter();
            //Log.Information("Event: OnNetworkReceiveEvent");
            //string payload = reader.GetString();
            //Log.Information("Received: {Payload}", payload);
            //writer.Put($"[{peer.EndPoint}]: {payload}");
            //server.SendToAll(writer, DeliveryMethod.Unreliable);
        }

        private static void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            Log.Information("Event: OnPeerConnectedEvent");

            Protocol_BN protocol = new Protocol_BN();
            //message.info.tag = 0x01;
            //message.info.message = "Thank You!";
            BN_Information info = new BN_Information();
            BN_Action action = new BN_Action();
            info.tag = 0x02;
            info.message = "Test";
            protocol.info = info;

            action.player = 0;
            action.shot_x = 1;
            action.shot_y = 2;
            protocol.action = action;
            
            processor.Send(peer, protocol, DeliveryMethod.ReliableUnordered);
            if (clients.Count<2)
            {
                clients.Add(peer);
            }
            
            //foreach (NetPeer client in server.ConnectedPeerList)
            //{
            //    Log.Information("Peer: {Endpoint}", client.EndPoint);
            //}
        }

        private static void ListenerOnConnectionRequestEvent(ConnectionRequest request)
        {
            Log.Information("Event: OnConnectionRequestEvent");
            request.Accept();
        }
    }
}