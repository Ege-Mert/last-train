using UnityEngine;

[System.Serializable]
public class UpgradeCost
{
    public int level;  
    public ResourceAmount[] costs;
}

[System.Serializable]
public class UpgradeBonus
{
    public int level;
    public float collectionRateBonus; // example for collector wagons
    public int maxWorkersBonus;       // example for worker capacity
    public float storageCapacityBonus; // for storage wagons
    // Add fields as needed for other wagons
}

[CreateAssetMenu(fileName = "WagonUpgradeData", menuName = "last-train/Wagon Upgrade Data")]
public class WagonUpgradeData : ScriptableObject
{
    public int maxLevel = 3;
    public UpgradeCost[] upgradeCosts;
    public UpgradeBonus[] upgradeBonuses;

    public ResourceAmount[] GetUpgradeCost(int currentLevel)
    {
        // find costs for currentLevel + 1
        // assuming upgradeCosts are sorted by level
        foreach (var cost in upgradeCosts)
        {
            if (cost.level == currentLevel + 1)
                return cost.costs;
        }
        return null;
    }

    public UpgradeBonus GetUpgradeBonus(int level)
    {
        foreach (var bonus in upgradeBonuses)
        {
            if (bonus.level == level)
                return bonus;
        }
        return null;
    }

    public bool CanUpgrade(int currentLevel)
    {
        return currentLevel < maxLevel;
    }
}
