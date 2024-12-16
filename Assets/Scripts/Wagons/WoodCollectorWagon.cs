using UnityEngine;

public class WoodCollectorWagon : Wagon
{
    private WorkerComponent workerComponent;
    private CollectorComponent collectorComponent;
    
    public WagonUpgradeData upgradeData;
    public override void Initialize(GameManager gm)
    {
        base.Initialize(gm);

        workerComponent = GetComponent<WorkerComponent>();
        collectorComponent = GetComponent<CollectorComponent>();

    if (collectorComponent != null && workerComponent != null)
    {
        collectorComponent.Initialize(gm, workerComponent);
        workerComponent.Initialize(gm.GetCentralHumanManager());
    }

    var upgradeComp = GetComponent<UpgradeComponent>();
    if (upgradeComp != null && upgradeData != null)
    {
        upgradeComp.Initialize(gm, this, upgradeData);
    }
    }

    void Update()
    {
        // Collect resources each frame for demonstration purposes
        // In a real game, might collect only if train is moving or under certain conditions
        if (collectorComponent != null)
        {
            collectorComponent.CollectResources(Time.deltaTime);
        }
    }
}
