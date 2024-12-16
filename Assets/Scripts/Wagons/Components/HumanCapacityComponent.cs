using UnityEngine;

public class HumanCapacityComponent : MonoBehaviour
{
    [SerializeField] private int baseCapacity = 10;
    private int bonusCapacity = 0;

    public void SetBonusCapacity(int bonus)
    {
        bonusCapacity = bonus;
    }

    public int GetMaxCapacity()
    {
        return baseCapacity + bonusCapacity;
    }

    // In a full implementation, we’d track how many humans are currently housed.
    // For now, just assume `CentralHumanManager` or another system uses this info.
}
