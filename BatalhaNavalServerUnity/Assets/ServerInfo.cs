using System;
using System.Collections;
using System.Collections.Generic;
using BatalhaNavalServer;
using LiteNetLib;
using TMPro;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    private static ServerInfo instance;
    public static ServerInfo Instance { get { return instance; } }
    [Header("PlayersOnlineGO")] 
    public TextMeshProUGUI playerOnlineText;
    [Header("PlayersQueueGO")] 
    public TextMeshProUGUI playerQueueText;
    [Header("MatchesGO")] 
    public TextMeshProUGUI matchesText;
    [Header("ConsoleGO")] 
    public TextMeshProUGUI consoleText;

    void Awake()
    {
        if (instance!=null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    private void Start()
    {
        Server.listener.PeerConnectedEvent += UpdatePlayersOnline;
        Server.listener.PeerDisconnectedEvent += UpdatePlayersOnline;
        Server.evOnNewQueueClient += UpdatePlayersOnline;
        Server.evOnMatchUpdate += UpdateMatches;
    }

    private void UpdateMatches()
    {
        matchesText.text = "";
        for (int i = 0; i < GameRoom.CurrentRooms.Count; i++)
        {
            matchesText.text += $"Match {i}:Players[{GameRoom.CurrentRooms[i].players[0].EndPoint}";
            for (int j = 1; j < GameRoom.CurrentRooms[i].players.Count; j++)
            {
                matchesText.text += $", {GameRoom.CurrentRooms[i].players[0].EndPoint}";
            }

            matchesText.text += "] \n";

        }
    }

    private void UpdatePlayersOnline()
    {
        playerOnlineText.text = "";
        for (int i = 0; i < Server.clients.Count; i++)
        {
            playerOnlineText.text += $"Player[{i}]: {Server.clients[i].EndPoint}\n";
        }

        playerQueueText.text = "";
        for (int i = 0; i < Server.queueClients.Count; i++)
        {
            playerQueueText.text += $"Player[{i}]: {Server.queueClients[i].EndPoint}\n";
        }
    }

    private void UpdatePlayersOnline(NetPeer peer, DisconnectInfo disconnectinfo)
    {
        playerOnlineText.text = "";
        for (int i = 0; i < Server.clients.Count; i++)
        {
            playerOnlineText.text += $"Player[{i}]: {Server.clients[i].EndPoint}";
        }

        playerQueueText.text = "";
        for (int i = 0; i < Server.queueClients.Count; i++)
        {
            playerQueueText.text += $"Player[{i}]: {Server.queueClients[i].EndPoint}";
        }
    }

    private void UpdatePlayersOnline(NetPeer peer)
    {
        playerOnlineText.text = "";
        for (int i = 0; i < Server.clients.Count; i++)
        {
            playerOnlineText.text += $"Player[{i}]: {Server.clients[i].EndPoint} \n";
        }
    }

    public void WriteConsole(string text)
    {
        consoleText.text +=$"<color=#32a852>[{DateTime.Now}]</color>: {text} \n";
    }
}
