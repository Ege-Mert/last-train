using UnityEngine;

public enum ResourceType
{
    WOOD,
    SCRAP,
    COAL,
    HUMAN
}

[System.Serializable]
public class ResourceData
{
    public ResourceType type;
    public float weightPerUnit;
}

[CreateAssetMenu(fileName = "ResourceSettings", menuName = "last-train/ResourceSettings")]
public class ResourceSettings : ScriptableObject
{
    [Header("Resource Definitions")]
    [Tooltip("Define weight per unit for each resource type.")]
    public ResourceData[] resources;

    public float GetResourceWeight(ResourceType type)
    {
        foreach (var r in resources)
        {
            if (r.type == type)
                return r.weightPerUnit;
        }
        return 0f; // default if not found
    }
}
