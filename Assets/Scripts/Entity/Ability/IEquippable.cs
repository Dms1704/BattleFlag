using System.Collections.Generic;

public interface IEquippable
{
    public bool IsDirty();
    public void Clean();
    Dictionary<EquipmentType, ItemEquipmentData> itemEquipmentDatas { get; set; }
    public void Equip(ItemEquipmentData equipment);
    public void Unequip(ItemEquipmentData equipment);
    public ItemEquipmentData GetEquipment(EquipmentType equipmentType);
    
}
