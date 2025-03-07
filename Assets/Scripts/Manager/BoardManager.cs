using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    // 预制体
    public GameObject smallHexMaskPrefab;
    public GameObject redHexMaskPrefab;
    
    // 组件
    public static BoardManager instance;
    private Tilemap tilemap;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    
    // 数据
    [SerializeField] private bool InUnityEditorMode = true;
    public Dictionary<Vector3Int, TileBase> groundDic { get; set; }
    private IList<Entity> entities = new List<Entity>();
    private Dictionary<Vector3Int, Entity> entitiesDic = new();
    private IList<GameObject> smallHexMasks = new List<GameObject>();
    private IList<GameObject> redHexMasks = new List<GameObject>();
    [SerializeField] private int numberOfCharacters;
    private int initializedCount = 0;
    [SerializeField] private int entityCreatePosX = -3;
    [SerializeField] private int playerCreatePosY = -3;
    [SerializeField] private int enemyCreatePosY = 3;
    
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

        Initialize();
    }

    public void Initialize()
    {
        numberOfCharacters = 10;
        for (int i = 0; i < 5; i++)
        {
            Vector3 cellToWorld = tilemap.CellToWorld(new Vector3Int(entityCreatePosX + i, playerCreatePosY, 0));
            GameObject player = Instantiate(playerPrefab, cellToWorld, Quaternion.identity);
            // AddEntity(player.GetComponent<Entity>());
        }

        for (int i = 0; i < 5; i++)
        {
            Vector3 cellToWorld = tilemap.CellToWorld(new Vector3Int(entityCreatePosX + i, enemyCreatePosY, 0));
            GameObject enemy = Instantiate(enemyPrefab, cellToWorld, new Quaternion(0, 180, 0, 0));
            // AddEntity(enemy.GetComponent<Entity>());
        }
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
        entitiesDic.Remove(tilemap.WorldToCell(entity.transform.position));
    }

    public IList<Entity> GetEntities()
    {
        return this.entities;
    }

    /**
     * return 0 不胜不负
     *        1 胜利
     *        -1 输了
     */
    public int WinOrLose()
    {
        bool win = true;
        bool lose = true;
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].isAlly)
            {
                lose = false;
            }
            else
            {
                win = false;
            }
        }

        if (!lose && !win)
        {
            return 0;
        }
        if (win)
        {
            return 1;
        }

        return -1;
    }

    public void CurrentOperateOver()
    {
        TurnOrderManager.instance.GetCurrentEntity().OperateOver();
    }
    
    public void QuitGame()
    {
        if (InUnityEditorMode)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 在Unity编辑器中结束运行
            #endif
        }
        else
        {
            Application.Quit();
        }
    }
}
