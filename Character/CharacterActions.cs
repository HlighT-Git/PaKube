using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    private GameObject map;
    public GameObject Map { get => map; set => map = value; }

    private void Awake()
    {
        this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
    public void JumpTo(int x, int y)
    {
        TileMap tm = map.GetComponent<TileMap>();
        float oldX = transform.position.x;
        float oldY = transform.position.y;
        Vector2 index = tm.GetGraphIndexByCoordinates(oldX, oldY);
        if (tm.Graph[(int)index.x, (int)index.y].TileBlock.Holding.CompareTag(this.tag))
        {
            tm.Graph[(int)index.x, (int)index.y].TileBlock.Holding = null;
        }

        this.transform.position = new Vector3(x, y, -1);
        index = tm.GetGraphIndexByCoordinates(x, y);
        GameObject go = tm.Graph[(int)index.x, (int)index.y].TileBlock.Holding;
        if (go != null && !this.CompareTag(go.tag))
        {
            Attacking(go);
            JumpTo((int)oldX, (int)oldY);
        }
        else
        {
            tm.Graph[(int)index.x, (int)index.y].TileBlock.Holding = this.gameObject;
        }
    }
    public void Moving(TileBlock startBlock, TileBlock endBlock)
    {
        StopCoroutine(this.GetComponent<AStarPathFinding>().FindPath(this.gameObject, startBlock, endBlock));
        StartCoroutine(this.GetComponent<AStarPathFinding>().FindPath(this.gameObject, startBlock, endBlock));
    }
    public void Attacking(GameObject other)
    {
        CharacterStatus thisCS = this.GetComponent<CharacterStatus>();
        CharacterStatus otherCS = other.GetComponent<CharacterStatus>();
        int damage = thisCS.Attack;
        if (damage <= otherCS.Shield)
        {
            otherCS.Shield -= damage;
        }
        else
        {
            damage -= otherCS.Shield;
            otherCS.Shield = 0;
            otherCS.Health -= damage;
        }
        if (otherCS.Health <= 0)
        {
            Destroy(other);
        }
    }
}
