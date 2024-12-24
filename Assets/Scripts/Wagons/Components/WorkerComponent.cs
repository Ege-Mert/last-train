using UnityEngine;

public class WorkerComponent : MonoBehaviour
{
    [SerializeField] private int currentWorkers = 0;
    [SerializeField] private int baseMaxWorkers = 5;
    private int maxWorkersBonus = 0;
    private CentralHumanManager humanManager;
    
    public void Initialize(CentralHumanManager chm)
    {
        humanManager = chm;
    }

    public bool AddWorkers(int count)
    {
        if (count <= 0) return false;

        int available = humanManager.GetAvailableHumans();
        // Use actual max workers (base + bonus)
        if (available >= count && (currentWorkers + count) <= GetMaxWorkers())
        {
            bool assigned = humanManager.AssignWorkers(this, count);
            if (assigned)
            {
                currentWorkers += count;
                Debug.Log($"Added {count} workers. Current: {currentWorkers}/{GetMaxWorkers()}");
                return true;
            }
        }
        Debug.Log($"Failed to add workers. Available: {available}, Current: {currentWorkers}, Max: {GetMaxWorkers()}");
        return false;
    }

    public bool RemoveWorkers(int count)
    {
        if (count <= 0) return false;
        if (currentWorkers >= count)
        {
            bool removed = humanManager.RemoveWorkers(this, count);
            if (removed)
            {
                currentWorkers -= count;
                Debug.Log($"Removed {count} workers. Current: {currentWorkers}/{GetMaxWorkers()}");
                return true;
            }
        }
        return false;
    }

    public void SetMaxWorkersBonus(int bonus)
    {
        int oldMax = GetMaxWorkers();
        maxWorkersBonus = bonus;
        Debug.Log($"Max workers updated: {oldMax} -> {GetMaxWorkers()} (Bonus: {bonus})");
    }

    public int GetMaxWorkers()
    {
        return baseMaxWorkers + maxWorkersBonus;
    }

    private void OnDestroy()
    {
        if (humanManager != null && currentWorkers > 0)
        {
            humanManager.HandleWagonDestruction(this);
        }
    }
        public float GetEfficiency()
    {
        return 1f + (0.2f * currentWorkers);
    }

    public int GetCurrentWorkers()
    {
        return currentWorkers;
    }
    
}