using System;

public static class CameraBoundsEvents
{
    // This event notifies listeners that the camera bounds have changed.
    // float left, float right: new bounding values for the camera X.
    public static event Action<float, float> OnCameraBoundsChanged;

    public static void RaiseCameraBoundsChanged(float left, float right)
    {
        OnCameraBoundsChanged?.Invoke(left, right);
    }
}
