using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private UI ui;
    public InventoryItem item;

    private void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem newItem)
    {
        image.color = Color.white;

        item = newItem;

        if (item != null)
        {
            image.sprite = item.itemData.icon;

            text.text = item.GetStack() > 1 ? item.stackCount.ToString() : "";
        }
    }

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0)) {
    //         PointerEventData data = new PointerEventData(EventSystem.current);
    //         data.position = Input.mousePosition;
    //         List<RaycastResult> results = new List<RaycastResult>();
    //         EventSystem.current.RaycastAll(data, results);
    //         foreach (var result in results) {
    //             Debug.Log("Hit: " + result.gameObject.name);
    //         }
    //     }
    // }

    public void CleanupSlot()
    {
        item = null;
        image.sprite = null;
        image.color = Color.clear;
        text.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
        {
            return;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.itemData);
            return;
        }

        if (item.itemData.ItemType == ItemType.Equipment)
        {
            Inventory.instance.Equip(item.itemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
        {
            return;
        }

        // ui.itemTooltip.HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.itemData == null)
        {
            return;
        }

        // ui.itemTooltip.ShowTooltip(item.itemData as ItemData_Equipment);
    }
}