using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ship : MonoBehaviour
{
    public ShipType type;
    public bool placed=false;
    public bool destroyed = false;
    public List<ShipPart> parts = new List<ShipPart>();
    public Tile hitShip;
    public bool CheckHit(Vector3Int pos)
    {
        foreach (var p in parts)
        {
            if (p.position==pos)
            {
                p.tile = hitShip;
                GetComponent<Placement>().tilemap.SetTile(pos,hitShip);
                UpdateState();
                return true;
                
            }
        }

        return false;
    }

    public bool DestroyShip()
    {
        foreach (var p in parts)
        {
            p.tile = hitShip;
            GetComponent<Placement>().tilemap.SetTile(p.position,hitShip);
        }
        UpdateState();
        return true;
    }

    private void UpdateState()
    {
        int count = 0;
        foreach (var p in parts)
        {
            if (p.tile == hitShip)
            {
                count++;
            }
        }

        if (count==parts.Count)
        {
            destroyed = true;
        }
        else
        {
            destroyed = false;
        }
    }
    public enum ShipType
    {
        PortaAvioes,
        Cargueiro,
        Cruseiro,
        Submarino
    }
}
[Serializable]
public class ShipPart
{
    public Vector3Int position;
    public Tile tile;

    public ShipPart(Vector3Int p, Tile t)
    {
        position = p;
        tile = t;
    }
}
