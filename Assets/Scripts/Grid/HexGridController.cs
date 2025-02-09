using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class HexGridController : MonoBehaviour
{
    public int xRadius = 5; // 地图半径（六边形环数）
    public int yRadius = 5; // 地图半径（六边形环数）
    private Tilemap _tilemap;
    public HexTerrainTile[] terrainTiles; // 预配置的地形Tile
    
    public GameObject hexMaskPrefab;
    
    private void Start()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        SingletonManager.instance.SetTilemap(_tilemap);
        
        hexMaskPrefab = Instantiate(hexMaskPrefab);
        hexMaskPrefab.SetActive(false);
        
        // GenerateTiles();
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = _tilemap.WorldToCell(mousePos);
        
        UpdateHexMask(cellPos);
    }

    private void GenerateTiles()
    {
        // 生成方块形状的六边形地形集合
        for (int i = -xRadius; i <= xRadius; i++)
        {
            for (int j = -yRadius; j <= yRadius; j++)
            {
                Vector3Int cellPos = new Vector3Int(i, j, 0);
                HexTerrainTile tile = GetTileByLogic(); // 根据逻辑选择Tile
                _tilemap.SetTile(cellPos, tile);
            }
        }
    }

    private HexTerrainTile GetTileByLogic() {
        // 示例：根据坐标生成随机地形
        int randomIndex = Random.Range(0, terrainTiles.Length);
        return terrainTiles[randomIndex];
    }
    
    /**
    * 更新mask位置
    */
    private void UpdateHexMask(Vector3Int cellPos)
    {
        TileBase tile = _tilemap.GetTile(new Vector3Int(cellPos.x, cellPos.y, 0));
        Vector3 center = _tilemap.CellToWorld(cellPos);
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
    
    // public Vector3 CellToWorld(Cell h)
    // {
    //     double x = 3.0 / 2.0 * h.x * qSize;
    //     double y = (Math.Sqrt(3.0) / 2.0 * h.x + Math.Sqrt(3.0) * h.y) * rSize;
    //     return new Vector3((float)x, (float)y);
    // }
}
