using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using TMPro;
using UnityEngine;

public class SendMessage : MonoBehaviour
{
    private Chat chat;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private TextMeshProUGUI text;
    
    
    private void Awake()
    {
        chat = GetComponent<Chat>();
        ChatProcessor.evOnReceivedPacket += UpdateText;
    }

    private void UpdateText(ChatProtocol protocol)
    {
        chatText.text += $"<color=#AA0000>Client [{protocol.Message.Owner}]: </color>{protocol.Message.Text} \n";
    }

    public void Send()
    {
        if (text.text!="")
        {
            ChatProtocol p = new ChatProtocol();
            Message message = new Message();
            message.Text = text.text;
            p.Message = message;
            chat.SendPacketToServer(p,DeliveryMethod.ReliableUnordered);
            chatText.text += $"<color=#00AA00>You: </color>{text.text} \n";
        }
        

    }
}
