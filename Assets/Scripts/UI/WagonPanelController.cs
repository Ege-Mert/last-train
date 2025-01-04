using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class WagonPanelController : MonoBehaviour
{
    [Header("UI References")]
    
    [SerializeField] private TMP_Text wagonNameText;
    [SerializeField] private TMP_Text productionRateText;
    [SerializeField] private TMP_Text conversionRateText;
    [SerializeField] private TMP_Text durabilityText;
    [SerializeField] private TMP_Text workersText;
    [SerializeField] private Button fixButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject destroyConfirmPanel;
    [SerializeField] private Image panelImage;
    [SerializeField] private Button destroyConfirmButton;
    [SerializeField] private Button destroyCancelButton;
    
    [SerializeField] private List<WagonUIStyle> wagonStyles;
    
    [Header("References")]
    [SerializeField] private ShopPanelController shopPanel;


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
        // Close shop if open
        if (shopPanel != null && shopPanel.IsShopOpen())
        {
            shopPanel.HideShop();
        }
    
        currentWagon = wagon;
        gameObject.SetActive(true);
        panelVisible = true;
    
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
        var collector = currentWagon.GetComponent<CollectorComponent>();
        var converter = currentWagon.GetComponent<ConverterComponent>();
        var style = GetStyleForType(currentWagon.GetWagonType());
        
        
        bool hasWorker = (worker != null);
        
        plusButton.gameObject.SetActive(hasWorker);
        minusButton.gameObject.SetActive(hasWorker);
        
        
        bool hasDurability = (durability != null);

        fixButton.gameObject.SetActive(hasDurability);
        // Update durability text
        if (hasDurability)
        {
            float pct = durability.GetDurabilityPercent() * 100f;
            durabilityText.text = $"Durability: {pct:0.0}%";
            fixButton.interactable = (durability.GetDurabilityPercent() < fixThreshold);
        }
        else
        {
            durabilityText.text = ""; // No need to enter a thing but also I am too lazy to disable the thing
        }
        
        
        // Update worker text
        if (hasWorker)
        {
            workersText.text = $"Workers: {worker.GetCurrentWorkers()}/{worker.GetMaxWorkers()}";
        }
        else
        {
            workersText.text = ""; // No need to enter a thing but also I am too lazy to disable the thing
        }

        // Update upgrade button
        bool hasUpgrade = (upgrade != null);
        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(hasUpgrade);
            if (hasUpgrade)
            {
                bool canUpgrade = (upgrade.GetCurrentLevel() < upgrade.GetMaxLevel());
                upgradeButton.interactable = canUpgrade;
            }
        }

        // Update collector text
        if (collector != null)
        {
            float baseRate = collector.GetCollectionRate(); 
            // or if it has a formula
            productionRateText.text = $"Collects: {baseRate} /sec";
            productionRateText.gameObject.SetActive(true);
        }
        else
        {
            productionRateText.gameObject.SetActive(false);
        }
        
        // Update converter text
        if (converter != null)
        {
            float baseRate = converter.GetConvertingRate(); 
            // or if it has a formula
            conversionRateText.text = $"Converts: {baseRate} /sec";
            conversionRateText.gameObject.SetActive(true);
        }
        else
        {
            conversionRateText.gameObject.SetActive(false);
        }
        
        // Update wagon name
        var baseWagon = currentWagon; // the root wagon script
        if (wagonNameText != null)
        {
            wagonNameText.text = baseWagon ? baseWagon.GetWagonName() : "Unknown Wagon";
        }
        
        // Update panel image
        if (panelImage != null)
        {
            if (style != null)
            {
                panelImage.sprite = style.panelBackground;
            }
        }
    }
    
    public WagonUIStyle GetStyleForType(WagonType type)
    {
        foreach (var style in wagonStyles)
        {
            if (style.wagonType == type) return style;
        }
        return null;
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
    
    public bool IsOpenForThisWagon(Wagon checkWagon)
    {
        return panelVisible && currentWagon == checkWagon;
    }
}
