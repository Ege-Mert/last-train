using UnityEngine;

public class DurabilityComponent : MonoBehaviour
{
    [SerializeField] private float maxDurability = 100f;
    [SerializeField] private float currentDurability = 100f;
    [SerializeField] private float degradeRate = 0.1f; // durability lost per second

    private bool isBroken = false;
    private SpriteRenderer spriteRenderer; // if we have a sprite to change color or sprite

    void Start()
    {
        currentDurability = maxDurability;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isBroken)
        {
            DegradeDurability(Time.deltaTime);
            UpdateVisuals();
        }
    }

    private void DegradeDurability(float deltaTime)
    {
        currentDurability -= degradeRate * deltaTime;
        if (currentDurability <= 0f)
        {
            currentDurability = 0f;
            BreakWagon();
        }
    }

    private void BreakWagon()
    {
        isBroken = true;
        Debug.Log($"{name} wagon is broken!");
        // Further logic to stop wagon operations handled by the wagon script.
    }

    public bool IsBroken()
    {
        return isBroken;
    }

    public void Repair(float repairAmount)
    {
        // For now, just a direct repair. Later integrate minigame or costs.
        currentDurability += repairAmount;
        if (currentDurability > maxDurability)
            currentDurability = maxDurability;

        if (currentDurability > 0f && isBroken)
        {
            isBroken = false;
            Debug.Log($"{name} wagon is repaired and operational again!");
        }
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        // Example: change color based on durability
        float durabilityPercent = currentDurability / maxDurability;
        // Darken the sprite as durability lowers: from white (fully durable) to red-ish (broken)
        Color c = Color.Lerp(Color.red, Color.white, durabilityPercent);
        spriteRenderer.color = c;
    }

    public float GetDurability()
    {
        return currentDurability;
    }

    public float GetMaxDurability()
    {
        return maxDurability;
    }

    public float GetDurabilityPercent()
    {
        return currentDurability / maxDurability;
    }
}
