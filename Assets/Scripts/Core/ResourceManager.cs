using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private ResourceSettings resourceSettings;
    private Dictionary<ResourceType, float> resourceAmounts = new Dictionary<ResourceType, float>();

    public void Initialize(ResourceSettings settings)
    {
        resourceSettings = settings;
        // Initialize all resources to 0
        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceAmounts[rt] = 0f;
        }
    }

    public bool CanAddResource(ResourceType type, float amount)
    {
        // In future, check storage limits if any
        return amount >= 0f; // for now, always true
    }

    public void AddResource(ResourceType type, float amount)
    {
        if (!CanAddResource(type, amount))
            return;

        resourceAmounts[type] += amount;
        Debug.Log($"Added {amount} of {type}. Total: {resourceAmounts[type]}");
    }

    public bool CanRemoveResource(ResourceType type, float amount)
    {
        if (resourceAmounts[type] >= amount)
            return true;
        return false;
    }

    public bool RemoveResource(ResourceType type, float amount)
    {
        if (CanRemoveResource(type, amount))
        {
            resourceAmounts[type] -= amount;
            Debug.Log($"Removed {amount} of {type}. Remaining: {resourceAmounts[type]}");
            return true;
        }
        return false;
    }

    public float GetResourceAmount(ResourceType type)
    {
        return resourceAmounts[type];
    }

    public float GetTotalWeight()
    {
        float total = 0f;
        foreach (var kvp in resourceAmounts)
        {
            float weightPerUnit = resourceSettings.GetResourceWeight(kvp.Key);
            total += kvp.Value * weightPerUnit;
        }
        return total;
    }
}
