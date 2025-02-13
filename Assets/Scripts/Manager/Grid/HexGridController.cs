using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class HexGridController : MonoBehaviour
{
    public int xRadius = 5; // 地图半径（六边形环数）
    public int yRadius = 5; // 地图半径（六边形环数）
    public int grassCount = 20;
    public int mushroomCount = 3;
    public int largeLeafCount = 3;
    private Tilemap _groundTilemap;
    private Tilemap _decorationTilemap;
    public HexTerrainTile[] groundTiles; // 预配置的地形Tile
    public HexTerrainTile[] grassTiles; // 预配置的地形Tile
    public HexTerrainTile[] mushroomTiles; // 预配置的地形Tile
    public HexTerrainTile[] largeLeafTiles; // 预配置的地形Tile
    
    public GameObject hexMaskPrefab;

    private void Awake()
    {
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        _groundTilemap = tilemaps[0];
        _decorationTilemap = tilemaps[1];
        
        TilemapSingleton.instance.SetTilemap(_groundTilemap);
    }

    private void Start()
    {
        hexMaskPrefab = Instantiate(hexMaskPrefab);
        hexMaskPrefab.SetActive(false);
        
        // GenerateTiles();
        Dictionary<Vector3Int, HexTerrainTile> tileMap = new Dictionary<Vector3Int, HexTerrainTile>();
        for (int i = -xRadius; i <= xRadius; i++)
        {
            for (int j = -yRadius; j <= yRadius; j++)
            {
                Vector3Int cellPos = new Vector3Int(i, j, 0);
                HexTerrainTile tile = _groundTilemap.GetTile<HexTerrainTile>(cellPos);
                tileMap.Add(new Vector3Int(i, j, -i-j), tile);
            }
        }

        // GenerateTiles();
        // GenerateGrassTiles();
        BoardManager.instance.tileDic = tileMap;
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = _groundTilemap.WorldToCell(mousePos);
        
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
                HexTerrainTile tile = GetGroundTileByLogic(); // 根据逻辑选择Tile
                _groundTilemap.SetTile(cellPos, tile);
            }
        }

        GenerateGrassTiles();
    }

    private void GenerateGrassTiles()
    {
        Dictionary<Vector3Int, bool> dic = new Dictionary<Vector3Int, bool>();
        for (int i = 0; i < grassCount; i++)
        {
            int xIndex = Random.Range(-xRadius, xRadius);
            int yIndex = Random.Range(-yRadius, yRadius);
            Vector3Int key = new Vector3Int(xIndex, yIndex, 0);
            if (dic.TryGetValue(key, out _))
            {
                continue;
            }

            HexTerrainTile tile = GetGrassTileByLogic();
            _decorationTilemap.SetTile(key, tile);
            dic.Add(key, true);
        }
    }

    private HexTerrainTile GetGroundTileByLogic() {
        // 示例：根据坐标生成随机地形
        int randomIndex = Random.Range(0, groundTiles.Length);
        return groundTiles[randomIndex];
    }
    
    private HexTerrainTile GetGrassTileByLogic() {
        // 示例：根据坐标生成随机地形
        int randomIndex = Random.Range(0, grassTiles.Length);
        return grassTiles[randomIndex];
    }
    
    private HexTerrainTile GetMushRoomTileByLogic() {
        // 示例：根据坐标生成随机地形
        int randomIndex = Random.Range(0, mushroomTiles.Length);
        return mushroomTiles[randomIndex];
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
