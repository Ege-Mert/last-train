using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventData", menuName = "last-train/EventData")]
public class EventData : ScriptableObject
{
    public string eventName;
    [TextArea] public string description;
    public List<EventEffect> effects;
}