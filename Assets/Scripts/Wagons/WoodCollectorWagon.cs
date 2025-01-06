using UnityEngine;

public class WoodCollectorWagon : Wagon, IWagonProduction
{
    private WorkerComponent workerComponent;
    private CollectorComponent collectorComponent;
    
    private float productionInterval = 1f; // Configurable

    
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
        
        gm.GetWagonProductionManager().RegisterWagon(this);

    }
    public bool CanProduce()
    {
        var durability = GetComponent<DurabilityComponent>();
        return durability == null || !durability.IsBroken();
    }
    
    public float GetProductionInterval()
    {
        return productionInterval;
    }
    
    public void ProcessProduction(float deltaTime)
    {
        // Check durability
        var durability = GetComponent<DurabilityComponent>();
        if (durability != null && durability.IsBroken())
        {
            // Wagon is broken, do not collect
            return;
        }
        // Collect resources each frame for demonstration purposes
        // In a real game, might collect only if train is moving or under certain conditions
        if (collectorComponent != null)
        {
            collectorComponent.CollectResources(Time.deltaTime);
        }
    }
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.GetWagonProductionManager().UnregisterWagon(this);
        }
    }
    

}
