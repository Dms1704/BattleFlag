using UnityEngine.EventSystems;

public class ItemEquipmentSlotUI : ItemSlotUI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "Equiment slot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
        {
            return;
        }
        Inventory.instance.UnEquip(item.itemData as ItemEquipmentData);
        CleanupSlot();
    }
}