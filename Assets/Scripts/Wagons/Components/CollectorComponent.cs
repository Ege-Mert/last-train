using UnityEngine;

public class CollectorComponent : MonoBehaviour
{
    [SerializeField] private ResourceType collectedResourceType = ResourceType.WOOD;
    [SerializeField] private float baseCollectionRate = 1f;
    private float bonusCollectionRate = 0f;
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

        float efficiency = workerComponent.GetEfficiency();
        float effectiveRate = (baseCollectionRate + bonusCollectionRate) * efficiency;
        float amountCollected = effectiveRate * deltaTime;

        gameManager.GetResourceManager().AddResource(collectedResourceType, amountCollected);
    }
}
