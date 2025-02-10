using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSingleton : MonoBehaviour
{
    public static TilemapSingleton instance;

    public Tilemap tilemap;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
        {
            instance = this;
        }
    }

    public void SetTilemap(Tilemap newTilemap)
    {
        if (tilemap == null)
        {
            tilemap = newTilemap;
        }
    }
}
