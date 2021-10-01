using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class ChatServerProcessor
{
    private readonly NetPacketProcessor processor;

    public ChatServerProcessor()
    {
        //Fazer os registerNestedTypes aqui
        processor = new NetPacketProcessor();
        
        processor.RegisterNestedType<Message>();
        processor.SubscribeReusable<ChatProtocol,NetPeer>(OnReceive);
        /*
        processor.RegisterNestedType<Vctr3>();
        processor.RegisterNestedType<Harpoon>();
        processor.RegisterNestedType<ActClient>();
        processor.RegisterNestedType<Ball>();
        processor.RegisterNestedType<Scores>();
        processor.SubscribeReusable<ProtocolPang,NetPeer>(OnReceive);
        */
    }
    
    
    private void OnReceive(ChatProtocol protocol, NetPeer peer)
    {
        Message m = new Message();
        m.Owner = peer.EndPoint.ToString();
        m.Text = protocol.Message.Text;
        protocol.Message = m;
        foreach (var client in ChatServerListener.clientsConnected)
        {
            if (client!=peer)
            {
                processor.Send(client,protocol,DeliveryMethod.ReliableUnordered);
            }
        }
    }
    

    public void ReadAllPackets(NetDataReader reader, NetPeer p)
    {
        processor.ReadAllPackets(reader,p);
    }

    public void Send<T>(NetPeer peer, T packet, DeliveryMethod deliveryMethod) where T: class, new()
    {
        processor.Send(peer, packet, deliveryMethod);
    }
}
