using UnityEngine;
using System.Collections;

public class TimeIntervalScheduler : MonoBehaviour, IEventScheduler
{
    public event System.Action OnIntervalElapsed;

    [SerializeField] private float minInterval = 30f;
    [SerializeField] private float maxInterval = 60f;

    private bool running = false;

    public void StartScheduling()
    {
        running = true;
        StartCoroutine(ScheduleLoop());
    }

    public void StopScheduling()
    {
        running = false;
    }

    private IEnumerator ScheduleLoop()
    {
        while (running)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
            if (running) OnIntervalElapsed?.Invoke();
        }
    }
}
