using System.Collections.Generic;
using UnityEngine;

public class WagonManager
{
    private GameManager gameManager;
    private Dictionary<WagonType, WagonBuildData> availableWagons = new Dictionary<WagonType, WagonBuildData>();
    private List<Wagon> activeWagons = new List<Wagon>();
    

    // Reference to wagon prefabs for each type
    private Dictionary<WagonType, GameObject> wagonPrefabs = new Dictionary<WagonType, GameObject>();

    public void Initialize(GameManager gm, WagonBuildData[] wagonBuildDatas, Dictionary<WagonType, GameObject> prefabs)
    {
        gameManager = gm;
        wagonPrefabs = prefabs;

        foreach (var data in wagonBuildDatas)
        {
            availableWagons[data.wagonType] = data;
        }

        Debug.Log("WagonManager initialized with " + availableWagons.Count + " wagon types available.");
    }

    public bool CanBuildWagon(WagonType type)
    {
        if (!availableWagons.ContainsKey(type))
            return false;

        var data = availableWagons[type];
        // Check if resources are sufficient
        foreach (var cost in data.constructionCosts)
        {
            if (gameManager.GetResourceManager().GetResourceAmount(cost.type) < cost.amount)
                return false;
        }
        return true;
    }

    public bool TryBuildWagon(WagonType type, Transform parent)
    {
        if (!CanBuildWagon(type))
            return false;

        var data = availableWagons[type];
        // Deduct resources
        foreach (var cost in data.constructionCosts)
        {
            gameManager.GetResourceManager().RemoveResource(cost.type, cost.amount);
        }

        // Instantiate wagon
        GameObject prefab = wagonPrefabs[type];
        if (prefab == null)
        {
            Debug.LogError("No prefab found for " + type);
            return false;
        }

        GameObject wagonGO = GameObject.Instantiate(prefab, parent);
        Wagon w = wagonGO.GetComponent<Wagon>();
        w.Initialize(gameManager);
        activeWagons.Add(w);

        Debug.Log("Built wagon of type: " + type);
        return true;
    }

    public void DestroyWagon(Wagon wagon)
    {
        if (wagon == null) return;

        Debug.Log($"Destroying wagon: {wagon.name}");
        
        // The WorkerComponent's OnDestroy will handle worker reassignment
        activeWagons.Remove(wagon);
        Object.Destroy(wagon.gameObject);
    }

    public List<Wagon> GetActiveWagons()
    {
        return activeWagons;
    }
}
