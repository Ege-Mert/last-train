using UnityEngine;

public class GameEndingsManager
{
    private bool gameEnded = false;
    private GameManager gameManager;

    public void Initialize(GameManager gm)
    {
        gameManager = gm;
    }

    public void HandleGameOver(bool won)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (won)
        {
            ShowWinEnding();
        }
        else
        {
            ShowLoseEnding();
        }

        // Pause game after showing the message or loading scene
        Time.timeScale = 0f;
    }

    private void ShowWinEnding()
    {
        // Check train speed
        float speed = gameManager.GetTrainBase().GetCurrentSpeed();
        string endingMessage;

        if (speed >= 10f)
        {
            endingMessage = "You Win! Great Ending: Your speed was incredible!";
        }
        else if (speed >= 5f)
        {
            endingMessage = "You Win! Decent Ending: You made it at a decent speed.";
        }
        else
        {
            endingMessage = "You Win! Barely Made It Ending: You barely crawled to the finish.";
        }

        Debug.Log(endingMessage);
        // Optional: Load a specific ending scene or show a UI panel with different visuals.
    }

    private void ShowLoseEnding()
    {
        // Just one losing outcome
        Debug.Log("You Lose! The disaster caught you!");
        // Optional: Load a losing scene or UI.
    }
}
