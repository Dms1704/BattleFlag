using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    // 预制体
    public GameObject smallHexMaskPrefab;
    
    // 组件
    public static BoardManager instance;
    private Tilemap tilemap;
    
    // 数据
    public Dictionary<Vector3Int, HexTerrainTile> tileDic { get; set; }
    private IList<Entity> entities = new List<Entity>();
    private IList<GameObject> smallHexMasks = new List<GameObject>();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 确保只有一个实例
        }
    }

    private void Start()
    {
        tilemap = TilemapSingleton.instance.tilemap;
    }

    public void ClearSmallHexMasks()
    {
        smallHexMasks.Clear();
    }

    public void GenerateSmallHexMasks(Transform transform, int scope)
    {
        IList<Vector3> list = HexGridUtil.FindScope(scope);
        for (int i = 0; i < list.Count; i++)
        {
            GameObject smallHexMask = Instantiate(smallHexMaskPrefab);
            smallHexMask.transform.position = transform.TransformPoint(list[i]);
        }
    }

    public HexTerrainTile FindTile(Vector3Int position)
    {
        return tileDic.GetValueOrDefault(position);
    }

    public void AddEntity(Entity entity)
    {
        this.entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        this.entities.Remove(entity);
    }

    public IList<Entity> GetEntities()
    {
        return this.entities;
    }
}
