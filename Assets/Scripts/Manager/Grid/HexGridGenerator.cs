using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[Serializable]
public class TerrainWeight
{
    public TileBase tile;
    public float weight;
}
public class HexGridGenerator : MonoBehaviour
{
    // 组件
    [Header("组件")]
    public Tilemap ground;
    public Tilemap decoration;
    public TileBase groundTile;
    public TileBase[] grassTiles;

    // 配置
    [Header("配置")]
    public int width;
    public int height;
    public int seed;
    public bool useRandomSeed;
    [Range(0f, 1f)]
    public float grassProbability;
    public float lacunarity;
    public List<TerrainWeight> terrainWeights = new List<TerrainWeight>();

    // 数据
    private Dictionary<Vector3Int, bool> grid = new Dictionary<Vector3Int, bool>(); // true:ground false:grass
    
    public void GenerateHexGrid()
    {
        GenerateData();
        
        GenerateGround();
    }

    private void GenerateData()
    {
        if (!useRandomSeed)
        {
            seed = Time.time.GetHashCode();
        }
        Random.InitState(seed);
        
        grid = new Dictionary<Vector3Int, bool>(); // true:ground false:grass

        float randomX = Random.Range(-10000, 10000);
        for (int x = -width; x <= width; x++)
        {
            for (int y = -height; y <= height; y++)
            {
                float perlinNoise = Mathf.PerlinNoise(x * lacunarity + randomX, y * lacunarity + randomX);
                grid.Add(new Vector3Int(x, y, 0), !(perlinNoise < grassProbability));
            }
        }
    }

    private void GenerateGround()
    {
        CleanHexGrid();

        // 地面
        for (int x = -width; x <= width; x++)
        {
            for (int y = -height; y <= height; y++)
            {
                if (grid.TryGetValue(new Vector3Int(x, y), out bool value))
                {
                    TileBase tile = GetGrassTileByLogic();
                    ground.SetTile(new Vector3Int(x, y), tile);
                }
            }
        }
        
        // 装饰
        // float weightTotal = 0;
        // for (int x = 0; x < terrainWeights.Count; x++)
        // {
        //     weightTotal += terrainWeights[x].weight;
        // }
        // for (int x = -width; x < width; x++)
        // {
        //     for (int y = -height; y < height; y++)
        //     {
        //         float randValue = Random.Range(1f, weightTotal);
        //         float temp = 0;
        //
        //         for (int i = 0; i < terrainWeights.Count; i++)
        //         {
        //             temp += terrainWeights[i].weight;
        //             if (randValue < temp)
        //             {
        //                 decoration.SetTile(new Vector3Int(x, y), terrainWeights[i].tile);
        //                 break;
        //             }
        //         }
        //     }
        // }
    }

    public void CleanHexGrid()
    {
        ground.ClearAllTiles();
    }
    
    private TileBase GetGrassTileByLogic() {
        // 示例：根据坐标生成随机地形
        int randomIndex = Random.Range(0, grassTiles.Length);
        return grassTiles[randomIndex];
    }
}
