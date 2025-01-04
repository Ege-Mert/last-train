using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Item", fileName = "NewShopItem")]
public class ShopItemSO : ScriptableObject
{
    public string itemName;        // e.g. "Wood Collector"
    public Sprite itemIcon;
    public ShopCategorySO category;  // reference to one of the categories
    public WagonType wagonType;    // so we know which wagon to build
    public ResourceAmount[] cost;  // or an int costWood if simpler
    public string description;     // for UI tooltip or detail
}
