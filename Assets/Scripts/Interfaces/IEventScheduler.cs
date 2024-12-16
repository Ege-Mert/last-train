using System;
using UnityEngine;

public interface IEventScheduler
{
    event Action OnIntervalElapsed;
    void StartScheduling();
    void StopScheduling();
}


