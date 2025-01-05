using UnityEngine;

public class SleepingWagon : Wagon 
{
    public WagonUpgradeData upgradeData;
    private HumanCapacityComponent humanCapacityComponent;

    public override void Initialize(GameManager gm)
    {
        base.Initialize(gm);

        humanCapacityComponent = GetComponent<HumanCapacityComponent>();
        if (humanCapacityComponent != null)
        {
            humanCapacityComponent.Initialize(gm.GetCentralHumanManager()); // Assuming GameManager has GetHumanManager()
        }

        var upgradeComp = GetComponent<UpgradeComponent>();
        if (upgradeComp != null && upgradeData != null)
        {
            upgradeComp.Initialize(gm, this, upgradeData);
        }
    }
}