using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    MainWeapon,
    DeputyWeapon,
    Armor,
    Amulet,
}
[CreateAssetMenu(fileName = "equipment", menuName = "Data/equipment")]
public class ItemEquipmentData : ItemData
{
    public EquipmentType equipmentType;
    public bool bTwoHanded;
    [Header("效果")]
    public List<ItemEffect> effects;
    [TextArea]
    public string itemEffectDescription;

    [Header("行动能力")]
    public int velocity;
    public int actionPoints;
    
    [Header("防护数值")]
    public int health;
    public int armor;  
    public int evasion;  

    [Header("伤害")]
    public int damage;
    public int critPower; 
    public int critChance;

    [Header("装备冷却")]
    public float itemCooldown;

    private int descriptionLine;

    public void ItemEffect(Transform enemyTransform)
    {
        foreach (ItemEffect effect in effects)
        {
            effect.ExcuteEffect(enemyTransform);
        }
    }

    public void AddModifiers()
    {
        CharacterStats stats = TurnOrderManager.instance.GetCurrentEntity().GetComponent<CharacterStats>();
        
        stats.velocity.AddModifier(velocity);
        stats.actionPoint.AddModifier(actionPoints);
        
        stats.maxHealth.AddModifier(health);
        stats.armor.AddModifier(armor);
        stats.evasion.AddModifier(evasion);

        stats.damage.AddModifier(damage);
        stats.critPower.AddModifier(critPower);
        stats.critChance.AddModifier(critChance);
    }

    public void RemoveModifiers()
    {
        CharacterStats stats = TurnOrderManager.instance.GetCurrentEntity().GetComponent<CharacterStats>();
        
        stats.velocity.AddModifier(velocity);
        stats.actionPoint.AddModifier(actionPoints);
        
        stats.maxHealth.RemoveModifier(health);
        stats.armor.RemoveModifier(armor);
        stats.evasion.RemoveModifier(evasion);

        stats.damage.RemoveModifier(damage);
        stats.critPower.RemoveModifier(critPower);
        stats.critChance.RemoveModifier(critChance);
    }
    
    public override string ToString()
    {
        string str = base.ToString() + "\n";
        str += "equipmentType: " + equipmentType + "\n";
        str += "velocity: " + velocity + "\n";
        str += "actionPoints: " + actionPoints + "\n";
        str += "health: " + health + "\n";
        str += "armor: " + armor + "\n";
        str += "evasion: " + evasion + "\n";
        str += "damage: " + damage + "\n";
        str += "critPower: " + critPower + "\n";
        str += "critChance: " + critChance + "\n";
        str += "itemCooldown: " + itemCooldown + "\n";
        return str;
    }

    public override string DescriptionBuild()
    {
        sb.Length = 0;
        descriptionLine = 0;

        AddItemDescription(velocity, "velocity");
        AddItemDescription(actionPoints, "actionPoints");
        
        AddItemDescription(health, "health");
        AddItemDescription(armor, "armor");
        AddItemDescription(evasion, "evasion");

        AddItemDescription(damage, "damage");
        AddItemDescription(critPower, "critPower");
        AddItemDescription(critChance, "critChance");

        if (descriptionLine < 3)
        {
            for (int i = 0; i < 3 - descriptionLine; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        if (itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(itemEffectDescription);
        }

        return sb.ToString();
    }

    private void AddItemDescription(int value, string name)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if (value > 0)
            {
                sb.Append(name + ": " + value);
            }

            descriptionLine++;
        }
    }
}
