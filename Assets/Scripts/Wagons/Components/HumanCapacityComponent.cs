using UnityEngine;

public class HumanCapacityComponent : MonoBehaviour 
{
    [SerializeField] private int baseCapacity = 10;
    private int bonusCapacity = 0;
    private CentralHumanManager humanManager;

    public void Initialize(CentralHumanManager manager)
    {
        humanManager = manager;
        // Register initial capacity
        humanManager.AddSleepingCapacity(GetMaxCapacity());
    }

    public void SetBonusCapacity(int bonus)
    {
        int oldCapacity = GetMaxCapacity();
        bonusCapacity = bonus;
        int newCapacity = GetMaxCapacity();
        
        // Update the central manager with the capacity change
        if (humanManager != null)
        {
            humanManager.RemoveSleepingCapacity(oldCapacity);
            humanManager.AddSleepingCapacity(newCapacity);
        }
    }

    public int GetMaxCapacity()
    {
        return baseCapacity + bonusCapacity;
    }
    
    private void OnDestroy()
    {
        if (humanManager != null)
        {
            humanManager.RemoveSleepingCapacity(GetMaxCapacity());
        }
    }
}