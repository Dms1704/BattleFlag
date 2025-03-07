using UnityEngine;

public class ItemEffect : ScriptableObject
{
    public virtual void ExcuteEffect(Transform enemyTransform)
    {
        Debug.Log("装备效果");
    }
}