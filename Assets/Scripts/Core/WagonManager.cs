using System.Collections.Generic;
using UnityEngine;

public class WagonManager
{
    private GameManager gameManager;
    private Transform wagonParent; // New field

    private Dictionary<WagonType, WagonBuildData> availableWagons = new Dictionary<WagonType, WagonBuildData>();
    private List<Wagon> activeWagons = new List<Wagon>();
    private Dictionary<WagonType, GameObject> wagonPrefabs = new Dictionary<WagonType, GameObject>();
    private GlobalStorageSystem globalStorageSystem;

    // 1) Add this field to store the locomotive's rear connection
    private Transform locomotiveRearConn;

    public delegate void WagonEvent(Wagon wagon);
    public event WagonEvent OnWagonAdded;
    public event WagonEvent OnWagonRemoved;

    public void Initialize(GameManager gm, 
                           WagonBuildData[] wagonBuildDatas, 
                           Dictionary<WagonType, GameObject> prefabs, 
                           GlobalStorageSystem storageSystem,
                           Transform parent)
    {
        gameManager = gm;
        wagonPrefabs = prefabs;
        globalStorageSystem = storageSystem;
        wagonParent = parent; // Store the wagon parent

        foreach (var data in wagonBuildDatas)
        {
            availableWagons[data.wagonType] = data;
        }

        Debug.Log("WagonManager initialized with " + availableWagons.Count + " wagon types available.");
    }

    // 2) Provide a public setter so GameManager can assign the locomotive’s rear connector
    public void SetLocomotiveConnectionPoint(Transform locoRear)
    {
        locomotiveRearConn = locoRear;
    }

    public bool CanBuildWagon(WagonType type)
    {
        if (!availableWagons.ContainsKey(type)) return false;

        var data = availableWagons[type];
        foreach (var cost in data.constructionCosts)
        {
            if (gameManager.GetResourceManager().GetResourceAmount(cost.type) < cost.amount)
                return false;
        }
        return true;
    }

    public bool TryBuildWagon(WagonType type)  // Removed Transform parent parameter
    {
        if (!CanBuildWagon(type)) return false;

        // Deduct resources
        var data = availableWagons[type];
        foreach (var cost in data.constructionCosts)
        {
            gameManager.GetResourceManager().RemoveResource(cost.type, cost.amount);
        }

        if (!wagonPrefabs.ContainsKey(type) || wagonPrefabs[type] == null)
        {
            Debug.LogError("No prefab for wagon type: " + type);
            return false;
        }

        // Decide spawn position
        Vector3 spawnPos = wagonParent.position;  // Using internal wagonParent

        if (activeWagons.Count > 0)
        {
            var lastWagon = activeWagons[activeWagons.Count - 1];
            var lastVis = lastWagon.GetComponentInChildren<WagonVisuals>();
            if (lastVis != null)
            {
                var backPos = lastVis.GetBackConnectorPosition();
                GameObject temp = wagonPrefabs[type];
                GameObject dummy = GameObject.Instantiate(temp);
                var dummyVis = dummy.GetComponentInChildren<WagonVisuals>();
                if (dummyVis != null)
                {
                    Vector3 frontOffset = dummyVis.GetFrontConnectorPosition() - dummyVis.transform.position;
                    spawnPos = backPos - frontOffset;
                }
                GameObject.Destroy(dummy);
            }
        }

        GameObject wagonGO = GameObject.Instantiate(wagonPrefabs[type], spawnPos, Quaternion.identity, wagonParent);
        Wagon w = wagonGO.GetComponent<Wagon>();
        w.Initialize(gameManager);
        activeWagons.Add(w);

        var storageComp = wagonGO.GetComponent<StorageComponent>();
        if (storageComp != null)
        {
            globalStorageSystem.AddStorageComponent(storageComp);
        }
            
        var humanCap = wagonGO.GetComponent<HumanCapacityComponent>();
        if (humanCap != null)
        {
            int cap = humanCap.GetMaxCapacity();
            gameManager.GetCentralHumanManager().AddSleepingCapacity(cap);
        }

        OnWagonAdded?.Invoke(w);

        return true;
    }

    public Wagon GetLastWagon()
    {
        if (activeWagons.Count == 0) return null;
        return activeWagons[activeWagons.Count - 1];
    }

    public void DestroyWagon(Wagon wagon)
    {
        if (wagon == null) return;
        OnWagonRemoved?.Invoke(wagon);
        activeWagons.Remove(wagon);
        Debug.Log($"Destroying wagon: {wagon.name}");

        // If it has a HumanCapacityComponent => remove capacity
        var humanCap = wagon.GetComponent<HumanCapacityComponent>();
        if (humanCap != null)
        {
            int cap = humanCap.GetMaxCapacity();
            gameManager.GetCentralHumanManager().RemoveSleepingCapacity(cap);
            ReAlignAllWagons();
        }

        // If it has a storage comp => remove from global storage
        var storageComp = wagon.GetComponent<StorageComponent>();
        if (storageComp != null)
        {
            globalStorageSystem.RemoveStorageComponent(storageComp);
            ReAlignAllWagons();
        }

        // Also reassign workers if it had WorkerComponent
        // (CentralHumanManager.HandleWagonDestruction)
        var workerComp = wagon.GetComponent<WorkerComponent>();
        if (workerComp != null)
        {
            gameManager.GetCentralHumanManager().HandleWagonDestruction(workerComp);
            ReAlignAllWagons();
        }

        Object.Destroy(wagon.gameObject);

        // Re-align after removal, if desired
        ReAlignAllWagons();
    }

    public void ReAlignAllWagons()
    {
        var wagons = activeWagons;
        if (wagons.Count == 0) return;

        // snap first wagon to locomotive
        SnapWagonToLocomotive(wagons[0]);

        // snap subsequent wagons
        for (int i = 1; i < wagons.Count; i++)
        {
            SnapWagonToPrevious(wagons[i], wagons[i - 1]);
        }
    }

    private void SnapWagonToLocomotive(Wagon w)
    {
        if (locomotiveRearConn == null) return;

        var wagonVis = w.GetComponentInChildren<WagonVisuals>();
        if (wagonVis == null) return;

        Vector3 locoPos = locomotiveRearConn.position;
        Vector3 wagonFrontPos = wagonVis.GetFrontConnectorPosition();
        Vector3 offset = locoPos - wagonFrontPos;

        w.transform.position += offset;
    }

    private void SnapWagonToPrevious(Wagon thisWagon, Wagon prevWagon)
    {
        var prevVis = prevWagon.GetComponentInChildren<WagonVisuals>();
        var thisVis = thisWagon.GetComponentInChildren<WagonVisuals>();
        if (prevVis == null || thisVis == null) return;

        Vector3 offset = prevVis.GetBackConnectorPosition() - thisVis.GetFrontConnectorPosition();
        thisWagon.transform.position += offset;
    }

    public List<Wagon> GetActiveWagons()
    {
        return activeWagons;
    }
}
