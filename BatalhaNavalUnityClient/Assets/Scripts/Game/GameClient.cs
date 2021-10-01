using System;
using LiteNetLib;
using ProtocolBatalhaNaval;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameClient : MonoBehaviour
    {
        private static GameClient instance;
        public static GameClient Instance => instance;
        private Client client;
        public int myId;
        public bool myTurn = false;
        private bool turnedOn=false;
        public int roomID;
        
        //--------------------------Eventos--------------------
        public delegate void dgOnTurn();

        public event dgOnTurn evOnTurn;

        public delegate void dgOnOpponentTurn();

        public event dgOnOpponentTurn evOnOpponentTurn;

        public delegate void dgOnAttackReceived(Protocol_BN p);

        public event dgOnAttackReceived evOnAttackedReceived;
        
        public delegate void dgOnAnswerReceived(Protocol_BN p);

        public event dgOnAnswerReceived evOnAnswerReceived;
        private void Awake()
        {
            instance = this;
        }
        
        
        public void GamePackageReceive(Protocol_BN protocol)
        {
            if (!turnedOn && protocol.info.tag == 0x03)
            {
                roomID = protocol.action.roomID;
                turnedOn = true;
                
                StopPlaying();
            }
            else
            {
                switch (protocol.info.tag)
                {
                    case 0x03:
                        
                        StopPlaying();
                        myId = Int32.Parse(protocol.info.message);
                        evOnOpponentTurn?.Invoke();
                        break;
                    case 0x04:
                        Debug.LogWarning("MENSAGEM PROTOCOL:"+protocol.ToString());
                        myId = Int32.Parse(protocol.info.message);
                        StartPlayingFirst();
                        evOnTurn?.Invoke();
                        break;
                    case 0x05:
                        StopPlaying();
                        RoomManager.Instance.CloseGame();
                        VictoryRoom();
                        Client.Instance.listener?._peer?.Disconnect();
                        RoomManager.Instance.Victory();
                        break;
                    case 0x07://FoiAtacado
                        evOnAttackedReceived?.Invoke(protocol);
                        break;
                    case 0x08:
                        evOnAnswerReceived?.Invoke(protocol);
                        break;
                    
                }
                
            }
        }

       

        public void SendAttack(Vector3Int pos,int playerIndex)
        {
            Protocol_BN protocol = new Protocol_BN();
            BN_Information info = new BN_Information();
            BN_Action action = new BN_Action();
            action.player = playerIndex;
            action.shot_x = pos.x;
            action.shot_y = pos.y;
            action.roomID = roomID;
            action.hitInfo = HitInfo.Empty;
            info.tag = 0x07;
            info.message = "ATTACK";
            protocol.action = action;
            protocol.info = info;
            Client.Instance.SendPacketToServer(protocol, DeliveryMethod.ReliableUnordered);
            evOnOpponentTurn?.Invoke();
            StopPlaying();
        }

        public void SendAnswer(Protocol_BN p, HitInfo h)
        {
            BN_Action a = new BN_Action();
            a.player = p.action.player;
            a.shot_x = p.action.shot_x;
            a.shot_y = p.action.shot_y;
            a.roomID = p.action.roomID;
            a.hitInfo = h;
            BN_Information i = new BN_Information();
            i.tag = 0x08;
            i.message = "ANSWER";
            p.info = i;
            p.action = a;
            Client.Instance.SendPacketToServer(p, DeliveryMethod.ReliableUnordered);
            evOnTurn?.Invoke();
            StartPlayingFirst();
        }
        public void StartPlayingFirst()
        {
            myTurn = true;
            Debug.Log($"My turn : {myTurn}");
        }
        public void StopPlaying()
        {
            myTurn = false;
            Debug.Log($"My turn : {myTurn}");
        }

        public void VictoryRoom()
        {
            Debug.Log("VictoryScreen");
        }
    }
}