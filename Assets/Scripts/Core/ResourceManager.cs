using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public event Action OnCapacityBecameOver;
    public event Action OnCapacityBecameFree;
    
    private bool isOverCapacity = false;

    
    private ResourceSettings resourceSettings;
    private Dictionary<ResourceType, float> resourceAmounts = new Dictionary<ResourceType, float>();
    private GlobalStorageSystem globalStorageSystem; // newly added
    

    public void Initialize(ResourceSettings settings, GlobalStorageSystem storageSystem)
    {
        resourceSettings = settings;
        globalStorageSystem = storageSystem;

        // Initialize all resource amounts to 0
        foreach (ResourceType rt in System.Enum.GetValues(typeof(ResourceType)))
        {
            resourceAmounts[rt] = 0f;
        }
    }
    
    public bool GetIsOverCapacity()
    {
        return isOverCapacity;
    }

    public bool CanAddResource(ResourceType type, float amount)
    {
        // In future, check storage limits if any
        return amount >= 0f; // for now, always true
    }

    public float AddResourcePartial(ResourceType type, float amount)
    {
        if (amount <= 0f) return 0f;

        float currentStored = GetTotalStoredResources();
        float maxCap = globalStorageSystem.GetTotalCapacity();

        float spaceAvailable = maxCap - currentStored;
        if (spaceAvailable <= 0f)
        {
            // Already over or exactly at capacity
            // If we weren't previously over, we are now
            CheckAndFireOverCapacity(true);
            return 0f;
        }

        float amountToAdd = Mathf.Min(spaceAvailable, amount);
        resourceAmounts[type] += amountToAdd;

        // If amountToAdd < amount, we definitely hit capacity
        if (amountToAdd < amount)
        {
            CheckAndFireOverCapacity(true);
        }
        else
        {
            // We successfully stored all. We might be exactly at capacity or below it.
            float newStored = currentStored + amountToAdd;
            if (newStored < maxCap)
            {
                CheckAndFireOverCapacity(false);
            }
            else
            {
                // If exactly at capacity, we might consider that over or not. 
                // Typically “over capacity” means strictly > max, but you can treat ‘== max’ as over.
                bool newlyOver = (newStored > maxCap); 
                CheckAndFireOverCapacity(newlyOver);
            }
        }
        return amountToAdd;
    }

    public bool CanRemoveResource(ResourceType type, float amount)
    {
        if (resourceAmounts[type] >= amount)
            return true;
        return false;
    }

    public bool RemoveResource(ResourceType type, float amount)
    {
        if (resourceAmounts[type] >= amount)
        {
            resourceAmounts[type] -= amount;

            // Removing might free capacity if we were over or at capacity
            CheckAndFireOverCapacity(false);

            return true;
        }
        return false;
    }

    private void CheckAndFireOverCapacity(bool newIsOver)
    {
        if (newIsOver != isOverCapacity)
        {
            // We changed capacity status
            isOverCapacity = newIsOver;
            if (isOverCapacity)
            {
                OnCapacityBecameOver?.Invoke();
            }
            else
            {
                OnCapacityBecameFree?.Invoke();
            }
        }
    }

    public float GetResourceAmount(ResourceType type)
    {
        return resourceAmounts[type];
    }

    public float GetTotalStoredResources()
    {
        float sum = 0f;
        foreach (var kvp in resourceAmounts)
        {
            sum += kvp.Value;
        }
        return sum;
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
    
    public float GetWeightCapacity()
    {
        return globalStorageSystem.GetTotalCapacity();
    }
}
