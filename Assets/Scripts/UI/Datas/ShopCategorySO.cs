using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Category", fileName = "NewShopCategory")]
public class ShopCategorySO : ScriptableObject
{
    public string categoryName;    // e.g. "Resources"
    public Sprite categoryIcon;    // optional icon to display
    // You could add color, background image, etc. here
}
