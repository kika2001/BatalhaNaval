using LiteNetLib;
using LiteNetLib.Utils;
using ProtocolBatalhaNaval;
using UnityEngine;

namespace DefaultNamespace
{
    public class ClientProcessor
    {
        private readonly NetPacketProcessor processor;

        public ClientProcessor()
        {
            processor = new NetPacketProcessor();
            processor.RegisterNestedType<BN_Information>();
            processor.RegisterNestedType<BN_Action>();
            processor.SubscribeReusable<Protocol_BN>(OnReceive);
        }
        
        private void OnReceive(Protocol_BN protocol)
        {
            Debug.Log($"Information: tag= {protocol.info.tag} | message: {protocol.info.message}");
            Debug.Log($"Action: player= {protocol.action.player} | X: {protocol.action.shot_x} | Y: {protocol.action.shot_y} | HitInfo: {protocol.action.hitInfo}");
            HandlePackets(protocol);
        }

        public void ReadAllPackets(NetDataReader reader)
        {
            processor.ReadAllPackets(reader);
        }

        public void Send<T>(NetPeer peer, T packet, DeliveryMethod deliveryMethod) where T: class, new()
        {
            processor.Send(peer, packet, deliveryMethod);
        }
        private void HandlePackets(Protocol_BN protocol)
        {
            Debug.Log(protocol.ToString());
            switch (protocol.info.tag)
            {
                case 0x01: //JoinGame
                    RoomManager.Instance.OpenRoom("QueueRoom");
                    break;
                case 0x03: case 0x04: case 0x05: case 0x07: case 0x08:
                    RoomManager.Instance.OpenGame();
                    GameClient.Instance.GamePackageReceive(protocol);
                    break;
               
            }
        }
        
        
    }
}