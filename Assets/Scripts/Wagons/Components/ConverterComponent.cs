using UnityEngine;

public class ConverterComponent : MonoBehaviour
{
    [SerializeField] private ResourceType inputResource = ResourceType.WOOD;
    [SerializeField] private float baseConversionRate = 0.5f;

    private WorkerComponent workerComponent;
    private GameManager gameManager;

    // Current output resource, initially low quality coal
    private ResourceType currentOutputResource = ResourceType.COAL_LOW;

    public void Initialize(GameManager gm, WorkerComponent wc)
    {
        gameManager = gm;
        workerComponent = wc;
    }

    public void SetCoalQualityOutput(ResourceType newOutputType)
    {
        currentOutputResource = newOutputType;
    }

    public void ConvertResources(float deltaTime)
    {
        if (gameManager == null || workerComponent == null) return;
    
        // No workers => no conversion
        if (workerComponent.GetCurrentWorkers() <= 0)
        {
            return;
        }
    
        float efficiency = workerComponent.GetEfficiency();
        float effectiveRate = baseConversionRate * efficiency * deltaTime;
    
        var rm = gameManager.GetResourceManager();
    
        // Check input availability
        float inputAvailable = rm.GetResourceAmount(inputResource);
        float amountToConvert = Mathf.Min(effectiveRate, inputAvailable);
    
        if (amountToConvert > 0)
        {
            // Remove input resource
            rm.RemoveResource(inputResource, amountToConvert);
            // Attempt partial add of output
            rm.AddResourcePartial(currentOutputResource, amountToConvert);
            // No leftover penalty needed here, 
            // ResourceManager handles over-capacity events automatically.
        }
    }
}
