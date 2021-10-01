using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using ProtocolBatalhaNaval;

namespace DefaultNamespace
{
    public class ClientListener : INetEventListener
    {
        public NetPeer _peer;
        public ClientProcessor processor;
        
        public ClientListener()
        {
            processor = new ClientProcessor();
        }
        public void OnPeerConnected(NetPeer peer)
        {
            RoomManager.Instance.OpenRoom("ConnectedRoom");
            //Debug.Log($"Connected to server {peer.EndPoint}.");
            _peer = peer;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (peer == _peer)
            {
                _peer = null;
                //RoomManager.Instance.CloseGame();
                RoomManager.Instance.OpenRoom("DisconnectedRoom");
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            processor.ReadAllPackets(reader);
            
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
           
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            
        }

       
        
    }
}