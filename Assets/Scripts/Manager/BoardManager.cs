using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    // 预制体
    public GameObject smallHexMaskPrefab;
    public GameObject redHexMaskPrefab;
    
    // 组件
    public static BoardManager instance;
    private Tilemap tilemap;
    
    // 数据
    public Dictionary<Vector3Int, TileBase> groundDic { get; set; }
    private IList<Entity> entities = new List<Entity>();
    private Dictionary<Vector3Int, Entity> entitiesDic = new();
    private IList<GameObject> smallHexMasks = new List<GameObject>();
    private IList<GameObject> redHexMasks = new List<GameObject>();
    [SerializeField] private int numberOfCharacters = 3;
    private int initializedCount = 0;
    
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
    
    public void HandleCharacterInitialized()
    {
        initializedCount++;
        if (initializedCount == numberOfCharacters)
        {
            AllCharactersLoaded();
        }
    }
    
    private void AllCharactersLoaded()
    {
        Debug.Log("所有角色加载完成！");
        // 执行后续代码...
        
        TurnOrderManager.instance.StartTurn();
    }

    public void ClearHexMasks()
    {
        for (int i = 0; i < smallHexMasks.Count; i++)
        {
            Destroy(smallHexMasks[i]);
        }
        smallHexMasks.Clear();
        for (int i = 0; i < redHexMasks.Count; i++)
        {
            Destroy(redHexMasks[i]);
        }
        redHexMasks.Clear();
    }

    public void GenerateAttackHexMasks(Transform transform, int scope, List<Entity> enemies)
    {
        ClearHexMasks();

        IList<Vector3Int> list = HexGridUtil.FindScope(scope);
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 cellCenter = transform.TransformPoint(HexGridUtil.GetCellCenter(list[i]));
            Vector3Int tilePos = tilemap.WorldToCell(cellCenter);

            if (entitiesDic.TryGetValue(tilePos, out Entity enemy) && enemy is Enemy)
            {
                enemies.Add(enemy);
                GameObject redHexMask = Instantiate(redHexMaskPrefab);
                redHexMask.transform.position = cellCenter;
                redHexMasks.Add(redHexMask);
            }
            else
            {
                // if (scope == 1)
                // {
                //     continue;
                // }
                GameObject smallHexMask = Instantiate(smallHexMaskPrefab);
                smallHexMask.transform.position = cellCenter;
                smallHexMasks.Add(smallHexMask);
            }
        }
    }

    public TileBase FindTile(Vector3Int position)
    {
        return groundDic.GetValueOrDefault(position);
    }

    public Dictionary<Vector3Int, Entity> GetEntitiesDic()
    {
        return entitiesDic;
    }

    public Entity GetEntity(Vector3Int position)
    {
        return entitiesDic.GetValueOrDefault(position);
    }

    public void RemoveEntityPosition(Vector3Int position)
    {
        entitiesDic.Remove(position);
    }

    public void UpdateEntityPosition(Vector3Int position, Entity entity)
    {
        entitiesDic.Add(position, entity);
    }

    public void AddEntity(Entity entity)
    {
        this.entities.Add(entity);
        entitiesDic.Add(tilemap.WorldToCell(entity.transform.position), entity);
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
