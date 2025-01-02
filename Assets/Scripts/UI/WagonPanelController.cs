using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WagonPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text durabilityText;
    [SerializeField] private TMP_Text workersText;
    [SerializeField] private Button fixButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject destroyConfirmPanel;
    [SerializeField] private Button destroyConfirmButton;
    [SerializeField] private Button destroyCancelButton;

    private Wagon currentWagon;
    private GameManager gameManager;
    private bool panelVisible = false;

    // Optionally store your fixThreshold, fixCostPerDurability, etc. here
    [SerializeField] private float fixThreshold = 0.8f;
    [SerializeField] private float fixCostPerDurability = 1f;

    public void Initialize(GameManager gm)
    {
        gameManager = gm;
        HidePanel();
    }

    public void ShowPanel(Wagon wagon)
    {
        currentWagon = wagon;
        // Activate the panel
        gameObject.SetActive(true);
        panelVisible = true;

        // Hide confirm subpanel by default
        if (destroyConfirmPanel != null)
            destroyConfirmPanel.SetActive(false);

        RefreshPanel();
    }

    public void HidePanel()
    {
        currentWagon = null;
        gameObject.SetActive(false);
        panelVisible = false;
    }

    private void Update()
    {
        if (panelVisible)
        {
            RefreshPanel(); // if you want continuous refresh
        }
    }

    private void RefreshPanel()
    {
        if (currentWagon == null) return;

        // Get references to wagon's components
        var durability = currentWagon.GetComponent<DurabilityComponent>();
        var worker = currentWagon.GetComponent<WorkerComponent>();
        var upgrade = currentWagon.GetComponent<UpgradeComponent>();

        // Update durability text
        if (durability != null && durabilityText != null)
        {
            float pct = durability.GetDurabilityPercent() * 100f;
            durabilityText.text = $"Durability: {pct:0.0}%";
            // fixButton interactable if below threshold
            if (fixButton != null)
            {
                fixButton.interactable = (durability.GetDurabilityPercent() < fixThreshold);
            }
        }

        // Update worker text
        if (worker != null && workersText != null)
        {
            workersText.text = $"Workers: {worker.GetCurrentWorkers()}/{worker.GetMaxWorkers()}";
        }

        // Update upgrade button
        if (upgradeButton != null && upgrade != null)
        {
            bool canUpgrade = (upgrade.GetCurrentLevel() < upgrade.GetMaxLevel());
            upgradeButton.gameObject.SetActive(canUpgrade);
        }
    }

    // Button Callbacks
    public void OnFixClicked()
    {
        if (currentWagon == null) return;
        var durability = currentWagon.GetComponent<DurabilityComponent>();
        if (durability == null) return;

        float currentDur = durability.GetDurability();
        float maxDur = durability.GetMaxDurability();
        float missingDur = maxDur - currentDur;
        if (missingDur <= 0f) return;

        float cost = fixCostPerDurability * missingDur;
        float availableWood = gameManager.GetResourceManager().GetResourceAmount(ResourceType.WOOD);
        if (availableWood >= cost)
        {
            gameManager.GetResourceManager().RemoveResource(ResourceType.WOOD, cost);
            durability.Repair(missingDur);
            Debug.Log("Wagon fixed!");
        }
        else
        {
            Debug.Log("Not enough wood to fix!");
        }
        RefreshPanel();
    }

    public void OnPlusWorkerClicked()
    {
        if (currentWagon == null) return;
        var worker = currentWagon.GetComponent<WorkerComponent>();
        if (worker == null) return;

        bool success = worker.AddWorkers(1);
        if (!success)
        {
            Debug.Log("Failed to add worker.");
        }
        RefreshPanel();
    }

    public void OnMinusWorkerClicked()
    {
        if (currentWagon == null) return;
        var worker = currentWagon.GetComponent<WorkerComponent>();
        if (worker == null) return;

        bool success = worker.RemoveWorkers(1);
        if (!success)
        {
            Debug.Log("Failed to remove worker.");
        }
        RefreshPanel();
    }

    public void OnDestroyClicked()
    {
        if (destroyConfirmPanel != null)
            destroyConfirmPanel.SetActive(true);
    }

    public void OnDestroyConfirm()
    {
        // gameManager.TestWagonRemoval(currentWagon);
        // if (currentWagon == null) return;
        gameManager.GetWagonManager().DestroyWagon(currentWagon);
        HidePanel();
    }

    public void OnDestroyCancel()
    {
        if (destroyConfirmPanel != null)
            destroyConfirmPanel.SetActive(false);
    }

    public void OnUpgradeClicked()
    {
        if (currentWagon == null) return;
        var upgrade = currentWagon.GetComponent<UpgradeComponent>();
        if (upgrade == null) return;

        bool success = upgrade.TryUpgrade();
        if (!success)
        {
            Debug.Log("Upgrade failed!");
        }
        RefreshPanel();
    }
}
