using UnityEngine;
using System.Collections.Generic;

public class EventManager
{
    private GameManager gameManager;
    private List<EventData> eventPool = new List<EventData>();
    //private float timeSinceLastEvent = 0f;
    private float minEventInterval = 30f; // for example
    private float maxEventInterval = 60f;
    private float nextEventTime;

    private bool eventInProgress = false;

    public void Initialize(GameManager gm, List<EventData> events)
    {
        gameManager = gm;
        eventPool = events;
        ScheduleNextEvent();
    }

    private void ScheduleNextEvent()
    {
        float interval = Random.Range(minEventInterval, maxEventInterval);
        nextEventTime = Time.time + interval;
    }

    public void UpdateEvents()
    {
        if (eventInProgress) return;

        if (Time.time >= nextEventTime && eventPool.Count > 0)
        {
            TriggerRandomEvent();
            ScheduleNextEvent();
        }
    }

    public void TriggerRandomEvent()
    {
        if (eventPool.Count == 0) return;

        int index = Random.Range(0, eventPool.Count);
        EventData chosenEvent = eventPool[index];
        ApplyEvent(chosenEvent);
    }

    private void ApplyEvent(EventData eventData)
    {
        Debug.Log($"Event Triggered: {eventData.eventName}\n{eventData.description}");
        foreach (var effect in eventData.effects)
        {
            effect.Apply(gameManager);
        }
        // In future, pause the game, show UI, and wait for player confirmation before ApplyEvent
    }
}
