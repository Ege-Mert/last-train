using UnityEngine;

public class ConverterComponent : MonoBehaviour
{
    [SerializeField] private ResourceType inputResource = ResourceType.WOOD;
    [SerializeField] private ResourceType outputResource = ResourceType.COAL;
    [SerializeField] private float baseConversionRate = 0.5f; // amount of output resource per second if input is available
    private float bonusConversionRate = 0f;
    private WorkerComponent workerComponent;
    private GameManager gameManager;

    public void Initialize(GameManager gm, WorkerComponent wc)
    {
        gameManager = gm;
        workerComponent = wc;
    }

    public void SetBonusConversionRate(float bonus)
    {
        bonusConversionRate = bonus;
    }

    public void ConvertResources(float deltaTime)
    {
        if (gameManager == null || workerComponent == null) return;

        float efficiency = workerComponent.GetEfficiency();
        float effectiveRate = (baseConversionRate + bonusConversionRate) * efficiency * deltaTime;

        var rm = gameManager.GetResourceManager();

        // Check if we have enough input resource
        // For simplicity, convert 1:1 rate (1 unit input -> 1 unit output)
        // If you want a ratio, you can adjust accordingly.
        
        float inputAvailable = rm.GetResourceAmount(inputResource);
        float amountToConvert = Mathf.Min(effectiveRate, inputAvailable);

        if (amountToConvert > 0)
        {
            rm.RemoveResource(inputResource, amountToConvert);
            rm.AddResource(outputResource, amountToConvert);
        }
    }
}
