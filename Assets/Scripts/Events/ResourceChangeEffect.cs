using UnityEngine;

[CreateAssetMenu(fileName = "ResourceChangeEffect", menuName = "random-events/Resource Change")]
public class ResourceChangeEffect : EventEffect
{
    public ResourceType resourceType;
    public float amountChange;

    public override void Apply(GameManager gameManager)
    {
        var rm = gameManager.GetResourceManager();
        if (amountChange > 0)
            rm.AddResourcePartial(resourceType, amountChange);
        else
            rm.RemoveResource(resourceType, Mathf.Abs(amountChange));
        
        Debug.Log($"Event Effect: Changed {resourceType} by {amountChange}");
    }
}
