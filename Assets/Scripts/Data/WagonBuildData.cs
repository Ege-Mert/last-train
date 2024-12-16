using UnityEngine;

[System.Serializable]
public class ResourceAmount
{
    public ResourceType type;
    public float amount;
}

[CreateAssetMenu(fileName = "WagonBuildData", menuName = "last-train/WagonBuildData")]
public class WagonBuildData : ScriptableObject
{
    public WagonType wagonType;
    [Tooltip("Resources required to build this wagon.")]
    public ResourceAmount[] constructionCosts;
    [Tooltip("A brief description of the wagon's function.")]
    public string description;
    [Tooltip("Icon representing this wagon type.")]
    public Sprite icon; // optional
}
