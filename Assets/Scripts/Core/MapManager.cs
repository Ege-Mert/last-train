using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    private GameManager gameManager;
    private MapSettings mapSettings;
    private List<MapNode> visibleNodes = new List<MapNode>();
    private MapNode currentNode;

    public void Initialize(GameManager gm, MapSettings settings)
    {
        gameManager = gm;
        mapSettings = settings;

        GenerateInitialPath();
    }

    private void GenerateInitialPath()
    {
        // Create a starting node at distance 0
        currentNode = CreateStartingNode();
        visibleNodes.Add(currentNode);

        // Generate initial nodes ahead
        var newNodes = GenerateNextNodes(mapSettings.nodesToGenerateEachStep);
        LinkNodes(currentNode, newNodes);
        visibleNodes.AddRange(newNodes);

        ApplyFogOfWar();
    }

    public List<MapNode> GetVisibleNodes()
    {
        return visibleNodes;
    }

    public void RevealNextNodes()
    {
        var newNodes = GenerateNextNodes(mapSettings.nodesToGenerateEachStep);
        // Link these new nodes to the last visible node
        if (visibleNodes.Count > 0)
        {
            MapNode lastVisible = visibleNodes[visibleNodes.Count - 1];
            LinkNodes(lastVisible, newNodes);
            visibleNodes.AddRange(newNodes);
        }

        ApplyFogOfWar();
    }
    public bool CanChoosePath(float trainDistance)
    {
        MapNode current = GetCurrentNode(trainDistance);
        if (current == null) return false;
    
        // Player can choose path only if they've traveled beyond completionThreshold
        if (trainDistance >= current.distanceFromStart + current.completionThreshold)
            return true;
        
        return false;
    }

    public void ChoosePath(MapNode chosenNode)
    {
        // Move the current position to chosenNode
        MoveToChosenNode(chosenNode);
        // Reveal more nodes if desired
        RevealNextNodes();
    }

    private void MoveToChosenNode(MapNode chosenNode)
    {
        int chosenIndex = visibleNodes.IndexOf(chosenNode);
        if (chosenIndex > 0)
        {
            visibleNodes.RemoveRange(0, chosenIndex);
        }
        currentNode = chosenNode;

        // Update distance traveled in GameProgressManager
        float traveled = chosenNode.distanceFromStart - gameManager.GetGameProgressManager().GetDistance();
        gameManager.GetGameProgressManager().UpdateDistance(traveled);
    }

    public List<MapNode> GenerateNextNodes(int count)
    {
        var newNodes = new List<MapNode>();
        GamePhase phase = gameManager.GetGameProgressManager().GetGamePhase();
        float baseDistance = gameManager.GetGameProgressManager().GetDistance();

        TerrainData tData = gameManager.GetTerrainManager().GetTerrainData();
        float maxAngle = tData.maxInclineAngle;

        for (int i = 0; i < count; i++)
        {
            BiomeType biome = SelectBiomeForPhase(phase);
            float angle = Random.Range(-maxAngle, maxAngle);

            MapNode node = new MapNode();
            node.biomeType = biome;
            node.terrainAngle = angle;
            node.isUphill = angle > 0;
            node.isDownhill = angle < 0;
            node.isFlat = Mathf.Approximately(angle, 0f);

            // Distance: increment by baseNodeDistanceIncrement per node
            node.distanceFromStart = baseDistance + ((i + 1) * mapSettings.baseNodeDistanceIncrement);

            newNodes.Add(node);
        }

        // Future: Could add branching logic here

        return newNodes;
    }

    private void LinkNodes(MapNode fromNode, List<MapNode> toNodes)
    {
        if (toNodes.Count > 0)
        {
            fromNode.nextNodes.AddRange(toNodes);
        }
    }

    private void ApplyFogOfWar()
    {
        if (visibleNodes.Count > mapSettings.maxVisibleNodes)
        {
            int excess = visibleNodes.Count - mapSettings.maxVisibleNodes;
            // Ensure currentNode is visible
            int currentIndex = visibleNodes.IndexOf(currentNode);
            if (currentIndex > 0)
            {
                visibleNodes.RemoveRange(0, currentIndex);
            }
            // If still more than maxVisibleNodes
            if (visibleNodes.Count > mapSettings.maxVisibleNodes)
            {
                excess = visibleNodes.Count - mapSettings.maxVisibleNodes;
                visibleNodes.RemoveRange(0, excess);
            }
        }
    }

    private MapNode CreateStartingNode()
    {
        MapNode startNode = new MapNode
        {
            biomeType = BiomeType.FOREST,
            terrainAngle = 0f,
            isFlat = true,
            distanceFromStart = 0f
        };
        return startNode;
    }
    public MapNode GetCurrentNode(float trainDistance)
    {
        // visibleNodes are sorted by distanceFromStart due to our generation logic
        // Find the node the train is currently on or the last node it has passed
        MapNode current = null;
        foreach (var node in visibleNodes)
        {
            if (node.distanceFromStart <= trainDistance)
                current = node;
            else
                break;
        }
        return current;
    }


    private BiomeType SelectBiomeForPhase(GamePhase phase)
    {
        float forestProb = gameManager.GetBiomeManager().GetForestProbability(phase);
        return Random.value < forestProb ? BiomeType.FOREST : BiomeType.DESERT;
    }
}
