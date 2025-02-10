using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;
    protected float cooldownTimer;
    
    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool CanUseSkill()
    {
        return cooldownTimer < 0;
    }

    public virtual void UseSkill()
    {
        if (CanUseSkill())
        {
            UseSkill();
            cooldownTimer = cooldown;
        }
    }
}