using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace ProtocolBatalhaNaval
{
    public class Protocol_BN
    {
        public BN_Action action { get; set; }
        public BN_Information info { get; set; }
        public override string ToString()
        {
            return $"Actions: player-{action.player} , shot_x-{action.shot_x} , shot_y-{action.shot_y} , hitInfo-{action.hitInfo} |\n" +
                   $"Information: tag-{info.tag} , message-{info.message}";
        }
    }
    public struct BN_Action : INetSerializable
    {
        public int roomID { get; set; }
        public int player{ get; set; }
        public int shot_x { get; set; }
        public int shot_y { get; set; }
        public HitInfo hitInfo{ get; set; }
        public BN_Action(int p,int x,int y)
        {
            roomID = 0;
            player = p;
            shot_x = x;
            shot_y = y;
            hitInfo = HitInfo.Empty;
        }
        
        public bool ReturnHit(HitInfo h)
        {
            if (h!=HitInfo.Empty)
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
            writer.Put(roomID);
            writer.Put(player);
            writer.Put(shot_x);
            writer.Put(shot_y);
            writer.Put((int)hitInfo);
        }

        public void Deserialize(NetDataReader reader)
        {
            roomID = reader.GetInt();
            player = reader.GetInt();
            shot_x = reader.GetInt();
            shot_y = reader.GetInt();
            hitInfo = (HitInfo)reader.GetInt();
        }
    }
    public struct BN_Information : INetSerializable
    {
        /// <summary>
        /// 0x01-JoinQueue | 0x02-Leave | 0x03 - GameStart | 0x04 - FirstPlay | 0x05 - Win | 0x06- Ready | 0x07 Attack
        /// </summary>
        public byte tag{ get; set; }
        public string message{ get; set; }

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
        Empty,
        Water,
        Ship,
        DestroyedPart,
    }
}