using UnityEngine;

[CreateAssetMenu(fileName = "GlobalStorageData", menuName = "last-train/GlobalStorageData")]
public class GlobalStorageData : ScriptableObject
{
    [Header("Base capacity for the entire train (without storage wagons).")]
    public float baseCapacity = 100f;
}
