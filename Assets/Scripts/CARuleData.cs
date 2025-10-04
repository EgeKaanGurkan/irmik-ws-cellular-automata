using System;
using UnityEngine;

[System.Serializable]
public class CARule
{
    [Header("Layer Range")]
    public int startLayer;
    public int endLayer;

    [Header("Survival Rules (for alive cells)")]
    [Range(0, 8)]
    public int survivalMin = 2;
    [Range(0, 8)]
    public int survivalMax = 3;

    [Header("Birth Rules (for dead cells)")]
    [Range(0, 8)]
    public int birthMin = 3;
    [Range(0, 8)]
    public int birthMax = 3;

    [Header("Rule Name")]
    public string ruleName = "Custom Rule";

    public CARule()
    {
        startLayer = 0;
        endLayer = 19;
        survivalMin = 2;
        survivalMax = 3;
        birthMin = 3;
        birthMax = 3;
        ruleName = "Conway's Life";
    }

    public CARule(int start, int end, int survMin, int survMax, int bMin, int bMax, string name)
    {
        startLayer = start;
        endLayer = end;
        survivalMin = survMin;
        survivalMax = survMax;
        birthMin = bMin;
        birthMax = bMax;
        ruleName = name;
    }

    public bool IsInRange(int layer)
    {
        return layer >= startLayer && layer <= endLayer;
    }

    public string GetRuleDescription()
    {
        return $"{ruleName}: Layers {startLayer}-{endLayer} | Survival: {survivalMin}-{survivalMax} | Birth: {birthMin}-{birthMax}";
    }
}

[System.Serializable]
public class CARuleSet
{
    public CARule[] rules;

    public CARuleSet()
    {
        // Default rules matching your current implementation
        rules = new CARule[]
        {
            new CARule(0, 19, 2, 3, 3, 3, "Conway's Life"),
            new CARule(20, 39, 2, 4, 4, 6, "Extended Life"),
            new CARule(40, 59, 2, 5, 2, 5, "Permissive Life")
        };
    }

    public CARule GetRuleForLayer(int layer)
    {
        foreach (var rule in rules)
        {
            if (rule.IsInRange(layer))
                return rule;
        }

        // Default fallback rule
        return new CARule(layer, layer, 2, 3, 3, 3, "Default");
    }
}
