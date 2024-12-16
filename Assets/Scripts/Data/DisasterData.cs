using UnityEngine;

[CreateAssetMenu(fileName = "DisasterData", menuName = "last-train/DisasterData")]
public class DisasterData : ScriptableObject
{
    public float initialSpeed = 0.5f;
    public float accelerationRate = 0.01f;
}