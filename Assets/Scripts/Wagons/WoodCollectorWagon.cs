using UnityEngine;

public class WoodCollectorWagon : Wagon
{
    private WorkerComponent workerComponent;
    private CollectorComponent collectorComponent;

    public override void Initialize(GameManager gm)
    {
        base.Initialize(gm);

        workerComponent = GetComponent<WorkerComponent>();
        collectorComponent = GetComponent<CollectorComponent>();

        if (collectorComponent != null && workerComponent != null)
        {
            collectorComponent.Initialize(gm, workerComponent);
            
            // Initialize workerComponent with humanManager
            workerComponent.Initialize(gm.GetCentralHumanManager());
        }
        else
        {
            Debug.LogWarning("WoodCollectorWagon missing WorkerComponent or CollectorComponent.");
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
