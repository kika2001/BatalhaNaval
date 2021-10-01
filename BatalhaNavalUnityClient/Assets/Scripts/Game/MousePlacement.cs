using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MousePlacement : MonoBehaviour
{
    private RaycastHit2D hit2D;
    private Ray2D ray;
    private float deltaX, deltaY;
    private Vector2 mousePosition;
    private GameObject ship;
    public LayerMask piecesLayer;
    
    public ShipManager shipManager;

    public Tilemap enemyTilemap;
    public TileBase waterTile;
    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,40,piecesLayer);
        if (!shipManager.allPlaced)
        {
            if (Input.GetMouseButtonDown(0))
            {
                
                if (ship)
                {
                    if (ship.GetComponent<Placement>().PlaceOnBoard())
                    {
                        //Destroy(ship.GetComponent<Placement>());
                        ship.GetComponent<Placement>().enabled = false;
                    }
                    else
                    {
                        ship.GetComponent<Placement>().ReturnToPlacement();
                    }
                    ship = null;
                }
                else
                {
                    if (hit2D)
                    {
                        ship = hit2D.transform.root.gameObject;
                        ship.GetComponent<Placement>().Pick();
                        deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - ship.transform.position.x;
                        deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - ship.transform.position.y;
                    }
                }
            }
            if (ship)
            {
                ship.transform.position = new Vector2(mousePosition.x-deltaX,mousePosition.y-deltaY);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ship.transform.Rotate(0,0,90f);
                }
            }
        }
        else
        {
            if (GameClient.Instance.myTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var postile = enemyTilemap.WorldToCell(pos);
                    if (enemyTilemap.GetTile(postile)==waterTile)
                    {
                        GameClient.Instance.SendAttack(postile,GameClient.Instance.myId);
                    }
                    
                }
            }
        }

    }
}
