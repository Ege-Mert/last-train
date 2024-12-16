using UnityEngine;

public class UpgradeComponent : MonoBehaviour
{
    [SerializeField] private WagonUpgradeData upgradeData;
    [SerializeField] private int currentLevel = 1; // starts at level 1

    private GameManager gameManager;
    private Wagon wagon;

    public void Initialize(GameManager gm, Wagon w, WagonUpgradeData data)
    {
        gameManager = gm;
        wagon = w;
        upgradeData = data;
        ApplyCurrentLevelBonus();
    }

    public bool TryUpgrade()
    {
        if (upgradeData == null) return false;
        if (!upgradeData.CanUpgrade(currentLevel)) return false;

        ResourceAmount[] costs = upgradeData.GetUpgradeCost(currentLevel);
        if (costs == null) return false;

        // Check resources
        var rm = gameManager.GetResourceManager();
        foreach (var cost in costs)
        {
            if (rm.GetResourceAmount(cost.type) < cost.amount)
                return false;
        }

        // Remove resources
        foreach (var cost in costs)
        {
            rm.RemoveResource(cost.type, cost.amount);
        }

        // Upgrade level
        currentLevel++;
        ApplyCurrentLevelBonus();
        Debug.Log($"{wagon.name} upgraded to level {currentLevel}.");
        return true;
    }

    private void ApplyCurrentLevelBonus()
    {
        var bonus = upgradeData.GetUpgradeBonus(currentLevel);
        if (bonus == null) return;

        // Apply bonuses to wagon components
        // For example, if this is a WoodCollectorWagon:
        WoodCollectorWagon woodWagon = wagon as WoodCollectorWagon;
        if (woodWagon != null)
        {
            var collector = woodWagon.GetComponent<CollectorComponent>();
            if (collector != null)
            {
                collector.SetBonusCollectionRate(bonus.collectionRateBonus);
            }

            var worker = woodWagon.GetComponent<WorkerComponent>();
            if (worker != null)
            {
                worker.SetMaxWorkersBonus((int)bonus.maxWorkersBonus);
            }
        }
            
        StorageWagon storageW = wagon as StorageWagon;
        if (storageW != null)
        {
            var storageComp = storageW.GetComponent<StorageComponent>();
            if (storageComp != null)
                storageComp.SetBonusCapacity(bonus.storageCapacityBonus);
        }

        SleepingWagon sleepingW = wagon as SleepingWagon;
        if (sleepingW != null)
        {
            var humanComp = sleepingW.GetComponent<HumanCapacityComponent>();
            if (humanComp != null)
                humanComp.SetBonusCapacity((int)bonus.maxWorkersBonus); // repurpose maxWorkersBonus as human capacity bonus if needed
        }


        // If it's a StorageWagon, apply storageCapacityBonus to StorageComponent, etc.
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetMaxLevel()
    {
        return upgradeData != null ? upgradeData.maxLevel : 1;
    }
}
