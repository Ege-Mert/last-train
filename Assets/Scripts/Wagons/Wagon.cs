using UnityEngine;

public abstract class Wagon : MonoBehaviour
{
    [SerializeField] protected WagonType wagonType;
    [SerializeField] protected string wagonName;
    [SerializeField] protected float baseWeight = 10f; // Example base weight

    protected GameManager gameManager;

    public virtual void Initialize(GameManager gm)
    {
        gameManager = gm;
    }

    public WagonType GetWagonType()
    {
        return wagonType;
    }

    public float GetWeight()
    {
        // Just returns base weight for now. Later, we might add resource contents or upgrades.
        return baseWeight;
    }
    
    public string GetWagonName(){
        return wagonName;
    }
}
