using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BatalhaNavalServer;
using LiteNetLib;
using ProtocolBatalhaNaval;
using UnityEngine;

public class GameRoom
{
    public static List<GameRoom> CurrentRooms = new List<GameRoom>();
    private int id;
    public List<NetPeer> players = new List<NetPeer>();
    private int countReady;
    
    public static GameRoom NewGameRoom(List<NetPeer> p)
    {
        GameRoom room = new GameRoom();
        CurrentRooms.Add(room);
        room.id = CurrentRooms.IndexOf(room);
        room.players = p;
        return room;
    }

    public void PlayerReady()
    {
        countReady++;
        if (countReady>=players.Count)
        {
            Debug.Log("All ready");
            
            Protocol_BN p1 = new Protocol_BN();
            BN_Information i1 = new BN_Information();
            BN_Action a1 = new BN_Action();
            i1.tag = 0x04;
            i1.message = "0";
            p1.info = i1;
            
            p1.action = a1;
            Server.processor.Send(players[0],p1,DeliveryMethod.ReliableOrdered);
            
            Protocol_BN p2 = new Protocol_BN();
            BN_Information i2 = new BN_Information();
            BN_Action a2 = new BN_Action();
            i2.tag = 0x03;
            i2.message = "1";
            p2.info = i2;
            p2.action = a2;
            Server.processor.Send(players[1],p2,DeliveryMethod.ReliableOrdered);
            
           
        }
    }
    public static GameRoom GetRoomByID(int id)
    {
        return CurrentRooms.Where(d => d.id == id).ToList()[0];
    }
    public int GetRoomID()
    {
        return CurrentRooms.IndexOf(this);
    }
    public void CloseRoom()
    {
        CurrentRooms.Remove(this);
        players.Clear();
    }
    public void ReceiveAttack(Protocol_BN protocol)
    {
        Debug.Log("Received attack");
        Server.processor.Send(players[1-protocol.action.player],protocol,DeliveryMethod.ReliableOrdered);
    }
    public void ReceiveAnswer(Protocol_BN protocol)
    {
        Debug.Log("Received Answer");
        Server.processor.Send(players[protocol.action.player],protocol,DeliveryMethod.ReliableOrdered);
    }
}
