using System.Collections.Generic;
using UnityEngine;

public class WagonManager
{
    private GameManager gameManager;
    private Dictionary<WagonType, WagonBuildData> availableWagons = new Dictionary<WagonType, WagonBuildData>();
    private List<Wagon> activeWagons = new List<Wagon>();
    private Dictionary<WagonType, GameObject> wagonPrefabs = new Dictionary<WagonType, GameObject>();
    
    private GlobalStorageSystem globalStorageSystem;


    public void Initialize(GameManager gm, WagonBuildData[] wagonBuildDatas, Dictionary<WagonType, GameObject> prefabs, GlobalStorageSystem storageSystem)
    {
        gameManager = gm;
        wagonPrefabs = prefabs;
        globalStorageSystem = storageSystem;
        

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
        GameObject wagonGO = GameObject.Instantiate(prefab, parent);
        Wagon w = wagonGO.GetComponent<Wagon>();
        w.Initialize(gameManager);
        activeWagons.Add(w);

        // If it has a StorageComponent, add it to the global storage system
        var storageComp = wagonGO.GetComponent<StorageComponent>();
        if (storageComp != null)
        {
            globalStorageSystem.AddStorageComponent(storageComp);
        }

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
