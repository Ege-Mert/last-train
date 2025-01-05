using System;
using UnityEngine;

public class DisasterManager
{
    public event Action OnLoseCondition;
    public event Action<float> OnDisasterDistanceChanged;

    private GameManager gameManager;
    private DisasterData disasterData;
    private GameBalanceData gameBalanceData;
    
    private float disasterSpeed;
    private float disasterPosition;
    private bool isActive = true;
    private bool isGameOver = false;

    [Header("Game Phase Modifiers")]
    private float earlyGameSpeedMultiplier = 1.0f;
    private float midGameSpeedMultiplier = 1.2f;
    private float lateGameSpeedMultiplier = 1.5f;

    public void Initialize(GameManager gm, DisasterData disasterConfig, GameBalanceData balanceData)
    {
        gameManager = gm;
        disasterData = disasterConfig;
        gameBalanceData = balanceData;

        // Initialize starting values
        disasterSpeed = disasterData.initialSpeed;
        disasterPosition = CalculateInitialPosition();
    }

    private float CalculateInitialPosition()
    {
        // Start at a percentage of the early game threshold
        return -gameBalanceData.earlyGameThreshold * 0.2f;
    }

    private float GetCurrentSpeedMultiplier(float playerDistance)
    {
        if (playerDistance <= gameBalanceData.earlyGameThreshold)
            return earlyGameSpeedMultiplier;
        else if (playerDistance <= gameBalanceData.midGameThreshold)
            return midGameSpeedMultiplier;
        else
            return lateGameSpeedMultiplier;
    }

    public void UpdateDisaster(float deltaTime)
    {
        if (!isActive || isGameOver) return;

        float playerDistance = gameManager.GetGameProgressManager().GetDistance();
        float speedMultiplier = GetCurrentSpeedMultiplier(playerDistance);

        // Update disaster speed with phase-based multiplier
        disasterSpeed += disasterData.accelerationRate * speedMultiplier * deltaTime;

        // Store previous position for change detection
        float previousPosition = disasterPosition;
        
        // Update position
        disasterPosition += disasterSpeed * deltaTime;

        // Notify listeners if position changed significantly
        if (Mathf.Abs(disasterPosition - previousPosition) > 0.1f)
        {
            OnDisasterDistanceChanged?.Invoke(disasterPosition);
        }

        // Check for game over
        if (disasterPosition >= playerDistance && !isGameOver)
        {
            isGameOver = true;
            OnLoseCondition?.Invoke();
        }
    }

    public float GetDisasterPosition() => disasterPosition;
    public float GetDisasterSpeed() => disasterSpeed;
    public float GetMaxDistance() => gameBalanceData.finalDestinationDistance;
    
    public float GetDistanceToTrain()
    {
        if (gameManager == null) return 0f;
        return gameManager.GetGameProgressManager().GetDistance() - disasterPosition;
    }

    public float GetCompletionPercentage()
    {
        return Mathf.Clamp01(disasterPosition / gameBalanceData.finalDestinationDistance);
    }

    public void StopDisaster() => isActive = false;
    public bool IsGameOver() => isGameOver;
    
    public GamePhase GetCurrentGamePhase()
    {
        float playerDistance = gameManager.GetGameProgressManager().GetDistance();
        
        if (playerDistance <= gameBalanceData.earlyGameThreshold)
            return GamePhase.EARLY;
        else if (playerDistance <= gameBalanceData.midGameThreshold)
            return GamePhase.MID;
        else
            return GamePhase.LATE;
    }
}

