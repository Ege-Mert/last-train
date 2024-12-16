using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "last-train/MapSettings")]
public class MapSettings : ScriptableObject
{
    [Header("Visibility Settings")]
    [Tooltip("Maximum number of nodes visible at once ahead of the player.")]
    public int maxVisibleNodes = 5;

    [Header("Generation Settings")]
    [Tooltip("How many nodes to generate each time new nodes are revealed.")]
    public int nodesToGenerateEachStep = 3;

    [Tooltip("Base distance increment between generated nodes.")]
    public float baseNodeDistanceIncrement = 10f;
}
