using Prefabs;
using UnityEngine;

[CreateAssetMenu(fileName = "HexRuleTile", menuName = "Tiles/HexRuleTile")]
public class HexRuleTile : HexagonalRuleTile, DataAccessible {
    public int moveCost = 2;    // 移动消耗
    public float defenseBonus = 0; // 防御加成
    public bool isWalkable = true;
    public HexTileType tileType = HexTileType.Ground;
    
    public int MoveCost()
    {
        return moveCost;
    }
}