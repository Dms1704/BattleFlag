using UnityEngine;

public enum SkillType
{
    Melee
}
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    // Start is called before the first frame update
    
    public MeleeSkill meleeSkill;
    
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        meleeSkill = new MeleeSkill();
    }

    public Skill GetSkill(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Melee:
                return meleeSkill;
            default:
                return meleeSkill;
        }
    }
}
