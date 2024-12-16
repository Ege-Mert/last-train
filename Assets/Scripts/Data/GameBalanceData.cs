using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceData", menuName = "last-train/Game Balance")]
public class GameBalanceData : ScriptableObject
{
    [Header("Game Progression")]
    [Tooltip("Distance at which the game ends (e.g., 1000 or 1200 km).")]
    public float finalDestinationDistance = 1000f;

    [Tooltip("Distance threshold for early game end. Below this = EARLY game.")]
    public float earlyGameThreshold = 300f;

    [Tooltip("Distance threshold for mid game start. Between earlyGameThreshold and this = MID game. Above this = LATE game until final destination.")]
    public float midGameThreshold = 500f;
}
