using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using ProtocolBatalhaNaval;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public ShipManager shipManager;
    public TextMeshProUGUI turnText;
    public GameObject blockPanel;
    public Tilemap myTilemap;
    public Tilemap enemyTilemap;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase missedTile;
    [SerializeField] private TileBase hitTile;
    [SerializeField] private TileBase shipTile;
    public GameObject waitingOpponent;
    private void Awake()
    {
        GameClient.Instance.evOnTurn += OnTurn;
        GameClient.Instance.evOnOpponentTurn += OnOpponentTurn;
        GameClient.Instance.evOnAttackedReceived += OnAttackReceived;
        GameClient.Instance.evOnAnswerReceived += OnAnswerReceived;
    }

    private void OnAnswerReceived(Protocol_BN p)
    {
        switch (p.action.hitInfo)
        {
           case HitInfo.Ship:
               enemyTilemap.SetTile(new Vector3Int(p.action.shot_x, p.action.shot_y, 0),hitTile);
               break;
           case HitInfo.Water:
               enemyTilemap.SetTile(new Vector3Int(p.action.shot_x, p.action.shot_y, 0),missedTile);
               break;
        }
        
    }

    private void OnAttackReceived(Protocol_BN p)
    {
        var tile = myTilemap.GetTile(new Vector3Int(p.action.shot_x, p.action.shot_y, 0));
        if (tile==waterTile)
        {
            myTilemap.SetTile(new Vector3Int(p.action.shot_x, p.action.shot_y, 0),missedTile);
            GameClient.Instance.SendAnswer(p,HitInfo.Water);
        }else if (tile ==shipTile)
        {
            shipManager.HittedShip(new Vector3Int(p.action.shot_x, p.action.shot_y, 0));
            myTilemap.SetTile(new Vector3Int(p.action.shot_x, p.action.shot_y, 0),hitTile);
            GameClient.Instance.SendAnswer(p,HitInfo.Ship);
        }
    }


    public void OnTurn()
    {
        waitingOpponent.SetActive(false);
        Debug.Log("OnTurn");
        turnText.text = "Player Turn:<color=#32a852> You </color>";
        blockPanel.SetActive(false);
    }

    public void OnOpponentTurn()
    {
        waitingOpponent.SetActive(false);
        Debug.Log("OnOpponentTurn");
        turnText.text = "Player Turn:<color=#a83232> Opponent </color>";
        blockPanel.SetActive(true);
    }
}
