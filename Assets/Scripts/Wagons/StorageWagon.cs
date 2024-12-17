using UnityEngine;

public class StorageWagon : Wagon
{
    public WagonUpgradeData upgradeData;
    private StorageComponent storageComponent;

    public override void Initialize(GameManager gm)
    {
        base.Initialize(gm);

        storageComponent = GetComponent<StorageComponent>();
        
        var upgradeComp = GetComponent<UpgradeComponent>();
        if (upgradeComp != null && upgradeData != null)
        {
            upgradeComp.Initialize(gm, this, upgradeData);
        }
    }
}
