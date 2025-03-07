using Prefabs;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum HexTileType
{
    Ground,
    Grass,
    Mushroom,
    Wood,
    Stone,
    BigLeaf,
    Stump
}
[CreateAssetMenu(fileName = "HexTile", menuName = "Tiles/HexTile")]
public class HexTile : TileBase, DataAccessible
{
    public Sprite tileSprite;
    public int moveCost = 2;    // 移动消耗
    public float defenseBonus = 0; // 防御加成
    public bool isWalkable = true;
    public HexTileType tileType = HexTileType.Ground;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = tileSprite;
        tileData.colliderType = Tile.ColliderType.Sprite;
    }
    public int MoveCost()
    {
        return moveCost;
    }
}
