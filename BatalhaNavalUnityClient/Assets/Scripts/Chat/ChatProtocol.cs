using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using LiteNetLib.Utils;
public class ChatProtocol
{
   public Message Message { get; set; }
}

public struct Message : INetSerializable
{
    public string Owner { get; set; }
    public string Text { get; set; }
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Owner);
        writer.Put(Text);
    }

    public void Deserialize(NetDataReader reader)
    {
        Owner = reader.GetString();
        Text = reader.GetString();
    }
}
