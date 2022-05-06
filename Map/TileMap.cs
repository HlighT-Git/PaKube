using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public class Node
    {
        private TileBlock tileBlock;
        private List<Node> neighbours;
        private AStarPathFinding.Node searchNode;
        private int x;
        private int y;
        private int cost;
        private int[,] costTo;

        public Node(int cost, int mapSizeX, int mapSizeY)
        {
            Neighbours = new List<Node>();
            this.Cost = cost;
            costTo = new int[mapSizeX, mapSizeY];
        }

        public TileBlock TileBlock { get => tileBlock; set => tileBlock = value; }
        public List<Node> Neighbours { get => neighbours; set => neighbours = value; }
        public AStarPathFinding.Node SearchNode { get => searchNode; set => searchNode = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Cost { get => cost; set => cost = value; }
        public int[,] CostTo { get => costTo; set => costTo = value; }
    }
    private class Character
    {
        private GameObject gameObj;
        private int x;
        private int y;

        public Character(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public GameObject GameObj { get => gameObj; set => gameObj = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
    }
    // Start is called before the first frame update
    private int mapSizeX = 10;
    private int mapSizeY = 10;
    private int mapRootX = -5;
    private int mapRootY = -5;
    private Node[,] graph;
    private Color[] tileColors;
    [SerializeReference] private GameObject tileVisualPrefab;
    [SerializeReference] private GameObject player;
    [SerializeReference] private GameObject enemyVisualPrefab;
    private Character[] chars;
    private int numberOfEnemies = 1;

    public int MapSizeX { get => mapSizeX; set => mapSizeX = value; }
    public int MapSizeY { get => mapSizeY; set => mapSizeY = value; }
    public int MapRootX { get => mapRootX; set => mapRootX = value; }
    public int MapRootY { get => mapRootY; set => mapRootY = value; }
    public Node[,] Graph { get => graph; set => graph = value; }
    public Color[] TileColors { get => tileColors; set => tileColors = value; }
    public GameObject Player { get => player; set => player = value; }
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SetUpTileColors();
    }
    public void GenerateMap()
    {
        GenerateMapData();
        GenerateMapVisual();
        GenerateGraph();
        Floyd.CalculateHeuristicForAStar(this);
        InitWorldStatus();
    }
    void Start()
    {
        GenerateMap();
    }
    private void SetUpTileColors()
    {
        TileColors = new Color[8];
        /*
         * 0: Non-use.
         * 1: Normal.
         * 2: Hover.
         * 3: Start.
         * 4: End.
         * 5: In queue.
         * 6: In path.
         * 7: Visited.
         */
        TileColors[0] = Color.black;
        TileColors[1] = Color.white;
        TileColors[2] = Color.blue;
        TileColors[3] = Color.green;
        TileColors[4] = Color.red;
        TileColors[5] = new Color(1, 1, 128 / 255f);
        TileColors[6] = new Color(1, 140 / 255f, 0);
        TileColors[7] = new Color(0, 128 / 255f, 0);
    }
    private void GenerateMapData()
    {
        graph = new Node[MapSizeX, MapSizeY];
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                graph[x, y] = new Node(Random.Range(0, 6), MapSizeX, MapSizeY);
            }
        }

        chars = new Character[numberOfEnemies + 1];
        bool[,] occupied = new bool[MapSizeX, MapSizeY];

        int playerX, playerY;
        playerX = Random.Range(0, MapSizeX);
        playerY = Random.Range(0, MapSizeY);
        graph[playerX, playerY].Cost = Random.Range(1, 6);
        occupied[playerX, playerY] = true;
        chars[0] = new Character(playerX, playerY);
        chars[0].GameObj = player;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            int enemyX, enemyY;
            enemyX = Random.Range(0, MapSizeX);
            enemyY = Random.Range(0, MapSizeY);
            while (occupied[enemyX, enemyY])
            {
                enemyX = Random.Range(0, MapSizeX);
                enemyY = Random.Range(0, MapSizeY);
            }
            graph[enemyX, enemyY].Cost = Random.Range(1, 6);
            occupied[enemyX, enemyY] = true;
            chars[i+1] = new Character(enemyX, enemyY);
        }
    }
    private void GenerateGraph()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                if (graph[x, y].Cost == 0)
                    continue;
                if (x > 0 && graph[x - 1, y].Cost != 0)
                    graph[x, y].Neighbours.Add(graph[x - 1, y]);
                if (y > 0 && graph[x, y - 1].Cost != 0)
                    graph[x, y].Neighbours.Add(graph[x, y - 1]);
                if (x < MapSizeX - 1 && graph[x + 1, y].Cost != 0)
                    graph[x, y].Neighbours.Add(graph[x + 1, y]);
                if (y < MapSizeY - 1 && graph[x, y + 1].Cost != 0)
                    graph[x, y].Neighbours.Add(graph[x, y + 1]);
            }
        }
    }
        public void ResetMap()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                if (graph[x, y].Cost == 0)
                    continue;
                graph[x, y].SearchNode = null;
                graph[x, y].TileBlock.SetStatus(1);
            }
        }
    }
    private void GenerateMapVisual()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                graph[x, y].X = x + MapRootX;
                graph[x, y].Y = y + MapRootY;
                GameObject go = Instantiate(tileVisualPrefab, new Vector3(graph[x, y].X, graph[x, y].Y), Quaternion.identity);
                TileBlock tb = go.GetComponent<TileBlock>();
                tb.Tilemap = this;
                tb.Node = graph[x, y];
                graph[x, y].TileBlock = tb;
                tb.InitTileBlockStatus();
            }
        }
        for (int i = 0; i < numberOfEnemies; i++)
        {
            int x = chars[i + 1].X;
            int y = chars[i + 1].Y;
            chars[i+1].GameObj = Instantiate(enemyVisualPrefab, new Vector3(graph[x, y].X, graph[x, y].Y, -1), Quaternion.identity);
        }
    }
    private void InitWorldStatus()
    {
        for (int i = 0; i <= numberOfEnemies; i++)
        {
            int x = chars[i].X;
            int y = chars[i].Y;
            chars[i].GameObj.transform.position = new Vector3 (graph[x, y].X, graph[x, y].Y, -1);
            chars[i].GameObj.GetComponent<CharacterActions>().Map = this.gameObject;
            graph[x, y].TileBlock.Holding = chars[i].GameObj;
        }
    }
    public Vector2 GetGraphIndexByCoordinates(float x, float y)
    {
        return new Vector2(x - mapRootX, y - mapRootY);
    }

    public void DestroyMap()
    {
        for (int x = 0; x < MapSizeX; x++)
        {
            for (int y = 0; y < MapSizeY; y++)
            {
                Destroy(graph[x, y].TileBlock.gameObject);
            }
        }
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Destroy(chars[i + 1].GameObj);
        }
    }
}
