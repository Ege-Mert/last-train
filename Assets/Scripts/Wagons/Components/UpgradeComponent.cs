using UnityEngine;

public class UpgradeComponent : MonoBehaviour
{
    [SerializeField] private WagonUpgradeData upgradeData;
    [SerializeField] private int currentLevel = 1;

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
        bool bonusApplied = ApplyCurrentLevelBonus();
        Debug.Log($"{wagon.name} upgraded to level {currentLevel}. Bonus applied: {bonusApplied}");
        return true;
    }

    private bool ApplyCurrentLevelBonus()
    {
        var bonus = upgradeData.GetUpgradeBonus(currentLevel);
        if (bonus == null)
        {
            Debug.LogError("No upgrade bonus found for level " + currentLevel);
            return false;
        }

        bool anyBonusApplied = false;

        // Get all potential components
        var collector = GetComponent<CollectorComponent>();
        var worker = GetComponent<WorkerComponent>();
        var storage = GetComponent<StorageComponent>();
        var humanCapacity = GetComponent<HumanCapacityComponent>();
        var converter = GetComponent<ConverterComponent>();

        // Apply bonuses to whatever components exist
        if (collector != null)
        {
            collector.SetBonusCollectionRate(bonus.collectionRateBonus);
            Debug.Log($"Applied collection rate bonus: {bonus.collectionRateBonus}");
            anyBonusApplied = true;
        }

        if (worker != null)
        {
            worker.SetMaxWorkersBonus((int)bonus.maxWorkersBonus);
            Debug.Log($"Applied max workers bonus: {bonus.maxWorkersBonus}");
            anyBonusApplied = true;
        }

        if (storage != null)
        {
            storage.SetBonusCapacity(bonus.storageCapacityBonus);
            Debug.Log($"Applied storage capacity bonus: {bonus.storageCapacityBonus}");
            anyBonusApplied = true;
        }

        if (humanCapacity != null)
        {
            humanCapacity.SetBonusCapacity((int)bonus.maxWorkersBonus);
            Debug.Log($"Applied human capacity bonus: {bonus.maxWorkersBonus}");
            anyBonusApplied = true;
        }
        if (converter != null)
    {
        // Set the converter to produce the specified coal quality resource
        converter.SetCoalQualityOutput(bonus.coalOutputResource);
        Debug.Log($"Set converter to produce: {bonus.coalOutputResource}");
        anyBonusApplied = true;
    }

        return anyBonusApplied;
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