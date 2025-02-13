using UnityEngine;
using UnityEngine.Tilemaps;

public enum HexTileType
{
    Ground,
    Grass
}
[CreateAssetMenu(fileName = "HexTerrainTile", menuName = "Tiles/Hex Terrain Tile")]
public class HexTerrainTile : TileBase {
    public Sprite tileSprite;
    public int moveCost = 2;    // 移动消耗
    public float defenseBonus = 0; // 防御加成
    public bool isWalkable = true;
    public HexTileType tileType = HexTileType.Ground;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        tileData.sprite = tileSprite;
        tileData.colliderType = Tile.ColliderType.Sprite;
    }
}