using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemStatSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UI ui;

    [SerializeField] private string statName;

    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI statValueText;
    [SerializeField] private TextMeshProUGUI statNameText;

    [TextArea]
    [SerializeField] private string description;

    private void OnValidate()
    {
        gameObject.name = "Stat - " + statName;

        if (statNameText != null)
        {
            statNameText.text = statName;
        }
    }

    void Start()
    {
        ui = GetComponentInParent<UI>();

        UpdateStatValue();
    }

    public void UpdateStatValue()
    {
        CharacterStats stats = TurnOrderManager.instance.GetCurrentEntity().GetComponent<CharacterStats>();

        if (stats != null)
        {
            statValueText.text = stats.GetStat(statType).GetValue().ToString();

            if (statType == StatType.health)
            {
                statValueText.text = stats.CalculatedMaxHealth().ToString();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ui.statTooltip.HideTooltip();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ui.statTooltip.ShowTooltip(description);
    }
}