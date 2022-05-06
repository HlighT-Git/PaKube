using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouse : MonoBehaviour
{
    private int previousStatus;
    private TileBlock tileBlockParent;

    private void Awake()
    {
        tileBlockParent = this.gameObject.GetComponent<TileCube>().TileBlockParent;
    }

    private void OnMouseDown()
    {
        if (tileBlockParent.Status == 0
            || tileBlockParent.Status == 3
            || tileBlockParent.Status == 4)
            return;
        TileMap tilemap = tileBlockParent.Tilemap;
        CharacterActions ca = tilemap.Player.GetComponent<CharacterActions>();

        tileBlockParent.Tilemap.ResetMap();

        TileBlock startBlock = tilemap.Player.GetComponent<CharacterStatus>().CurrentTileBlockStanding();
        TileBlock endBlock = tileBlockParent;
        startBlock.SetStatus(3);
        endBlock.SetStatus(4);
        ca.Moving(startBlock, endBlock);
    }
    private void OnMouseOver()
    {
        if (tileBlockParent.Status == 0
            || tileBlockParent.Status == 3
            || tileBlockParent.Status == 4)
            return;
        if (tileBlockParent.Status != 2)
            previousStatus = tileBlockParent.Status;
        tileBlockParent.SetStatus(2);
    }
    private void OnMouseExit()
    {
        if (tileBlockParent.Status == 2)
        {
            tileBlockParent.SetStatus(previousStatus);
        }
    }
}
