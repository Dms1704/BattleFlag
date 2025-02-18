using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityStatsUI : MonoBehaviour
{
    private Entity entity;
    private CharacterStats stats;
    private Slider healthSlider;
    private Slider healthEffectSlider;
    private Slider actionPointSlider;
    private Slider actionPointsEffectSlider;
    
    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        int idx = 0;
        healthEffectSlider = GetComponentsInChildren<Slider>()[idx++];
        healthSlider = GetComponentsInChildren<Slider>()[idx++];
        actionPointsEffectSlider = GetComponentsInChildren<Slider>()[idx++];
        actionPointSlider = GetComponentsInChildren<Slider>()[idx];
        stats = GetComponentInParent<CharacterStats>();
        
        stats.onHealthChanged += UpdateHealthUI;
        stats.onActionPointChanged += UpdateActionPointsUI;
        entity.stats.UpdateStats();
        healthEffectSlider.interactable = false;
        healthSlider.interactable = false;
        actionPointsEffectSlider.interactable = false;
        actionPointSlider.interactable = false;
    }

    private void UpdateHealthUI()
    {
        healthSlider.maxValue = stats.CalculatedMaxHealth();
        healthEffectSlider.maxValue = stats.CalculatedMaxHealth();
        
        healthSlider.value = stats.currentHealth;
        StartCoroutine(SliderCacheChange(healthSlider, healthEffectSlider, .03f));
    }
    
    private void UpdateActionPointsUI()
    {
        actionPointSlider.maxValue = stats.CalculatedMaxActionPoint();
        actionPointsEffectSlider.maxValue = stats.CalculatedMaxActionPoint();
        
        actionPointSlider.value = stats.actionPoint.GetValue();
        StartCoroutine(SliderCacheChange(actionPointSlider, actionPointsEffectSlider, .005f));
    }

    private IEnumerator SliderCacheChange(Slider fill, Slider fillEffect, float speed)
    {
        float timeToMove = (fillEffect.value - fill.value) / speed;
        if (timeToMove < 0)
        {
            fillEffect.value = fill.value;
        }
        for (int i = 0; i < timeToMove; i++)
        {
            if (fillEffect.value > fill.value)
            {
                fillEffect.value -= speed;
            }
            yield return null;
        }
    }
    
    private void OnDisable()
    {
        stats.onHealthChanged -= UpdateHealthUI;
        stats.onActionPointChanged -= UpdateActionPointsUI;
    }
}