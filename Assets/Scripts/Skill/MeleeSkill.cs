/**
 * 近战攻击
 */
public class MeleeSkill : Skill
{
    private Entity entity;

    public MeleeSkill(Entity entity) : base(entity)
    {
        this.entity = entity;
    }

    public MeleeSkill()
    {
    }

    public override void UseSkill(Entity entity)
    {
        base.UseSkill(entity);
    }
}
