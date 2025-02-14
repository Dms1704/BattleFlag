using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<ItemData> startItems;
    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    public List<InventoryItem> stashItems;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    public List<InventoryItem> equipmentItems;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    [Header("库存UI")] [SerializeField]
    private Transform inventorySlotParent;

    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private ItemSlotUI[] inventoryItemSlots;
    private ItemSlotUI[] stashItemSlots;
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
        stashItems = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        equipmentItems = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<ItemSlotUI>();
        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<ItemEquipmentSlotUI>();
        statSlots = statSlotParent.GetComponentsInChildren<ItemStatSlotUI>();

        for (int i = 0; i < startItems.Count; i++)
        {
            AddItem(startItems[i]);
        }
    }

    public void Equip(ItemData itemData)
    {
        ItemData_Equipment equipmentItemData = itemData as ItemData_Equipment;
        InventoryItem inventoryItem = new InventoryItem(equipmentItemData);

        ItemData_Equipment oldItem = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == equipmentItemData.equipmentType)
            {
                oldItem = item.Key;
            }
        }

        if (oldItem != null)
        {
            UnEquip(oldItem);
            AddItem(oldItem);
        }

        equipmentItems.Add(inventoryItem);
        equipmentDictionary.Add(equipmentItemData, inventoryItem);

        equipmentItemData.AddModifiers();

        RemoveItem(itemData);
    }

    public void UnEquip(ItemData_Equipment itemToDelete)
    {
        if (itemToDelete == null)
        {
            return;
        }

        if (equipmentDictionary.TryGetValue(itemToDelete, out InventoryItem value))
        {
            equipmentItems.Remove(value);
            equipmentDictionary.Remove(itemToDelete);
            itemToDelete.RemoveModifiers();
        }
    }

    private void UpdateSlotsUI()
    {
        for (int i = 0; i < equipmentItemSlots.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentItemSlots[i].slotType)
                {
                    equipmentItemSlots[i].UpdateSlot(item.Value);
                }
            }
        }

        for (int i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanupSlot();
        }

        for (int i = 0; i < stashItemSlots.Length; i++)
        {
            stashItemSlots[i].CleanupSlot();
        }

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventoryItems[i]);
        }

        for (int i = 0; i < stashItems.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stashItems[i]);
        }

        // ͳ����ֵչʾ
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

        if (item.ItemType == ItemType.Equipment)
        {
            AddInventoryItem(item);
        }
        else if (item.ItemType == ItemType.Material)
        {
            AddStashItem(item);
        }

        UpdateSlotsUI();
    }

    private void AddStashItem(ItemData item)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(item);
            stashItems.Add(newItem);
            stashDictionary.Add(item, newItem);
        }
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
        if (item.ItemType == ItemType.Equipment)
        {
            RemoveInventoryItem(item);
        }
        else if (item.ItemType == ItemType.Material)
        {
            RemoveStashItem(item);
        }

        UpdateSlotsUI();
    }

    private void RemoveStashItem(ItemData item)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem value))
        {
            if (value.stackCount <= 1)
            {
                stashItems.Remove(value);
                stashDictionary.Remove(item);
            }
            else
            {
                value.RemoveStack();
            }
        }
    }

    private void RemoveInventoryItem(ItemData item)
    {
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            if (value.stackCount <= 1)
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

    public ItemData_Equipment GetEquipment(EquipmentType type)
    {
        ItemData_Equipment equipmentItemData = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == type)
            {
                equipmentItemData = item.Key;
            }
        }
        return equipmentItemData;
    }

    public void UseFlask()
    {
        ItemData_Equipment flaskItemData = GetEquipment(EquipmentType.Flask);
        if (flaskItemData == null)
        {
            return;
        }

        bool canUseFlask = (Time.time > flaskItemData.itemCooldown + lastTimeUsedFlask) || lastTimeUsedFlask == 0;
        if (canUseFlask)
        {
            flaskItemData.ItemEffect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
        {
            Debug.Log("Flask is on cooldown");
        }
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment armorItemData = GetEquipment(EquipmentType.Armor);
        if (armorItemData == null)
        {
            return false;
        }

        if ((Time.time > armorItemData.itemCooldown + lastTimeUsedArmor) || lastTimeUsedArmor == 0)
        {
            lastTimeUsedArmor = Time.time;
            return true;
        }
        Debug.Log("Armor is on cooldown");

        return false;
    }
}