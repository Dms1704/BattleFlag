using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridController : MonoBehaviour
{
    public int xRadius = 5; // 地图半径（六边形环数）
    public int yRadius = 5; // 地图半径（六边形环数）
    private Tilemap _groundTilemap;
    
    public GameObject hexMaskPrefab;

    private void Awake()
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        _groundTilemap = tilemaps[0];
        
        TilemapSingleton.instance.SetTilemap(_groundTilemap);
    }

    private void Start()
    {
        hexMaskPrefab = Instantiate(hexMaskPrefab);
        hexMaskPrefab.SetActive(false);
        
        // GenerateTiles();
        Dictionary<Vector3Int, TileBase> tileMap = new Dictionary<Vector3Int, TileBase>();
        for (int i = -xRadius; i <= xRadius; i++)
        {
            for (int j = -yRadius; j <= yRadius; j++)
            {
                Vector3Int cellPos = new Vector3Int(i, j, 0);
                TileBase tileBase = _groundTilemap.GetTile<TileBase>(cellPos);
                tileMap.Add(new Vector3Int(i, j, -i-j), tileBase);
            }
        }

        BoardManager.instance.groundDic = tileMap;
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = _groundTilemap.WorldToCell(mousePos);
        
        UpdateHexMask(cellPos);
    }
    
    /**
    * 更新mask位置
    */
    private void UpdateHexMask(Vector3Int cellPos)
    {
        TileBase tile = BoardManager.instance.FindTile(new Vector3Int(cellPos.x, cellPos.y, -cellPos.x-cellPos.y));
        Vector3 center = _groundTilemap.CellToWorld(cellPos);
        if (tile != null)
        {
            hexMaskPrefab.transform.position = new Vector3(center.x, center.y, 0);
            hexMaskPrefab.SetActive(true); // 可根据需要显示/隐藏
        }
        else
        {
            hexMaskPrefab.SetActive(false); // 可根据需要显示/隐藏
        }
    }
}
