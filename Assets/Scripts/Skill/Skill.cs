using UnityEngine;

public class Skill
{
    public int scope = 2;
    public int cost = 4;

    protected Entity _entity;

    public Skill()
    {
    }

    public Skill(Entity entity)
    {
        _entity = entity;
    }

    public virtual bool CanUseSkill()
    {
        return true;
    }

    public virtual void UseSkill(Entity entity)
    {
        if (!CanUseSkill())
        {
            Debug.Log("技能冷却中");
            return;
        }
        
        entity.CostActionPoint(cost);
    }
}