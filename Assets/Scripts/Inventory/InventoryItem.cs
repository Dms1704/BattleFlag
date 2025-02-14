using System;

[Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int stackCount;

    public InventoryItem(ItemData itemData)
    {
        this.itemData = itemData;
        AddStack();
    }

    public void AddStack() => stackCount++;
    public void RemoveStack() => stackCount--;
}