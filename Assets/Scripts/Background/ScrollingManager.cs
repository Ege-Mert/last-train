using UnityEngine;

public class ScrollingManager : MonoBehaviour
{
    [Tooltip("Reference to the train base, so we can get the current speed.")]
    public TrainBase trainBase;

    [Tooltip("List of scrolling layers for parallax backgrounds.")]
    public ScrollingLayer[] layers;

    private void Update()
    {
        if (trainBase == null) return;

        float speed = trainBase.GetCurrentSpeed();
        float deltaTime = Time.deltaTime;

        // For each layer, call Scroll() with the train speed
        foreach (var layer in layers)
        {
            if (layer != null)
            {
                layer.Scroll(speed, deltaTime);
            }
        }
    }
}
