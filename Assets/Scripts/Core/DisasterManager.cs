using UnityEngine;

public class DisasterManager
{
    private GameManager gameManager;
    private float disasterSpeed = 0f; // speed of the disaster in units/sec
    private float accelerationRate = 0.01f; // how quickly disaster speeds up over time
    private float disasterPosition = -50f; // start some distance behind the player’s start (e.g., -50)
    private bool gameOver = false;

    public void Initialize(GameManager gm, float initialSpeed, float acceleration)
    {
        gameManager = gm;
        disasterSpeed = initialSpeed;
        accelerationRate = acceleration;
        disasterPosition = -50f; // start behind the starting point
        gameOver = false;
    }

    public void UpdateDisaster(float deltaTime)
    {
        if (gameOver) return;

        // Increase disaster speed over time
        disasterSpeed += accelerationRate * deltaTime;

        // Move disaster forward
        disasterPosition += disasterSpeed * deltaTime;

        // Check if disaster catches the player
        float playerDistance = gameManager.GetGameProgressManager().GetDistance();
        if (disasterPosition >= playerDistance)
        {
            // Disaster caught the player
            Debug.Log("Disaster caught the player! Game Over.");
            gameOver = true;
            // Trigger lose condition here (e.g., call a GameOver method in GameManager)
            gameManager.GameOver(false);
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

    public bool IsGameOver()
    {
        return gameOver;
    }
}
