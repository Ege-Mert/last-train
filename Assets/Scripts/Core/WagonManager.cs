using UnityEngine;
using System.Collections.Generic;

public class WagonManager
{
    private GameManager gameManager;
    private Dictionary<WagonType, WagonBuildData> availableWagons = new Dictionary<WagonType, WagonBuildData>();
    private List<Wagon> activeWagons = new List<Wagon>();
    private Dictionary<WagonType, GameObject> wagonPrefabs = new Dictionary<WagonType, GameObject>();
    private GlobalStorageSystem globalStorageSystem;

    private Transform wagonParent;
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
        wagonParent = parent;

        foreach (var data in wagonBuildDatas)
        {
            availableWagons[data.wagonType] = data;
        }
    }

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

    public bool TryBuildWagon(WagonType type)
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
        Vector3 spawnPos = wagonParent.position;
        if (activeWagons.Count > 0)
        {
            var lastWagon = activeWagons[activeWagons.Count - 1];
            var lastVis = lastWagon.GetComponentInChildren<WagonVisuals>();
            if (lastVis != null)
            {
                var backPos = lastVis.GetBackConnectorPosition();
                GameObject dummy = GameObject.Instantiate(wagonPrefabs[type]);
                var dummyVis = dummy.GetComponentInChildren<WagonVisuals>();
                if (dummyVis != null)
                {
                    Vector3 frontOffset = dummyVis.GetFrontConnectorPosition() - dummyVis.transform.position;
                    spawnPos = backPos - frontOffset;
                }
                GameObject.Destroy(dummy);
            }
        }

        // Instantiate
        GameObject wagonGO = GameObject.Instantiate(wagonPrefabs[type], spawnPos, Quaternion.identity, wagonParent);
        Wagon w = wagonGO.GetComponent<Wagon>();
        w.Initialize(gameManager);
        activeWagons.Add(w);

        // If it has a StorageComponent => add to global storage system
        var storageComp = wagonGO.GetComponent<StorageComponent>();
        if (storageComp != null)
        {
            globalStorageSystem.AddStorageComponent(storageComp);
        }

        // If it has a HumanCapacityComponent => add capacity
        var humanCap = wagonGO.GetComponent<HumanCapacityComponent>();
        if (humanCap != null)
        {
            int cap = humanCap.GetMaxCapacity();
            gameManager.GetCentralHumanManager().AddSleepingCapacity(cap);
        }

        OnWagonAdded?.Invoke(w);

        // Recalc camera bounds
        RecalculateCameraBounds();

        return true;
    }

    public void DestroyWagon(Wagon wagon)
    {
        if (wagon == null) return;
        OnWagonRemoved?.Invoke(wagon);
        activeWagons.Remove(wagon);

        var humanCap = wagon.GetComponent<HumanCapacityComponent>();
        if (humanCap != null)
        {
            int cap = humanCap.GetMaxCapacity();
            gameManager.GetCentralHumanManager().RemoveSleepingCapacity(cap);
        }

        var storageComp = wagon.GetComponent<StorageComponent>();
        if (storageComp != null)
        {
            globalStorageSystem.RemoveStorageComponent(storageComp);
        }

        var workerComp = wagon.GetComponent<WorkerComponent>();
        if (workerComp != null)
        {
            gameManager.GetCentralHumanManager().HandleWagonDestruction(workerComp);
        }

        Object.Destroy(wagon.gameObject);

        ReAlignAllWagons();
        RecalculateCameraBounds();
    }

    public void ReAlignAllWagons()
    {
        if (activeWagons.Count == 0) return;

        SnapWagonToLocomotive(activeWagons[0]);
        for (int i = 1; i < activeWagons.Count; i++)
        {
            SnapWagonToPrevious(activeWagons[i], activeWagons[i - 1]);
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

    // **Here** we compute new left/right camera bounds and raise an event instead of calling camera directly
    private void RecalculateCameraBounds()
    {
        if (activeWagons.Count == 0)
        {
            // default bounds
            CameraBoundsEvents.RaiseCameraBoundsChanged(-10f, 10f);
            return;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        foreach (var w in activeWagons)
        {
            float x = w.transform.position.x;
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
        }

        float margin = 5f;
        minX -= margin;
        maxX += margin;

        // Instead of calling cameraController, we raise an event
        CameraBoundsEvents.RaiseCameraBoundsChanged(minX, maxX);
    }

    public List<Wagon> GetActiveWagons() => activeWagons;
}
