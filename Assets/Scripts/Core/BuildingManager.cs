using System.Collections.Generic;
using UnityEngine;

public class BuildingManager
{
    private GameManager gameManager;
    private Dictionary<WagonType, WagonBuildData> availableWagons = new Dictionary<WagonType, WagonBuildData>();

    public void Initialize(GameManager gm, WagonBuildData[] wagonDatas)
    {
        gameManager = gm;
        availableWagons.Clear();
        foreach (var data in wagonDatas)
        {
            availableWagons[data.wagonType] = data;
        }
        Debug.Log("BuildingManager initialized with " + availableWagons.Count + " available wagon types.");
    }

    public bool CanBuildWagon(WagonType type)
    {
        if (!availableWagons.ContainsKey(type))
            return false;

        var data = availableWagons[type];
        var rm = gameManager.GetResourceManager();

        foreach (var cost in data.constructionCosts)
        {
            if (rm.GetResourceAmount(cost.type) < cost.amount)
                return false;
        }
        return true;
    }

    public bool TryBuildWagon(WagonType type, Transform parent)
    {
        if (!CanBuildWagon(type))
        {
            Debug.Log("Not enough resources to build wagon of type: " + type);
            return false;
        }

        // If we can build it, just call WagonManager
        bool result = gameManager.GetWagonManager().TryBuildWagon(type);
        return result;
    }

    public List<WagonBuildData> GetAvailableWagons()
    {
        return new List<WagonBuildData>(availableWagons.Values);
    }

    public WagonBuildData GetBuildData(WagonType type)
    {
        if (availableWagons.ContainsKey(type))
            return availableWagons[type];
        return null;
    }
}
