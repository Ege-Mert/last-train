using UnityEngine;

public class CollectorComponent : MonoBehaviour
{
    [SerializeField] private ResourceType collectedResourceType = ResourceType.WOOD;
    [SerializeField] private float baseCollectionRate = 1f;
    public float bonusCollectionRate = 0f;
    private float effectiveRate;
    
    private WorkerComponent workerComponent;
    private GameManager gameManager;

    public void Initialize(GameManager gm, WorkerComponent wc)
    {
        gameManager = gm;
        workerComponent = wc;
    }

    public void SetBonusCollectionRate(float bonus) 
    {
        bonusCollectionRate = bonus;
    }

    public void CollectResources(float deltaTime)
    {
        if (gameManager == null || workerComponent == null) return;
        // Check if workers are assigned
        if (workerComponent.GetCurrentWorkers() <= 0) return;

        float efficiency = workerComponent.GetEfficiency();
        effectiveRate = (baseCollectionRate + bonusCollectionRate) * efficiency;
        float amountCollected = effectiveRate * deltaTime;

        // We rely on partial-add logic in ResourceManager
        // ResourceManager will raise events if capacity is exceeded,
        // so no leftover calculation or penalty call is needed here.
        gameManager.GetResourceManager().AddResourcePartial(collectedResourceType, amountCollected);
    }
    
    public float GetCollectionRate()
    {
        return effectiveRate;
    }
}
