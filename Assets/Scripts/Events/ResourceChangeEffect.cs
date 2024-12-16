using UnityEngine;

[System.Serializable]
public class ResourceChangeEffect : EventEffect
{
    public ResourceType resourceType;
    public float amountChange; // positive = add, negative = remove

    public override void Apply(GameManager gameManager)
    {
        var rm = gameManager.GetResourceManager();
        if (amountChange > 0)
            rm.AddResource(resourceType, amountChange);
        else
            rm.RemoveResource(resourceType, Mathf.Abs(amountChange));
        Debug.Log($"Event: Changed {resourceType} by {amountChange}");
    }
}
