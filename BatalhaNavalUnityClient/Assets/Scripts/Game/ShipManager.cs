using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using LiteNetLib;
using ProtocolBatalhaNaval;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public List<Ship> ships = new List<Ship>();
    public bool allPlaced = false;
    private int shipsAlive;
    public delegate void dgOnShipHit();
    public static event dgOnShipHit evOnShipHit;
    public GameObject waitingOpponent;
    private void Update()
    {
        
        if (ships.Where(d => d.placed==false).ToList().Count()==0 && !allPlaced) //Verifica se estao todos postos
        {
            allPlaced = true;
            Protocol_BN protocol = new Protocol_BN();
            BN_Information info = new BN_Information();
            BN_Action action = new BN_Action();
            info.tag = 0x06;
            action.roomID = GameClient.Instance.roomID;
            protocol.info = info;
            protocol.action = action;
            Client.Instance.SendPacketToServer(protocol,DeliveryMethod.ReliableOrdered);
            waitingOpponent.SetActive(true);
            /*
            foreach (var s in ships)
            {
                s.DestroyShip();
            }
            */
        }
        shipsAlive = ships.Where(d => d.destroyed == false).ToList().Count();
        if (shipsAlive==0)
        {
            Client.Instance.listener?._peer?.Disconnect();
            RoomManager.Instance.Lose();
        }
    }

    public void HittedShip(Vector3Int pos)
    {
        foreach (var ship in ships)
        {
            ship.CheckHit(pos);
        }
    }
    
}
