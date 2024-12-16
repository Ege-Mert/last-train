using System.Collections.Generic;

public class MapNode
{
    public BiomeType biomeType;
    public float terrainAngle;
    public bool isUphill;
    public bool isDownhill;
    public bool isFlat;
    public float distanceFromStart;
    public float completionThreshold = 5f; // e.g., player must travel 5 more units after reaching this node
    public List<MapNode> nextNodes = new List<MapNode>();
}

public enum BiomeType
{
    FOREST,
    DESERT
}
