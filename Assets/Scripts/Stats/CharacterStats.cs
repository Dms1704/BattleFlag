using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StatType
{
    damage,
    critChance,
    critPower,
    health,

    armor,
    evasion,

    velocity,
    actionPoint
}
public class CharacterStats : MonoBehaviour
{
    private Entity entity;
    private EntityFX fx;
    
    [Header("行动能力")]
    public Stat velocity;
    public Stat actionPoint;    // 行动点数

    [Header("防御属性")]
    public Stat armor;
    public Stat evasion;    // 闪避

    [Header("伤害属性")]
    public Stat damage;
    public Stat critPower;
    public Stat critChance;

    [Header("生命值")]
    public int currentHealth;
    public Stat maxHealth;
        
    private bool isDead;

    public System.Action onHealthChanged;
    public System.Action onActionPointChanged;

    protected virtual void Start()
    {
        entity = GetComponent<Entity>();
        critPower.SetDefaultValue(150);
        currentHealth = CalculatedMaxHealth();

        fx = GetComponent<EntityFX>();

        onHealthChanged?.Invoke();
        onActionPointChanged?.Invoke();
    }

    protected virtual void Update()
    {

    }

    public void UpdateStats()
    {
        onHealthChanged?.Invoke();
        onActionPointChanged?.Invoke();
    }

    public virtual void IncreaseStatBy(int modifier, float duration, Stat statToModifier)
    {
        StartCoroutine(StartModCoroutine(duration, statToModifier, modifier));
    }

    private IEnumerator StartModCoroutine(float duration, Stat statToModifier, int modifier)
    {
        statToModifier.AddModifier(modifier);
        yield return new WaitForSeconds(duration);
        statToModifier.RemoveModifier(modifier);
    }

    public virtual void DoDamage(CharacterStats targetStats)
    {
        if (TargetCanAvoid(targetStats))
        {
            entity.StartCoroutine(nameof(Entity.KnockbackAndBack));
            return;
        }

        int totalDamage = damage.GetValue();
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(targetStats, totalDamage);
        targetStats.TakeDamage(totalDamage);
    }

    private bool TargetCanAvoid(CharacterStats targetStats)
    {
        int totalEvasion = targetStats.evasion.GetValue();
        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    public virtual void TakeDamage(int damage)
    {
        DecreaseHealthBy(damage);

        fx.StartCoroutine("FlashFX");
        entity.StartCoroutine(nameof(Entity.KnockbackAndBack));

        if (currentHealth <= 0 && !isDead)
        {
            Die();
            entity.Die();
        }
    }

    public virtual void IncreaseHealthBy(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > CalculatedMaxHealth())
        {
            currentHealth = CalculatedMaxHealth();
        }

        onHealthChanged?.Invoke();
    }

    protected virtual void DecreaseHealthBy(int damage)
    {
        currentHealth -= damage;
        onHealthChanged?.Invoke();
    }

    public virtual void CostActionPoint(int cost)
    {
        actionPoint.AddModifier(-cost);
        onActionPointChanged?.Invoke();
    }

    public virtual void RecoveryActionPoint()
    {
        actionPoint.RemoveAllModifiers();
        onActionPointChanged?.Invoke();
    }

    public virtual void Die()
    {
        isDead = true;
    }

    private int CheckTargetArmor(CharacterStats targetStats, int totalDamage)
    {
        totalDamage -= targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    public bool IsDead()
    {
        return isDead;
    }

    #region 数值计算
    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }
        return false;
    }

    private int CalculateCriticalDamage(int damage)
    {
        float totalCritPower = (critPower.GetValue());
        float critDamage = damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }

    public int CalculatedMaxHealth()
    {
        return maxHealth.GetValue();
    }
    
    public int CalculatedMaxActionPoint()
    {
        return actionPoint.GetBaseValue();
    }
    #endregion

    public Stat GetStat(StatType buffType)
    {
        switch (buffType)
        {
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critPower:
                return critPower;
            case StatType.health:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.actionPoint:
                return actionPoint;
        }
        return null;
    }
}
