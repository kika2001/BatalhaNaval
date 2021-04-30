using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace ProtocolBatalhaNaval
{
    public class Protocol_BN
    {
        public BN_Action action = new BN_Action();
        public BN_Information info = new BN_Information();
    }
    public struct BN_Action : INetSerializable
    {
        public int player;
        public int shot_x { get; set; }
        public int shot_y { get; set; }
        //public HitInfo hitInfo;
        public byte hitInfo;
        public BN_Action(int p,int x,int y)
        {
            player = p;
            shot_x = x;
            shot_y = y;
            //hitInfo = HitInfo.Empty;
            hitInfo = 0x01;
        }
        /*
        public bool ReturnHit(HitInfo h)
        {
            if (h!= HitInfo.Empty)
            {
                hitInfo = h;
                return true;
            }
            else
            {
                return false;
            }
        }
        */
        
        public bool ReturnHit(byte h)
        {
            if (h!=0x01)
            {
                hitInfo = h;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(player);
            writer.Put(shot_x);
            writer.Put(shot_y);
            writer.Put(hitInfo);
        }

        public void Deserialize(NetDataReader reader)
        {
            player = reader.GetInt();
            shot_x = reader.GetInt();
            shot_y = reader.GetInt();
            //hitInfo = (HitInfo)reader.GetInt();
            hitInfo = reader.GetByte();
        }
    }
    public struct BN_Information : INetSerializable
    {
        public byte tag;
        public string message;

        /*
        public BN_Information(byte t, string m)
        {
            tag = t;
            message = m;
        }
        */

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(tag);
            writer.Put(message);
        }

        public void Deserialize(NetDataReader reader)
        {
            tag = reader.GetByte();
            message = reader.GetString();
        }
    }
    public enum HitInfo
    {
        Empty = 0x01,
        Water =0x02,
        Ship = 0x03,
        DestroyedPart = 0x04
    }
}