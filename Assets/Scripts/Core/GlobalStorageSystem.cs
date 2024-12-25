using System.Collections.Generic;
using UnityEngine;

public class GlobalStorageSystem
{
    private float baseCapacity;
    private float totalCapacityCached;
    private List<StorageComponent> storageComponents = new List<StorageComponent>();

    public void Initialize(float baseCap)
    {
        baseCapacity = baseCap;
        RecalculateCapacity();
    }

    public void AddStorageComponent(StorageComponent comp)
    {
        storageComponents.Add(comp);
        RecalculateCapacity();
    }

    public void RemoveStorageComponent(StorageComponent comp)
    {
        if (storageComponents.Remove(comp))
        {
            RecalculateCapacity();
        }
    }

    public void RecalculateCapacity()
    {
        float sum = baseCapacity;
        foreach (var comp in storageComponents)
        {
            sum += comp.GetMaxCapacity();
        }
        totalCapacityCached = sum;
        Debug.Log("GlobalStorageSystem: Recalculated total capacity = " + totalCapacityCached);
    }

    public float GetTotalCapacity()
    {
        return totalCapacityCached;
    }
}
