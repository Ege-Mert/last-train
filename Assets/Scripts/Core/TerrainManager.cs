public class TerrainManager
{
    private TerrainData terrainData;

    public void Initialize(TerrainData data)
    {
        terrainData = data;
    }

    public TerrainData GetTerrainData()
    {
        return terrainData;
    }
}
