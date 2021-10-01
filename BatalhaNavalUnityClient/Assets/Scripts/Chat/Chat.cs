using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using UnityEngine.UI;
public class Chat : MonoBehaviour
{
    private NetManager client;
    private ChatListener listener;
    
    //-----------------Protocol--------------
    

    private ChatProtocol p;
    public Button connectBtn;
    public GameObject chatBox;
    private void Awake()
    {
        ChatListener.evOnPeerConnected += EnterChat;
    }

    private void EnterChat(NetPeer serverpeer)
    {
        connectBtn.gameObject.SetActive(false);
        chatBox.gameObject.SetActive(true);
    }

    public void ConnectToServer()
    {
        string server = "127.0.0.1";
        int port = 8999;
        listener = new ChatListener();
        client = new NetManager(listener);

        
        client.Start();
        client.Connect(server, port, string.Empty);

    }

    public void SendPacketToServer(ChatProtocol protocol, DeliveryMethod method)
    {
        listener.processor.Send(listener._serverPeer, protocol, method);
    }
    private void Update()
    {
        client?.PollEvents();
    }
    

    private void OnApplicationQuit()
    {
        listener?.DisconnectFromServer();
    }

}
