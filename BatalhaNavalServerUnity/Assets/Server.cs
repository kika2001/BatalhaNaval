using System;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using ProtocolBatalhaNaval;
using UnityEngine;

namespace BatalhaNavalServer
{
    public class Server : MonoBehaviour
    {
        private static NetManager server;
        public static NetPacketProcessor processor;
        public static List<NetPeer> clients = new List<NetPeer>();
        public static List<NetPeer> queueClients = new List<NetPeer>();
        public static EventBasedNetListener listener;
        
        public delegate void dgNewQueueClient();
        public static event dgNewQueueClient evOnNewQueueClient;

        public delegate void dgMatchUpdate();

        public static event dgMatchUpdate evOnMatchUpdate;
        private void Awake()
        {
            listener = new EventBasedNetListener();
            server = new NetManager(listener);
            processor = new NetPacketProcessor();
            server.UnconnectedMessagesEnabled = true;
            server.BroadcastReceiveEnabled = true;
            server.Start(9000);
            processor.RegisterNestedType<BN_Action>();
            processor.RegisterNestedType<BN_Information>();
            processor.SubscribeReusable<Protocol_BN, NetPeer>(OnReceive);
            //--------------------------------------ServerEvents-------------------------------------------------------
            listener.ConnectionRequestEvent += ListenerOnConnectionRequestEvent;
            listener.PeerConnectedEvent += ListenerOnPeerConnectedEvent;
            listener.NetworkReceiveEvent += ListenerOnNetworkReceiveEvent;
            listener.NetworkReceiveUnconnectedEvent += ListenerOnNetworkReceiveUnconnectedEvent;
            listener.PeerDisconnectedEvent += ListenerOnPeerDisconnectedEvent;
            //--------------------------------------ServerEvents-------------------------------------------------------
            
            //--------------------------------------LobbyEvents-------------------------------------------------------
            evOnNewQueueClient+= QueueRoomChecker;
            //--------------------------------------LobbyEvents-------------------------------------------------------
            
            StartCoroutine(PoolEventsUpdate());
        }

     

        private void Start()
        {
            ServerInfo.Instance.WriteConsole("Server Started");
        }

       
        private static void OnReceive(Protocol_BN protocol, NetPeer peer)
        {
            ServerInfo.Instance.WriteConsole(protocol.ToString());
            HandlePackets(protocol,peer);
        }

        private static void ListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            ServerInfo.Instance.WriteConsole($"Client [{peer.EndPoint}] disconnected.");
            WinCondition(peer);
            
            clients.Remove(peer);
            queueClients.Remove(peer);
        }

        private static void ListenerOnNetworkReceiveUnconnectedEvent(IPEndPoint remoteendpoint, NetPacketReader reader, UnconnectedMessageType messagetype)
        {
            Debug.Log("Event: OnNetworkReceiveUnconnectedEvent");
            Debug.Log($"Payload: {reader.GetString()}");
        }

        private static void ListenerOnNetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
        {
            processor.ReadAllPackets(reader,peer);
            ServerInfo.Instance.WriteConsole("Received");
        }

        private static void ListenerOnPeerConnectedEvent(NetPeer peer)
        {
            ServerInfo.Instance.WriteConsole($"Client [{peer.EndPoint}] connected.");
            clients.Add(peer);
        }

        private static void ListenerOnConnectionRequestEvent(ConnectionRequest request)
        {
            Debug.Log("Event: OnConnectionRequestEvent");
            request.Accept();
        }
        private IEnumerator PoolEventsUpdate()
        {
            server.PollEvents();
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(PoolEventsUpdate());
        }

        private static void HandlePackets(Protocol_BN protocol, NetPeer peer)
        {
            switch (protocol.info.tag)
            {
                case 0x01: //JoinGame
                   
                    Protocol_BN p = new Protocol_BN();
                    BN_Information info = new BN_Information();
                    info.tag = 0x01;
                    info.message = "QUEUE";
                    p.info = info;
                    processor.Send(peer, p, DeliveryMethod.ReliableUnordered);
                    
                    queueClients.Add(peer);
                    evOnNewQueueClient?.Invoke();
                    
                    break;
                case 0x02:
                    WinCondition(peer);
                    break;
                case 0x06: //ReadyPackage
                    GameRoom.GetRoomByID(protocol.action.roomID).PlayerReady(); //Recebeu pacote de ready e fazer agora o resto
                    break;
                case 0x07:
                    GameRoom.GetRoomByID(protocol.action.roomID).ReceiveAttack(protocol);
                    break;
                case 0x08:
                    GameRoom.GetRoomByID(protocol.action.roomID).ReceiveAnswer(protocol);
                    break;
            }
        }

        private static void WinCondition(NetPeer disconectPeer)
        {
            for (int i = 0; i < GameRoom.CurrentRooms.Count; i++)
            {
                if (GameRoom.CurrentRooms[i].players.Contains(disconectPeer) && GameRoom.CurrentRooms[i].players.Count>1)
                {
                    GameRoom.CurrentRooms[i].players.Remove(disconectPeer);
                    Protocol_BN p = new Protocol_BN();
                    BN_Information info = new BN_Information();
                    info.tag = 0x05;
                    info.message = "You won because the oppenent disconnected";
                    p.info = info;
                    processor.Send(GameRoom.CurrentRooms[i].players[0], p, DeliveryMethod.ReliableUnordered);
                    GameRoom.CurrentRooms[i].CloseRoom();
                    evOnMatchUpdate?.Invoke();
                }
            }
            
        }
        private void QueueRoomChecker()
        {
            if (queueClients.Count>=2)
            {
                List<NetPeer> c = new List<NetPeer>();
                c.Add(queueClients[0]);
                c.Add(queueClients[1]);
                var room = GameRoom.NewGameRoom(c);
                evOnMatchUpdate?.Invoke();
                queueClients.Remove(c[0]);
                queueClients.Remove(c[1]);

                foreach (var client in c)
                {
                    Protocol_BN protocol = new Protocol_BN();
                    BN_Information info = new BN_Information();
                    BN_Action action = new BN_Action();
                    info.tag = 0x03;
                    info.message = "GAMESTART";
                    protocol.info = info;
                    action.roomID = room.GetRoomID();
                    processor.Send(client, protocol, DeliveryMethod.ReliableOrdered);
                }
                //Only Do after all players confirmed
                /*
                //MessageForFirstClient
                Protocol_BN p = new Protocol_BN();
                BN_Information i = new BN_Information();
                BN_Action a = new BN_Action();
                i.tag = 0x04;
                i.message = "FIRST";
                p.info = i;
                a.roomID = room.GetRoomID();
                processor.Send(c[0], p, DeliveryMethod.ReliableOrdered);
                */
            }
        }
    }
}