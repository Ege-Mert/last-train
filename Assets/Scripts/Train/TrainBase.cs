using System;
using Unity.VisualScripting;
using UnityEngine;

public class TrainBase : MonoBehaviour
{
    public event Action OnWinCondition;

    
    [Header("Train Settings")]
    public float baseMotorForce = 10f;
    public float frictionCoefficient = 0.01f;
    public float coalConsumptionRate = 0.1f; // units of coal per second at full motor usage
    public float coalQualityMultiplier = 1f; // In future, this might depend on wagon upgrades
    public float maxSpeed = 20f; // just a cap for safety
    public float acceleration = 0f; // computed
    public float currentSpeed = 0f; // it was private
    private bool isSlowedDown = false;
    private float slowdownFactor = 0.7f;
    
    private bool winTriggered = false;
    
    [SerializeField] private Transform wagonParent;  // Add this field


    private GameManager gameManager;
    private TrainPhysics trainPhysics;
    
    private ResourceType[] coalPriorityOrder = 
    { 
        ResourceType.COAL_HIGH, 
        ResourceType.COAL_MED, 
        ResourceType.COAL_LOW 
    };
    
    public void Initialize(GameManager gm, TrainPhysics physics)
    {
        gameManager = gm;
        trainPhysics = physics;
        Debug.Log("TrainBase initialized.");
        
        var resourceManager = gm.GetResourceManager();
        resourceManager.OnCapacityBecameOver += HandleOverCapacity;
        resourceManager.OnCapacityBecameFree += HandleCapacityFreed;
    }

    void Update()
    {
        UpdateSpeed(Time.deltaTime);
        CheckIfWin();
    }


    private void UpdateSpeed(float deltaTime)
    {
        // 1. Consume coal to produce motor force
        float coalNeeded = coalConsumptionRate * deltaTime;
        bool hasCoal = TryConsumeCoal(coalNeeded);

        float motorForce = 0f;
        if (hasCoal)
        {
            motorForce = trainPhysics.CalculateMotorForce(baseMotorForce, coalQualityMultiplier, coalConsumptionRate);
        }

        // If slowed
        if (isSlowedDown)
        {
            motorForce *= slowdownFactor;
        }

        // 2. Calculate total weight
        float totalWeight = CalculateTotalWeight();

        // 3. Get terrain angle from current node or environment
        // For now, let's assume flat (0 angle). In future, integrate with MapManager and current node angle.
        float angle = GetCurrentTerrainAngle();

        // 4. Calculate friction and gravity
        float frictionForce = trainPhysics.CalculateFrictionForce(totalWeight, frictionCoefficient);
        float gravityForce = trainPhysics.CalculateGravityForce(totalWeight, angle);

        // 5. Get total force
        float totalForce = trainPhysics.GetTotalForce(motorForce, frictionForce, gravityForce);

        // 6. acceleration = totalForce / mass (assume totalWeight is mass for simplicity)
        // If totalWeight = mass. Otherwise, we might need a separate mass variable.
        // Let's treat totalWeight as mass for now.
        if (totalWeight <= 0f) totalWeight = 1f; // avoid division by zero
        acceleration = totalForce / totalWeight;

        // 7. Update speed
        currentSpeed += acceleration * deltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed); // no negative speed or above max

        // 8. Update distance traveled in GameProgressManager
        float distanceDelta = currentSpeed * deltaTime;
        gameManager.GetGameProgressManager().UpdateDistance(distanceDelta);
    }

    private float CalculateTotalWeight()
    {
        // Weight from wagons + resources + base train weight if any
        // For simplicity, let's say wagons each have their own base weight plus resources.
        // We'll sum all active wagons' weights + ResourceManager total weight if needed.
        
        float wagonWeight = 0f;
        var wagons = gameManager.GetWagonManager().GetActiveWagons();
        foreach (var w in wagons)
        {
            wagonWeight += w.GetWeight();
        }

        // Add resource weight if distinct from wagon storage. 
        // If wagons store resources and their weight methods account for that, skip this.
        // Otherwise, we could also do:
        // float resourceWeight = gameManager.GetResourceManager().GetTotalWeight();
        // For now, assume wagon weight includes any contained resource weight if implemented.

        float totalWeight = wagonWeight; // If you have a base locomotive weight, add it here.
        return totalWeight;
    }
    
    private bool TryConsumeCoal(float amount)
    {
        var rm = gameManager.GetResourceManager();

        foreach (var coalType in coalPriorityOrder)
        {
            // Implement CanRemoveResource if needed, or just try RemoveResource directly
            if (rm.RemoveResource(coalType, amount))
            {
                AdjustCoalQualityMultiplier(coalType);
                return true;
            }
        }

        coalQualityMultiplier = 1.0f;
        return false;
    }

    private void AdjustCoalQualityMultiplier(ResourceType coalType)
    {
        if (coalType == ResourceType.COAL_HIGH) coalQualityMultiplier = 1.5f;
        else if (coalType == ResourceType.COAL_MED) coalQualityMultiplier = 1.2f;
        else coalQualityMultiplier = 1.0f;
    }

    private float GetCurrentTerrainAngle()
    {
        float trainDistance = gameManager.GetGameProgressManager().GetDistance();
        MapNode currentNode = gameManager.GetMapManager().GetCurrentNode(trainDistance);
        if (currentNode != null)
        {
            return currentNode.terrainAngle;
        }
        return 0f;
    }
    private void CheckIfWin() // We can refactor this in future if need so it doesnt check every frame but I am tired rn
    {
        float trainDistance = gameManager.GetGameProgressManager().GetDistance();
        float finalDistance = gameManager.gameBalanceData.finalDestinationDistance;

        if (!winTriggered && trainDistance >= finalDistance)
        {
            winTriggered = true;
            OnWinCondition?.Invoke();
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    public int GetMaxSpeed()
    {
        return (int)maxSpeed;
    }
    public Transform GetWagonParent()
    {
        return wagonParent;
    }
    
    private void HandleOverCapacity()
    {
        // Start slowdown
        isSlowedDown = true;
        Debug.Log("Train slowed down due to over capacity.");
    }

    private void HandleCapacityFreed()
    {
        // End slowdown
        isSlowedDown = false;
        Debug.Log("Train no longer slowed, capacity freed.");
    }
}
