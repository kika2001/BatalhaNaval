using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

public class ChatListener : INetEventListener
{
    public NetPeer _serverPeer;
    public ChatProcessor processor;

    public delegate void dgOnPeerConnected(NetPeer serverpeer);

    public static event dgOnPeerConnected evOnPeerConnected;
    public ChatListener()
    {
        processor = new ChatProcessor();
    }
    public void OnPeerConnected(NetPeer peer)
    {
        _serverPeer = peer;
        evOnPeerConnected.Invoke(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (_serverPeer!=null)
        {
            _serverPeer = null;
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

    public void SendToServer(ChatProtocol p,DeliveryMethod m)
    {
        processor.Send(_serverPeer,p,m);
    }
    public void DisconnectFromServer()
    {
        _serverPeer?.Disconnect();
    }
}
