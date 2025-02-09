using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat : IComparable
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalValue = baseValue;

        foreach (var modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }

    public int GetBaseValue()
    {
        return baseValue;
    }

    public void SetDefaultValue(int defaultValue)
    {
        baseValue = defaultValue;
    }

    public void AddModifier(int modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier)
    {
        modifiers.Remove(modifier);
    }

    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }

    public int CompareTo(object obj)
    {
        if (obj is Stat)
        {
            return baseValue.CompareTo(((Stat)obj).GetBaseValue());
        }

        return 1;
    }
}