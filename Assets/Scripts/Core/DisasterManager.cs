using System;
using UnityEngine;

public class DisasterManager
{
    public event Action OnLoseCondition;

    
    private GameManager gameManager;
    private float disasterSpeed = 0f; // speed of the disaster in units/sec
    private float accelerationRate = 0.01f; // how quickly disaster speeds up over time
    private float disasterPosition = -50f; // start some distance behind the player’s start (e.g., -50)
    private bool systemActive = true;

    public bool gameOverTriggered = false; // OH boy I hopes nothings bads evers happens to my unprotexteds publics bools. Then I mets Larry The Public Bool Fondler.

    public void Initialize(GameManager gm, float initialSpeed, float acceleration)
    {
        gameManager = gm;
        disasterSpeed = initialSpeed;
        accelerationRate = acceleration;
        disasterPosition = -50f; // start behind the starting point
        // You can start a coroutine or have another system call UpdateDisaster each frame

    }

        public void UpdateDisaster(float deltaTime)
    {
        if (!systemActive || gameOverTriggered) return;

        // Increase disaster speed over time
        disasterSpeed += accelerationRate * deltaTime;

        // Move disaster forward
        disasterPosition += disasterSpeed * deltaTime;

        float playerDistance = gameManager.GetGameProgressManager().GetDistance();
        if (disasterPosition >= playerDistance)
        {
            // Disaster caught the player
            gameOverTriggered = true;
            OnLoseCondition?.Invoke();
        }
    }

    public float GetDisasterPosition()
    {
        return disasterPosition;
    }

    public float GetDisasterSpeed()
    {
        return disasterSpeed;
    }

    public void StopDisaster()
    {
        systemActive = false;
    }
}
