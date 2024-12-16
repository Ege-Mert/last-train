using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "last-train/BiomeData")]
public class BiomeData : ScriptableObject
{
    [Header("Biome Probabilities")]
    [Tooltip("Probability of forest biome during early game (0 to 1).")]
    public float forestEarlyProbability = 0.8f;

    [Tooltip("Probability of forest biome during mid game.")]
    public float forestMidProbability = 0.5f;

    [Tooltip("Probability of forest biome during late game.")]
    public float forestLateProbability = 0.2f;
    
    // Desert probability can be implied as (1 - forestProbability).
    // If you want more biomes, add more fields here.
}
