using UnityEngine;

[CreateAssetMenu(fileName = "WagonUIStyle", menuName = "WagonUI/StyleData")]
public class WagonUIStyle : ScriptableObject
{
    public WagonType wagonType;
    public Sprite panelBackground; // or just a color or something
    // You can add more fields if you want
}
