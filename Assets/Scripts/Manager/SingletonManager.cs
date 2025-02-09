using UnityEngine;
using UnityEngine.Tilemaps;

public class SingletonManager : MonoBehaviour
{
    public static SingletonManager instance;

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
