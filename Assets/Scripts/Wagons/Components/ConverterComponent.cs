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

        // Check if there are any workers assigned
        if (workerComponent.GetCurrentWorkers() <= 0)
        {
            // No workers means no conversion
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
            rm.RemoveResource(inputResource, amountToConvert);
            rm.AddResource(currentOutputResource, amountToConvert);
        }
    }
}
