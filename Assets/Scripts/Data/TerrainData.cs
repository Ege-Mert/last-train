using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "last-train/TerrainData")]
public class TerrainData : ScriptableObject
{
    [Header("Terrain Settings")]
    [Tooltip("Maximum absolute angle in degrees that a tile can have (e.g., 15 means from -15 to +15 degrees).")]
    public float maxInclineAngle = 15f;
    
    // In a more complex scenario, you could add friction modifiers or other terrain parameters.
}
