using UnityEngine;

public class EntityAnimationTriggers : MonoBehaviour
{
    private Entity entity => GetComponentInParent<Entity>();

    private void AnimationTrigger()
    {
        entity.AnimationTrigger();
    }
}