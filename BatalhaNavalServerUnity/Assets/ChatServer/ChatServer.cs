using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;

public class ChatServer : MonoBehaviour
{
    public static ChatServer instance;
    private NetManager server;
    private ChatServerListener listener;
    private bool serverStarted;

    private void Awake()
    {
        if (instance!=null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        StartServer();
    }

    public void StartServer()
    {
        serverStarted = true;
        listener = new ChatServerListener();
        server = new NetManager(listener);
        server.Start(8999);
    }
    private void Update()
    {
        if (!serverStarted) return;
        server.PollEvents();
        //SendOtherClientsPackagesToPeers();
    }

    /*
    private void SendOtherClientsPackagesToPeers()
    {
        foreach (var c in ClientHandler.clients)
        {
            var info =ClientHandler.GetInformationFromOtherClients(c.peer);
            ProtocolPang p = new ProtocolPang();
            p.clients = info;
            p.balls = new List<Ball>();
            p.balls = BallManager.Instance.ballsPrt;
            p.scores = ScoreManager.Instance.scores;
            //Debug.Log($"Balls: {p.balls[0].ToString()}");
            listener.processor.Send(c.peer,p,DeliveryMethod.ReliableUnordered);
        }
    }
    */
}
