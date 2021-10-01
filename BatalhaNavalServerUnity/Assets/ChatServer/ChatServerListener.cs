using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

public class ChatServerListener: INetEventListener
{
    
    public ChatServerProcessor processor;
    public static List<NetPeer> clientsConnected = new List<NetPeer>();

    public ChatServerListener()
    {
        processor = new ChatServerProcessor();
    }
    public void OnPeerConnected(NetPeer peer)
    {
        clientsConnected.Add(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        clientsConnected.Remove(peer);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        processor.ReadAllPackets(reader,peer);
       
        
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        //Accepts connection from the client
        request.Accept();
    }
}
