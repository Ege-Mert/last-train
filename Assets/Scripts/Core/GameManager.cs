using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Data References")]
    public GlobalStorageData globalStorageData;
    public GameBalanceData gameBalanceData;
    public TerrainData terrainData;
    public BiomeData biomeData;
    public MapSettings mapSettings;
    public ResourceSettings resourceSettings;
    public DisasterData disasterData;
    public WagonBuildData[] wagonBuildDatas;
    
    
    public List<GameObject> wagonPrefabs; // Each prefab corresponds to a type in wagonBuildDatas order
    public List<EventData> eventPool; // Assign in inspector
    [SerializeField] private TimeIntervalScheduler eventSchedulerPrefab; // a prefab or assign a scene object
    





    // Managers
    private GameProgressManager gameProgressManager;
    private TerrainManager terrainManager;
    private BiomeManager biomeManager;
    private MapManager mapManager;
    private ResourceManager resourceManager;
    private CentralHumanManager centralHumanManager;
    private WagonManager wagonManager;
    private BuildingManager buildingManager;
    private TrainBase trainBase;
    private TrainPhysics trainPhysics;
    private DisasterManager disasterManager;
    private EventManager eventManager;
    private TimeIntervalScheduler eventScheduler;
    private GameEndingsManager endingsManager;
    private GlobalStorageSystem globalStorageSystem;
    
    [Header("Viusal Manager Reference")]
    [SerializeField] private TrainVisualManager trainVisualManager;
    [SerializeField] private Transform wagonParent; // Assign WagonAttachPoint in inspector




    public static GameManager Instance { get; private set; }

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Initialize();
    }
    private void Update()
    {
        GetCentralHumanManager().GetAvailableHumans();
        GetCentralHumanManager().GetTotalHumans();
        GetGlobalStorageSystem().GetTotalCapacity();
    }
    

    public void Initialize()
    {
        // GameProgress
        gameProgressManager = new GameProgressManager();
        gameProgressManager.Initialize(gameBalanceData);

        // Terrain
        terrainManager = new TerrainManager();
        terrainManager.Initialize(terrainData);

        // Biome
        biomeManager = new BiomeManager();
        biomeManager.Initialize(biomeData);

        // Map
        mapManager = new MapManager();
        mapManager.Initialize(this, mapSettings);
        
        // Storage
        globalStorageSystem = new GlobalStorageSystem();
        float baseCap = 100f;
        if (globalStorageData != null)
        {
            baseCap = globalStorageData.baseCapacity; 
        }
        globalStorageSystem.Initialize(baseCap);

        // Resource
        resourceManager = new ResourceManager();
        resourceManager.Initialize(resourceSettings, globalStorageSystem);


        // Humans
        centralHumanManager = new CentralHumanManager();
        centralHumanManager.Initialize();
        
        // Wagon
        wagonManager = new WagonManager();

        // Build a dictionary for prefabs:
        Dictionary<WagonType, GameObject> prefabDict = new Dictionary<WagonType, GameObject>();
        for (int i = 0; i < wagonBuildDatas.Length; i++)
        {
            prefabDict[wagonBuildDatas[i].wagonType] = wagonPrefabs[i];
        }
        wagonManager.Initialize(this, wagonBuildDatas, prefabDict, globalStorageSystem);

        
        // Building
        buildingManager = new BuildingManager();
        buildingManager.Initialize(this, wagonBuildDatas);
        
        // TrainPhysics
        trainPhysics = new TrainPhysics();
        trainPhysics.Initialize(this);
        // Assume we have a reference to a TrainBase GameObject with TrainBase script attached
        // Assign it via the Inspector or via a Find call:
        trainBase = FindObjectOfType<TrainBase>();
        if (trainBase != null)
        {
            trainBase.Initialize(this, trainPhysics);
        }
        if (trainVisualManager != null){ 
        trainVisualManager.Initialize(this);
        }
        else{
            Debug.Log("Not working");
        }
        wagonManager.SetLocomotiveConnectionPoint(wagonParent);
        
        
        // Disaster
        disasterManager = new DisasterManager();
        disasterManager.Initialize(this, disasterData.initialSpeed, disasterData.accelerationRate);
        
        // DisasterUpdater
        var updater = new GameObject("DisasterUpdater").AddComponent<DisasterUpdater>();
        updater.Initialize(disasterManager);
        
        // Events
        // Create or find the eventScheduler
        eventScheduler = Instantiate(eventSchedulerPrefab); 
        // If it's a scene object, you can skip Instantiate and just reference it from Inspector

        eventManager = new EventManager();
        eventManager.Initialize(this, eventPool, eventScheduler);
        
        // Endings
        endingsManager = new GameEndingsManager();
        endingsManager.Initialize(this);
        
        // Events from DisasterManager and TrainBase
        GetDisasterManager().OnLoseCondition += () => endingsManager.HandleGameOver(false);
        GetTrainBase().OnWinCondition += () => endingsManager.HandleGameOver(true);
        



        resourceManager.AddResourcePartial(ResourceType.WOOD, 200f);
        // if (wagonParent != null)
        // {
        //     wagonManager.TryBuildWagon(WagonType.STORAGE, wagonParent);
        // }
        // else
        // {
        //     Debug.LogError("Wagon parent transform not assigned!");
        // }
        StartCoroutine(BuildInitialWagons());


        /*
        Debug.Log("GameManager initialized. Current Distance: " + gameProgressManager.GetDistance());
        Debug.Log("GameManager initialized. Current Distance: " + gameProgressManager.GetDistance());
        resourceManager.AddResource(ResourceType.WOOD, 10f);
        resourceManager.RemoveResource(ResourceType.WOOD, 5f);
        centralHumanManager.AddHumans(5);
		Debug.Log("Available Humans: " + centralHumanManager.GetAvailableHumans());
        resourceManager.AddResource(ResourceType.WOOD, 20f);
        bool built = wagonManager.TryBuildWagon(WagonType.WOOD_COLLECTOR, this.transform);
        Debug.Log("TryBuildWagon result: " + built);
        
        // Test: Add resources
        resourceManager.AddResource(ResourceType.WOOD, 20f);

        // Check if we can build a WOOD_COLLECTOR wagon
        bool canBuild = buildingManager.CanBuildWagon(WagonType.WOOD_COLLECTOR);
        Debug.Log("Can build WOOD_COLLECTOR: " + canBuild);

        // If can build, try building it:
        if (canBuild)
        {
            buildingManager.TryBuildWagon(WagonType.WOOD_COLLECTOR, this.transform);
        }
        resourceManager.AddResource(ResourceType.COAL, 100f);
        
        
    var wagons = GetWagonManager().GetActiveWagons();
    foreach (var w in wagons)
    {
        if (w.GetWagonType() == WagonType.WOOD_COLLECTOR)
        {
            var wc = w.GetComponent<WorkerComponent>();
            if (wc != null)
            {
                bool added = wc.AddWorkers(3); // Try adding 3 workers
                Debug.Log("Tried adding workers: " + added);
            }
        }
    }
        */
        GetCentralHumanManager().AddHumans(100);


    }
    private IEnumerator BuildInitialWagons()
    {
        yield return new WaitForSeconds(0.1f); // Initial delay
        
        wagonManager.TryBuildWagon(WagonType.STORAGE, wagonParent);
        yield return new WaitForSeconds(5f); // Initial delay
        wagonManager.TryBuildWagon(WagonType.WOOD_COLLECTOR, wagonParent);
        yield return new WaitForSeconds(1f); // Initial delay
        wagonManager.TryBuildWagon(WagonType.SCRAP_COLLECTOR, wagonParent);
        yield return new WaitForSeconds(1f); // Initial delay
        wagonManager.TryBuildWagon(WagonType.CONVERTER, wagonParent);
        yield return new WaitForSeconds(1f); // Initial delay
        wagonManager.TryBuildWagon(WagonType.SLEEPING, wagonParent);
        yield return new WaitForSeconds(1f); // Initial delay
        wagonManager.TryBuildWagon(WagonType.WOOD_COLLECTOR, wagonParent);
        yield return new WaitForSeconds(1f); // Initial delay
        wagonManager.TryBuildWagon(WagonType.CONVERTER, wagonParent);
        yield return new WaitForSeconds(2f); // Initial delay
        TestWagonRemoval();
        yield return new WaitForSeconds(2f); // Initial delay
        TestWagonRemoval();
        
        yield return new WaitForSeconds(4f); // Initial delay
        GetTrainBase().currentSpeed = 12;
    }
    public void TestWagonRemoval()
{
    var wagons = wagonManager.GetActiveWagons();
    if (wagons.Count > 0)
    {
        wagonManager.DestroyWagon(wagons[2]);
    }
}

    public GameProgressManager GetGameProgressManager() { return gameProgressManager; }
    public TerrainManager GetTerrainManager() { return terrainManager; }
    public BiomeManager GetBiomeManager() { return biomeManager; }
    public MapManager GetMapManager() { return mapManager; }
    public ResourceManager GetResourceManager() { return resourceManager; }
    public CentralHumanManager GetCentralHumanManager() { return centralHumanManager; }
    public WagonManager GetWagonManager() { return wagonManager; }
    public BuildingManager GetBuildingManager() { return buildingManager; }
    public EventManager GetEventManager() { return eventManager; }
    public TrainBase GetTrainBase() { return trainBase; }
    public TrainPhysics GetTrainPhysics() { return trainPhysics; }
    public DisasterManager GetDisasterManager() { return disasterManager; }
    public GlobalStorageSystem GetGlobalStorageSystem() {return globalStorageSystem;}
    public TrainVisualManager GetTrainVisualManager() {return trainVisualManager;}
}
