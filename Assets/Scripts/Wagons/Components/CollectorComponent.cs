using UnityEngine;

public class CollectorComponent : MonoBehaviour
{
    [SerializeField] private float baseCollectionRate = 1f; // wood per second
    private WorkerComponent workerComponent;
    private GameManager gameManager;

    public void Initialize(GameManager gm, WorkerComponent wc)
    {
        gameManager = gm;
        workerComponent = wc;
    }

    public void CollectResources(float deltaTime)
    {
        if (gameManager == null || workerComponent == null) return;

        // Calculate effective collection rate
        float efficiency = workerComponent.GetEfficiency();
        float effectiveRate = baseCollectionRate * efficiency;

        float amountCollected = effectiveRate * deltaTime;

        // Add resources to the ResourceManager
        gameManager.GetResourceManager().AddResource(ResourceType.WOOD, amountCollected);
    }
}
