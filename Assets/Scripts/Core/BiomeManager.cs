public class BiomeManager
{
    private BiomeData biomeData;

    public void Initialize(BiomeData data)
    {
        biomeData = data;
    }

    // Returns probability that the next tile should be forest in the given phase.
    public float GetForestProbability(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.EARLY: return biomeData.forestEarlyProbability;
            case GamePhase.MID: return biomeData.forestMidProbability;
            case GamePhase.LATE: return biomeData.forestLateProbability;
            default: return 0.5f;
        }
    }
}
