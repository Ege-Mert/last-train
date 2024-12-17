using UnityEngine;

public class ConverterWagon : Wagon
{
    public WagonUpgradeData upgradeData;
    private WorkerComponent workerComponent;
    private ConverterComponent converterComponent;

    public override void Initialize(GameManager gm)
    {
        base.Initialize(gm);

        workerComponent = GetComponent<WorkerComponent>();
        converterComponent = GetComponent<ConverterComponent>();

        if (workerComponent != null && converterComponent != null)
        {
            workerComponent.Initialize(gm.GetCentralHumanManager());
            converterComponent.Initialize(gm, workerComponent);
        }

        var upgradeComp = GetComponent<UpgradeComponent>();
        if (upgradeComp != null && upgradeData != null)
        {
            upgradeComp.Initialize(gm, this, upgradeData);
        }
    }

    void Update()
    {
        if (converterComponent != null)
        {
            converterComponent.ConvertResources(Time.deltaTime);
        }
    }
}
