using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class ChatProcessor
{
    private readonly NetPacketProcessor processor;

    public delegate void dgOnReceivePacket(ChatProtocol protocol);

    public static event dgOnReceivePacket evOnReceivedPacket;

    public ChatProcessor()
    {
        processor = new NetPacketProcessor();
        processor.RegisterNestedType<Message>();
        processor.SubscribeReusable<ChatProtocol>(OnReceive);
    }

    private void OnReceive(ChatProtocol protocol)
    {
        //Debug.Log(protocol.clients.Count);
        evOnReceivedPacket?.Invoke(protocol);
    }

    public void ReadAllPackets(NetDataReader reader)
    {
        processor.ReadAllPackets(reader);
    }

    public void Send<T>(NetPeer peer, T packet, DeliveryMethod deliveryMethod) where T: class, new()
    {
        processor.Send(peer, packet, deliveryMethod);
    }
    
}
