using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sylves;
using UnityEngine;

public class HexGridUtil
{
    private static HexGrid grid = new HexGrid(new Vector2(1, 0.519586f), HexOrientation.FlatTopped);
    
    public static Cell Vector3ToCell(Vector3Int pos)
    {
        return new Cell(pos.x, pos.y, pos.z);
    }
    
    public static Vector3Int CellToVectorInt(Cell cell)
    {
        return new Vector3Int(cell.x, cell.y, -cell.x-cell.y);
    }

    public static Vector3Int IgnoreZ(Vector3Int pos)
    {
        return new Vector3Int(-pos.x, pos.y, 0);
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
    
    public static IGrid GenerateGrid()
    {
        return new HexGrid(new Vector2(1, 0.519586f), HexOrientation.FlatTopped);
    }

    public static Vector3 GetCellCenter(Vector3Int cell)
    {
        return grid.GetCellCenter(Vector3ToCell(cell));
    }
    
    #region 寻路
    public static CellPath FindPath(Transform srcTransform, IGrid grid)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var origin = srcTransform.worldToLocalMatrix.MultiplyPoint3x4(ray.origin);
        var direction = srcTransform.worldToLocalMatrix.MultiplyVector(ray.direction);
        // 有这句才能确保寻路算法没有问题
        RaycastInfo? h = grid.Raycast(origin, direction).Cast<RaycastInfo?>().FirstOrDefault();
        var currentCell = h?.cell;

        var srcCell = new Cell(0, 0);
        CellPath path = null;
        if (currentCell != null && !srcCell.Equals(currentCell.Value))
        {
            // Debug.Log("终点目标：" + currentCell.Value);
            path = Pathfinding.FindPath(grid, srcCell, currentCell.Value, _ => true);
            // if (path != null)
            //     Debug.Log(path.ToString());
        }

        return path;
    }

    public static Vector3[] FindPathWithTargetPos(Transform srcTransform)
    {
        IGrid grid = new HexGrid(new Vector2(1, 0.519586f), HexOrientation.FlatTopped);
        CellPath path = FindPath(srcTransform, grid);
        return PathToTargetPos(grid, srcTransform, path);
    }

    public static Vector3[] PathToTargetPos(IGrid grid, Transform srcTransform, CellPath path)
    {
        IList<Step> steps = path.Steps;
        Vector3[] targetPoses = new Vector3[steps.Count];
        for (int i = 0; i < steps.Count; i++)
        {
            Cell dest = path.Steps[i].Dest;
            Vector3 end = srcTransform.TransformPoint(grid.GetCellCenter(dest));
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
}
