using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DefaultNamespace;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using ProtocolBatalhaNaval;

public class Client : MonoBehaviour
{
    private static Client instance;
    public static Client Instance => instance;
    public NetManager client;
    public ClientListener listener;

    private void Awake()
    {
        if (instance!=null)
        {
            instance.DisconnectFromServer();
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        
       RoomManager.Instance.OpenRoom("MainRoom");
    }

    //------------------------------------------------------Behaviours------------------------------------------------------

    #region ButtonBehaviours

    
    public void DisconnectFromServer()
    {
        listener?._peer?.Disconnect();
        //RoomManager.Instance.OpenRoom("MainRoom");
    }

    public void ReturnToConnectRoom()
    {
        RoomManager.Instance.OpenRoom("MainRoom");
    }
    
    public void ConnectToServer()
    {
        string server = "127.0.0.1";
        int port = 9000;
        listener = new ClientListener();
        client = new NetManager(listener);
        
        //Nao está a dar ainda pois falata meter a mudar de cenas e isso usando eventos mas isso vai ser redundante. arranjar forma de melhorar
        //É preciso colocar o servidor da mesma maneira, separando as cenas em classes diferentes
        
        client.Start();
        client.Connect(server, port, string.Empty);
    }
    

    public void JoinQueue()
    {
        Protocol_BN protocol = new Protocol_BN();
        BN_Information info = new BN_Information();
        info.tag = 0x01;
        info.message = "REQUEST";
        protocol.info = info;
        listener.processor.Send(listener._peer, protocol, DeliveryMethod.ReliableUnordered);
    }

    #endregion

   
    void Update()
    {
        if (client != null)
        {
            client.PollEvents();
        }
    }

   

    public void LeaveMatch()
    {
        Protocol_BN protocol = new Protocol_BN();
        BN_Information info = new BN_Information();
        info.tag = 0x02;
        info.message = "LEAVE";
        protocol.info = info;
        listener.processor.Send(listener._peer, protocol, DeliveryMethod.ReliableUnordered);
        RoomManager.Instance.OpenRoom("LostRoom");
    }
    //--------------------------------------------GameStart--------------------------------------------
    public void SendPacketToServer(Protocol_BN protocol, DeliveryMethod method)
    {
        listener.processor.Send(listener._peer, protocol, method);
    }
    private void OnApplicationQuit()
    {
        listener?._peer?.Disconnect();
    }
}