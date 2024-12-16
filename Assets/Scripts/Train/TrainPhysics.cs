using UnityEngine;

public class TrainPhysics
{
    private GameManager gameManager;

    public void Initialize(GameManager gm)
    {
        gameManager = gm;
    }

    public float CalculateMotorForce(float motorBase, float coalQualityMultiplier, float coalRate)
    {
        // Example formula: MotorForce = motorBase * coalQualityMultiplier * coalRate
        // coalRate = how many units of coal per second are consumed
        // Adjust as needed
        return motorBase * coalQualityMultiplier * coalRate;
    }

    public float CalculateFrictionForce(float totalWeight, float frictionCoefficient)
    {
        // S = G * k (from GDD)
        // frictionForce = totalWeight * frictionCoefficient
        return totalWeight * frictionCoefficient;
    }

    public float CalculateGravityForce(float totalWeight, float angle)
    {
        // If angle > 0 (uphill), gravity acts as a resistive force: G*sin(α)
        // If angle < 0 (downhill), gravity acts as a forward force: G*cos(α) (depending on sign)
        // For simplicity, let’s assume:
        // Uphill: gravityForce = totalWeight * sin(angleInRadians)
        // Downhill: gravityForce = totalWeight * sin(angleInRadians) but negative angle gives negative force (which would be forward)
        
        // Convert angle to radians
        float angleRad = angle * Mathf.Deg2Rad;
        
        // We'll assume sin(angle) for uphill and sin(angle) for downhill,
        // Positive angle = uphill, negative angle = downhill
        // totalForce = totalWeight * sin(angleRad)
        // Positive result means resistive (uphill), negative means forward (downhill)
        
        float gravityComponent = totalWeight * Mathf.Sin(angleRad);
        // Gravity component will be positive for uphill (angle > 0) and negative for downhill (angle < 0)
        return gravityComponent;
    }

    public float GetTotalForce(float motor, float friction, float gravity)
    {
        // TotalForce = Motor - Friction - Gravity (if uphill)
        // If angle is negative (downhill), gravity might be negative, effectively adding to motor force.
        // Just sum them up: motor - friction - gravity
        return motor - friction - gravity;
    }
}
