using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Sylves;
using UnityEngine;

public class HexGridUtil : MonoBehaviour
{
    private static readonly HexGrid _grid = new(new Vector2(1, 0.519586f), HexOrientation.FlatTopped);
    
    public static Cell Vector3ToCell(Vector3Int pos)
    {
        return new Cell(pos.x, pos.y, pos.z);
    }

    public static Vector3Int IgnoreZ(Vector3Int pos)
    {
        return new Vector3Int(pos.x, pos.y, 0);
    }

    /**
     * 六边形偏移坐标转换为轴坐标
     */
    public static Vector3Int OddQToAxial(Vector3Int pos)
    {
        int x = pos.y - (pos.x - pos.x & 1) / 2;
        int y = pos.x;
        return new Vector3Int(x, y);
    }
    
    /**
     * 轴坐标转换为六边形偏移坐标
     */
    public static Vector3Int AxialToOddQ(Vector3Int pos)
    {
        int x = pos.y + (pos.x - pos.x & 1) / 2;
        int y = pos.x;
        return new Vector3Int(x, y);
    }

    public static Vector3 GetCellCenter(Vector3Int cell)
    {
        return _grid.GetCellCenter(Vector3ToCell(cell));
    }
    
    public static Vector3 GetCellCenter(Cell cell)
    {
        return _grid.GetCellCenter(cell);
    }
    
    #region 寻路
    public static CellPath FindPath(Entity entity)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var origin = entity.transform.worldToLocalMatrix.MultiplyPoint3x4(ray.origin);
        var direction = entity.transform.worldToLocalMatrix.MultiplyVector(ray.direction);
        // 有这句才能确保寻路算法没有问题
        RaycastInfo? h = _grid.Raycast(origin, direction).Cast<RaycastInfo?>().FirstOrDefault();
        var currentCell = h?.cell;

        var srcCell = new Cell(0, 0);
        CellPath path = null;
        if (currentCell != null && !srcCell.Equals(currentCell.Value))
        {
            path = Pathfinding.FindPath(_grid, srcCell, currentCell.Value, IsAccessible);
        }
        
        bool IsAccessible(Cell cell)
        {
            // 这里在寻路算法里面，需要使用本地坐标
            Vector3 point = entity.transform.TransformPoint(GetCellCenter(cell));
            Vector3Int cellPos = IgnoreZ(TilemapSingleton.instance.tilemap.WorldToCell(point));
            Entity thisEntity = BoardManager.instance.GetEntity(cellPos);
            return !(thisEntity != null && thisEntity != entity);
        }

        return path;
    }

    public static Vector3[] PathToTargetPos(Transform srcTransform, CellPath path)
    {
        IList<Step> steps = path.Steps;
        Vector3[] targetPoses = new Vector3[steps.Count];
        for (int i = 0; i < steps.Count; i++)
        {
            Cell dest = path.Steps[i].Dest;
            Vector3 end = srcTransform.TransformPoint(GetCellCenter(dest));
            targetPoses[i] = end;
        }

        return targetPoses;
    }
    #endregion

    #region 范围
    public static IList<Vector3Int> FindScope(int scope)
    {
        IList<Vector3Int> allVecs = new List<Vector3Int>();
        for (int i = -scope; i <= scope; i++)
        {
            for (int j = -scope; j <= scope; j++)
            {
                for (int k = -scope; k <= scope; k++)
                {
                    if (i == 0 && j == 0 && k == 0)
                    {
                        continue;
                    }
                    if (i + j + k == 0)
                    {
                        allVecs.Add(new Vector3Int(i, j, k));
                    }
                }
            }
        }
       
        return allVecs;
    }
    #endregion

    #region 距离和邻居
    public static Entity FindClosestEntity(Entity entity)
    {
        Entity closest = null;
        int scope = 10;
        for (int i = -scope; i <= scope; i++)
        {
            for (int j = -scope; j <= scope; j++)
            {
                for (int k = -scope; k <= scope; k++)
                {
                    if (i == 0 && j == 0 && k == 0)
                    {
                        continue;
                    }
                    if (i + j + k == 0)
                    {
                        Vector3 point = entity.transform.TransformPoint(GetCellCenter(new Cell(i, j, k)));
                        closest = BoardManager.instance.GetEntity(TilemapSingleton.instance.tilemap.WorldToCell(point));
                        if (closest)
                        {
                            break;
                        }
                    }
                }
            }
        }
        return closest;
    }

    public static int Distance(Entity a, Entity b)
    {
        
        return 0;
    }
    #endregion
}
