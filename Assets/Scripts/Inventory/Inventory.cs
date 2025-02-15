using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    
    // 组件
    [Header("库存UI")] 
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;
    [SerializeField] private CharacterSpriteUI characterSpriteUI;

    // 数据
    public List<ItemData> startItems;
    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    private ItemSlotUI[] inventoryItemSlots;
    private ItemEquipmentSlotUI[] equipmentItemSlots;
    private ItemStatSlotUI[] statSlots;
    private float lastTimeUsedFlask;
    private float lastTimeUsedArmor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventoryItems = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>();
        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<ItemEquipmentSlotUI>();
        statSlots = statSlotParent.GetComponentsInChildren<ItemStatSlotUI>();

        for (int i = 0; i < startItems.Count; i++)
        {
            AddItem(startItems[i]);
        }
    }

    public void Equip(ItemData itemData)
    {
        Entity currentEntity = TurnOrderManager.instance.GetCurrentEntity();
        ItemEquipmentData itemEquipmentData = itemData as ItemEquipmentData;
        currentEntity.equipment.Equip(itemEquipmentData);
    }

    public void UnEquip(ItemEquipmentData itemEquipmentToDelete)
    {
        Entity currentEntity = TurnOrderManager.instance.GetCurrentEntity();
        currentEntity.equipment.Unequip(itemEquipmentToDelete);
    }

    private void UpdateModelToUI()
    {
        UpdateSlotsUI();

        Entity currentEntity = TurnOrderManager.instance.GetCurrentEntity();
        if (currentEntity != null && currentEntity.CanEquip())
        {
            if (currentEntity.equipment.itemEquipmentDatas.TryGetValue(EquipmentType.MainWeapon, out var equipment))
            {
                characterSpriteUI.UpdateSprite(currentEntity.playerSpriteSr.sprite,
                    currentEntity.equipment.itemEquipmentDatas[EquipmentType.MainWeapon].icon,
                    null, equipment.bTwoHanded);
            }
        }
    }

    private void UpdateSlotsUI()
    {
        Entity currentEntity = TurnOrderManager.instance.GetCurrentEntity();
        if (currentEntity != null && currentEntity.CanEquip())
        {
            Dictionary<EquipmentType,ItemEquipmentData> equipmentDatas = currentEntity.equipment.itemEquipmentDatas;
            for (int i = 0; i < equipmentItemSlots.Length; i++)
            {
                if (equipmentDatas.TryGetValue(EquipmentType.MainWeapon, out ItemEquipmentData mainWeapon) && i == 0)
                {
                    equipmentItemSlots[i].UpdateSlot(new InventoryItem(mainWeapon));
                }
                else if (equipmentDatas.TryGetValue(EquipmentType.DeputyWeapon, out ItemEquipmentData deputyWeapon) && i == 1)
                {
                    equipmentItemSlots[i].UpdateSlot(new InventoryItem(deputyWeapon));
                }
                else if (equipmentDatas.TryGetValue(EquipmentType.Armor, out ItemEquipmentData armor) && i == 2)
                {
                    equipmentItemSlots[i].UpdateSlot(new InventoryItem(armor));
                }
                else if (equipmentDatas.TryGetValue(EquipmentType.Amulet, out ItemEquipmentData amulet) && i == 3)
                {
                    equipmentItemSlots[i].UpdateSlot(new InventoryItem(amulet));
                }
            }
        }

        for (int i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanupSlot();
        }

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventoryItems[i]);
        }

        for (int i = 0; i < statSlots.Length; i++)
        {
            statSlots[i].UpdateStatValue();
        }
    }

    public void AddItem(ItemData item)
    {
        if (item == null)
        {
            return;
        }

        if (item.ItemType == ItemType.Equipment || item.ItemType == ItemType.Material)
        {
            AddInventoryItem(item);
        }

        UpdateModelToUI();
    }

    private void AddInventoryItem(ItemData item)
    {
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(item);
            inventoryItems.Add(newItem);
            inventoryDictionary.Add(item, newItem);
        }
    }

    public void RemoveItem(ItemData item)
    {
        if (item.ItemType == ItemType.Equipment || item.ItemType == ItemType.Material)
        {
            RemoveInventoryItem(item);
        }

        UpdateModelToUI();
    }

    private void RemoveInventoryItem(ItemData item)
    {
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            if (value.GetStack() <= 1)
            {
                inventoryItems.Remove(value);
                inventoryDictionary.Remove(item);
            }
            else
            {
                value.RemoveStack();
            }
        }
    }
}