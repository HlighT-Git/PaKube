using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [SerializeReference] private int maxHealth;
    [SerializeReference] private int health;
    [SerializeReference] private int attack;
    [SerializeReference] private int shield;
    public int Health { get => health; set => health = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Shield { get => shield; set => shield = value; }

    private void Awake()
    {
        maxHealth = 10;
        health = maxHealth;
        attack = 5; 
        shield = 5;
    }
    public TileBlock CurrentTileBlockStanding()
    {
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        TileMap tm = this.GetComponent<CharacterActions>().Map.GetComponent<TileMap>();
        Vector2 index = tm.GetGraphIndexByCoordinates(x, y);
        return tm.Graph[(int)index.x, (int)index.y].TileBlock;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
