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
        if (disasterManager != null && !disasterManager.gameOverTriggered)
            disasterManager.UpdateDisaster(Time.deltaTime);
    }
}