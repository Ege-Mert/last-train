using UnityEngine;
using System.Collections.Generic;

public class EventManager
{
    private GameManager gameManager;
    private List<EventData> eventPool;
    private IEventScheduler scheduler;
    private bool systemActive = false;

    public void Initialize(GameManager gm, List<EventData> events, IEventScheduler sched)
    {
        gameManager = gm;
        eventPool = events;
        scheduler = sched;
        systemActive = true;

        scheduler.OnIntervalElapsed += OnInterval;
        scheduler.StartScheduling();
    }

    private void OnInterval()
    {
        if (systemActive && eventPool.Count > 0)
        {
            TriggerRandomEvent();
        }
    }

    public void TriggerRandomEvent()
    {
        int index = Random.Range(0, eventPool.Count);
        var chosenEvent = eventPool[index];
        ApplyEvent(chosenEvent);
    }

    private void ApplyEvent(EventData eventData)
    {
        Debug.Log($"Event Triggered: {eventData.eventName}\n{eventData.description}");
        foreach (var effect in eventData.effects)
        {
            effect.Apply(gameManager);
        }
        // UI integration for events can happen here or via an OnEventTriggered event
    }

    public void StopEvents()
    {
        systemActive = false;
        scheduler.StopScheduling();
    }
}

