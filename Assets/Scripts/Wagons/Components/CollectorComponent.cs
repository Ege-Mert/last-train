using UnityEngine;

public class CollectorComponent : MonoBehaviour
{
    [SerializeField] private float baseCollectionRate = 1f; // wood per second
    private WorkerComponent workerComponent;
    private GameManager gameManager;
    private float bonusCollectionRate = 0f;


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
        gameManager.GetResourceManager().AddResource(ResourceType.WOOD, amountCollected);
    }
}
