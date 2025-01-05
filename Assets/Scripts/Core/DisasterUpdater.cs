using UnityEngine;

public class DisasterUpdater : MonoBehaviour
{
    private DisasterManager disasterManager;

    public void Initialize(DisasterManager dm)
    {
        disasterManager = dm;
    }

    void Update()
    {
        if (disasterManager != null && !disasterManager.IsGameOver())
        {
            disasterManager.UpdateDisaster(Time.deltaTime);
        }
    }
}