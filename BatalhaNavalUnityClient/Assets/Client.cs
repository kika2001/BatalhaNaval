using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using ProtocolBatalhaNaval;
public class Client : MonoBehaviour
{
    private static NetManager client;
    private static NetPacketProcessor processor;
    private NetPeer _peer;
    NetDataWriter writer = new NetDataWriter();
    public GameObject mainRoomGO;
    public GameObject disconnectedRoomGO;
    public GameObject connectedRoomGO;


    void Start()
    {
        ReturnToMainRoom();
    }
    //------------------------------------------------------Behaviours------------------------------------------------------
    #region ButtonBehaviours

    public void DisconnectFromServer()
    {
        client.DisconnectPeer(_peer);
        ReturnToMainRoom();
    }
    
    public void ReturnToMainRoom()
    {
        mainRoomGO.SetActive(true);
        disconnectedRoomGO.SetActive(false);
        connectedRoomGO.SetActive(false);
    }

    public void ConnectToServer()
    {
        string server = "127.0.0.1";
        int port = 9000;
        EventBasedNetListener listener = new EventBasedNetListener();
        client = new NetManager(listener);
        processor = new NetPacketProcessor();
        listener.PeerConnectedEvent += OnConnectToServer;
        listener.NetworkReceiveEvent += OnReceiveData;
        listener.PeerDisconnectedEvent += OnDisconect;
        processor.RegisterNestedType<BN_Information>();
        processor.RegisterNestedType<BN_Action>();
        processor.SubscribeReusable<Protocol_BN>(OnReceive);
        
        client.Start();

        NetDataWriter writer = new NetDataWriter();
        //writer.Put("Unconnected Message");
        //client.SendUnconnectedMessage(writer, new IPEndPoint(IPAddress.Parse(server), port));
        //writer.Reset();
        //writer.Put("Anyone Out There!");
        //client.SendBroadcast(writer, port);
        client.Connect(server, port, string.Empty);
    }

    public void JoinGame()
    {
        writer.Reset();
        writer.Put("");
        _peer.Send(writer,DeliveryMethod.ReliableUnordered);
    }
    #endregion
    //------------------------------------------------------Behaviours------------------------------------------------------
    private void OnReceive(Protocol_BN protocol)
    {
        Debug.Log($"Information: tag= {protocol.info.tag} | message: {protocol.info.message}");
        Debug.Log($"Action: player= {protocol.action.player} | X: {protocol.action.shot_x} | Y: {protocol.action.shot_y}");
    }
    private void OnDisconect(NetPeer peer, DisconnectInfo disconnectinfo)
    {
        if (peer == _peer)
        {
            _peer = null;
            mainRoomGO.SetActive(false);
            disconnectedRoomGO.SetActive(true);
            connectedRoomGO.SetActive(false);
        }
    }

    private void OnReceiveData(NetPeer peer, NetPacketReader reader, DeliveryMethod deliverymethod)
    {
        //Debug.Log(reader.GetString());
        processor.ReadAllPackets(reader,peer);
    }

    private void OnConnectToServer(NetPeer peer)
    {
        mainRoomGO.SetActive(false);
        disconnectedRoomGO.SetActive(false);
        connectedRoomGO.SetActive(true);

        Debug.Log($"Connected to server {peer.EndPoint}.");
        _peer = peer;
        StartCoroutine(HandleInputs());  
    }

    private IEnumerator HandleInputs()
    {
        if (_peer!= null)
        {
            writer.Reset();
            writer.Put("Test");
            _peer.Send(writer, DeliveryMethod.ReliableOrdered);
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(HandleInputs());
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (client != null)
        {
            client.PollEvents();
        }
    }
}
