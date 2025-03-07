using UnityEngine;

public class HexTileData : MonoBehaviour
{
    public int moveCost = 2;    // 移动消耗
    public float defenseBonus = 0; // 防御加成
    public bool isWalkable = true;
    public HexTileType tileType = HexTileType.Ground;
}