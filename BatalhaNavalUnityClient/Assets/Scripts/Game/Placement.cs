using UnityEngine;
using UnityEngine.Tilemaps;

public class Placement : MonoBehaviour
{
    public Transform defaultPosition;
    public bool canBePlaced;

    public Tilemap tilemap;
    public Tile shipTile;
    public bool beingPicked=false;
   
    

    
    void Update()
    {
        if (beingPicked)
        {
            int invalid = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (tilemap.GetTile(tilemap.WorldToCell(transform.GetChild(i).position))==shipTile ||tilemap.GetTile(tilemap.WorldToCell(transform.GetChild(i).position))==null )
                {
                    invalid++;
                    transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.red;
                }
                else if(tilemap.GetTile(tilemap.WorldToCell(transform.GetChild(i).position)))
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                }
                
            }

            if (invalid>0)
            {
                canBePlaced = false;
                
            }
            else
            {
                canBePlaced = true;
            }
        }
    }

    public bool PlaceOnBoard()
    {
        var ship = GetComponent<Ship>();
        if (canBePlaced)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                tilemap.SetTile(tilemap.WorldToCell(transform.GetChild(i).position),shipTile);
                Tile tile = tilemap.GetTile<Tile>(tilemap.WorldToCell(transform.GetChild(i).position));
                ship.parts.Add(new ShipPart(tilemap.WorldToCell(transform.GetChild(i).position),tile));
                ship.placed = true;
                transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            }
            return true;
        }

        return false;
    }

    public void Pick()
    {
        beingPicked = true;
    }
    public void ReturnToPlacement()
    {
        for (int i = 0; i < transform.childCount; i++)
        { 
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
        }
        transform.position = defaultPosition.position;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        beingPicked = false;
    }
    
   
}