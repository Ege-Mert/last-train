using UnityEngine;

public class WorkerComponent : MonoBehaviour
{
    [SerializeField] private int currentWorkers = 0;
    [SerializeField] private int maxWorkers = 5;
    private CentralHumanManager humanManager;
    private int maxWorkersBonus = 0;
    
    public void Initialize(CentralHumanManager chm)
    {
        humanManager = chm;
    }
    

    public bool AddWorkers(int count)
    {
        if (count <= 0) return false;

        int available = humanManager.GetAvailableHumans();
        if (available >= count && (currentWorkers + count) <= maxWorkers)
        {
            // Assign workers from humanManager
            bool assigned = humanManager.AssignWorkers(this, count);
            if (assigned)
            {
                currentWorkers += count;
                return true;
            }
        }
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
                return true;
            }
        }
        return false;
    }

    public float GetEfficiency()
    {
        return 1f + (0.2f * currentWorkers);
    }

    public int GetCurrentWorkers()
    {
        return currentWorkers;
    }
    
    public void SetMaxWorkersBonus(int bonus)
    {
        maxWorkersBonus = bonus;
    }


    public int GetMaxWorkers()
    {
        return maxWorkers;
    }
}
