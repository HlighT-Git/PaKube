using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileBlock : MonoBehaviour
{
    private TileMap tilemap;
    private TileMap.Node node;
    private TileCube tilecube;
    private TextMeshProUGUI costText;
    private GameObject holding;
    private int status;

    public TileMap Tilemap { get => tilemap; set => tilemap = value; }
    public TileMap.Node Node { get => node; set => node = value; }
    public TextMeshProUGUI CostText { get => costText; set => costText = value; }
    public int Status { get => status; set => status = value; }
    public GameObject Holding { get => holding; set => holding = value; }

    public void SetStatus(int status)
    {
        this.status = status;
        tilecube.SetColorByStatus();
    }
    private void Awake()
    {
        CostText = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        tilecube = this.gameObject.transform.GetChild(0).GetComponent<TileCube>();
    }
    public void InitTileBlockStatus()
    {
        Status = (Node.Cost != 0) ? 1 : 0;
        CostText.text = Node.Cost.ToString();
        // Color
        if (Node.Cost != 0)
            tilecube.OriginalColor = new Color((255 - 50 * Node.Cost) / 255f, 1, 1);
        tilecube.SetColor(tilecube.OriginalColor);
    }
}
