using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Data References")]
    public GameBalanceData gameBalanceData;
    public TerrainData terrainData;
    public BiomeData biomeData;
    public MapSettings mapSettings;
    public ResourceSettings resourceSettings;
    public WagonBuildData[] wagonBuildDatas;
    public List<GameObject> wagonPrefabs; // Each prefab corresponds to a type in wagonBuildDatas order
    public DisasterData disasterData;


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


    void Awake()
    {
        Initialize();
    }
    
    void Update()
    {
        Debug.Log(resourceManager.GetResourceAmount(ResourceType.WOOD));   
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

        // Resource
        resourceManager = new ResourceManager();
        resourceManager.Initialize(resourceSettings);

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

        wagonManager.Initialize(this, wagonBuildDatas, prefabDict);
        
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
        
        */
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
        
        GetCentralHumanManager().AddHumans(10);
        
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



    }

    public GameProgressManager GetGameProgressManager() { return gameProgressManager; }
    public TerrainManager GetTerrainManager() { return terrainManager; }
    public BiomeManager GetBiomeManager() { return biomeManager; }
    public MapManager GetMapManager() { return mapManager; }
    public ResourceManager GetResourceManager() { return resourceManager; }
    public CentralHumanManager GetCentralHumanManager() { return centralHumanManager; }
    public WagonManager GetWagonManager() { return wagonManager; }
    public BuildingManager GetBuildingManager() { return buildingManager; }
}
