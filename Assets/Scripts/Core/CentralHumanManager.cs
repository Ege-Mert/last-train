using System.Collections.Generic;
using UnityEngine;

public class CentralHumanManager
{
    // Total humans currently in the train
    private int totalHumans = 0;

    // Humans available for work (not assigned anywhere)
    private int availableHumans = 0;

    // Worker assignment dictionary
    private Dictionary<object, int> assignedWorkers = new Dictionary<object, int>();

    // NEW: total sleeping capacity across all wagons
    private int totalSleepingCapacity = 0;

    public void Initialize()
    {
        totalHumans = 0;
        availableHumans = 0;
        assignedWorkers.Clear();
        totalSleepingCapacity = 0;
    }

    #region Add/Remove Humans
    public void AddHumans(int count)
    {
        if (count > 0)
        {
            totalHumans += count;
            availableHumans += count;
            Debug.Log($"Added {count} humans. Total: {totalHumans}, Available: {availableHumans}");
        }
    }

    public bool AssignWorkers(object target, int count)
    {
        if (count <= 0) return false;
        if (availableHumans >= count)
        {
            availableHumans -= count;
            if (!assignedWorkers.ContainsKey(target))
                assignedWorkers[target] = 0;
            assignedWorkers[target] += count;
            Debug.Log($"Assigned {count} humans to {target}. Available now: {availableHumans}");
            return true;
        }
        return false;
    }

    public bool RemoveWorkers(object target, int count)
    {
        if (count <= 0 || !assignedWorkers.ContainsKey(target)) return false;

        int currentlyAssigned = assignedWorkers[target];
        if (currentlyAssigned >= count)
        {
            assignedWorkers[target] = currentlyAssigned - count;
            availableHumans += count;
            if (assignedWorkers[target] == 0)
                assignedWorkers.Remove(target);
            Debug.Log($"Removed {count} humans from {target}. Available now: {availableHumans}");
            return true;
        }
        return false;
    }

    public void HandleWagonDestruction(object target)
    {
        if (assignedWorkers.ContainsKey(target))
        {
            int workersToReassign = assignedWorkers[target];
            availableHumans += workersToReassign;
            assignedWorkers.Remove(target);
            Debug.Log($"Wagon destroyed: Reassigned {workersToReassign} workers back to available pool. Now available: {availableHumans}");
        }
    }
    #endregion

    #region Sleeping Capacity Tracking
    public void AddSleepingCapacity(int capacity)
    {
        totalSleepingCapacity += capacity;
        Debug.Log($"Added {capacity} sleeping capacity. Total capacity: {totalSleepingCapacity}");
    }

    public void RemoveSleepingCapacity(int capacity)
    {
        totalSleepingCapacity -= capacity;
        if (totalSleepingCapacity < 0) totalSleepingCapacity = 0; 
        Debug.Log($"Removed {capacity} sleeping capacity. Total capacity: {totalSleepingCapacity}");
    }

    public bool IsOverCapacity()
    {
        // true if totalHumans > totalSleepingCapacity
        return totalHumans > totalSleepingCapacity;
    }

    public int GetSleepingCapacity()
    {
        return totalSleepingCapacity;
    }
    #endregion

    #region Getters
    public int GetAvailableHumans()
    {
        return availableHumans;
    }

    public int GetTotalHumans()
    {
        return totalHumans;
    }
    #endregion
}
