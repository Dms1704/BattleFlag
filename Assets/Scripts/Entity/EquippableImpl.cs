using System.Collections.Generic;

public class EquippableImpl : IEquippable
{
    private bool dirty = false;

    public Dictionary<EquipmentType, ItemEquipmentData> itemEquipmentDatas { get; set; } = new();

    public bool IsDirty()
    {
        return dirty;
    }

    public void Clean()
    {
        dirty = false;
    }
    
    public void Equip(ItemEquipmentData equipment)
    {
        // 如果装备主手双手武器，需要卸载副手
        // 如果装备主手单手武器，不用管副手
        if (equipment.bTwoHanded && equipment.equipmentType == EquipmentType.MainWeapon)
        {
            if (itemEquipmentDatas.TryGetValue(EquipmentType.DeputyWeapon, out ItemEquipmentData deputyWeapon))
            {
                Unequip(deputyWeapon);
            }
            if (itemEquipmentDatas.TryGetValue(EquipmentType.MainWeapon, out ItemEquipmentData mainWeapon))
            {
                Unequip(mainWeapon);
            }
            
            itemEquipmentDatas.Add(equipment.equipmentType, equipment);
            Inventory.instance.RemoveItem(equipment);
            
            equipment.AddModifiers();
            dirty = true;
        }
    }

    public void Unequip(ItemEquipmentData equipment)
    {
        if (equipment == null)
        {
            return;
        }

        itemEquipmentDatas.Remove(equipment.equipmentType);
        Inventory.instance.AddItem(equipment);
        
        equipment.RemoveModifiers();
        dirty = true;
    }

    public ItemEquipmentData GetEquipment(EquipmentType equipmentType)
    {
        return itemEquipmentDatas[equipmentType];
    }
}
