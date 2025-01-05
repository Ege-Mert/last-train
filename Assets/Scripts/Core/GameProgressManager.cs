using UnityEngine;

public class GameProgressManager
{
    private float distanceTraveled = 0f;
    private GameBalanceData balanceData;

    public void Initialize(GameBalanceData data)
    {
        balanceData = data;
        distanceTraveled = 0f;
    }

    public void UpdateDistance(float deltaDistance)
    {
        // Increase the total traveled distance
        distanceTraveled += deltaDistance;
    }

    public float GetDistance()
    {
        return distanceTraveled;
    }

    public GamePhase GetGamePhase()
    {
        if (distanceTraveled < balanceData.earlyGameThreshold)
            return GamePhase.EARLY;
        else if (distanceTraveled < balanceData.midGameThreshold)
            return GamePhase.MID;
        else
            return GamePhase.LATE;
    }

    public bool HasReachedFinalDestination()
    {
        return distanceTraveled >= balanceData.finalDestinationDistance;
    }
}

