using UnityEngine;
using System.Collections.Generic;

public class StorageComponent : MonoBehaviour
{
    [SerializeField] private float baseCapacity = 50f;
    public float bonusCapacity = 0f;

    // For a generic storage, we may allow storing multiple resource types.
    // Let’s store the actual amounts in a dictionary.
    private Dictionary<ResourceType, float> storedResources = new Dictionary<ResourceType, float>();

    public void SetBonusCapacity(float bonus)
    {
        bonusCapacity = bonus;

    }

    public float GetMaxCapacity()
    {
        return baseCapacity + bonusCapacity;
    }

    public float GetUsedCapacity()
    {
        float total = 0f;
        foreach (var kvp in storedResources)
        {
            total += kvp.Value;
        }
        return total;
    }

    public bool CanStore(ResourceType type, float amount)
    {
        float availableSpace = GetMaxCapacity() - GetUsedCapacity();
        return availableSpace >= amount;
    }

    public bool StoreResource(ResourceType type, float amount)
    {
        var durability = GetComponent<DurabilityComponent>();
        if (durability != null && durability.IsBroken())
        {
            // Can't store or retrieve because wagon is broken
            return false;
        }
        if (!CanStore(type, amount)) return false;
        if (!storedResources.ContainsKey(type))
            storedResources[type] = 0f;

        storedResources[type] += amount;
        return true;
    }

    public float RetrieveResource(ResourceType type, float amount)
    {
        if (!storedResources.ContainsKey(type) || storedResources[type] < amount)
        {
            amount = storedResources.ContainsKey(type) ? storedResources[type] : 0f;
        }

        storedResources[type] -= amount;
        if (storedResources[type] <= 0f)
            storedResources.Remove(type);

        return amount;
    }

    // If needed, methods to get specific resource amounts or list stored resources.
}
