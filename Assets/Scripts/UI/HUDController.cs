using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Wagon Spawn Buttons")]
    [SerializeField] private Button woodCollectorButton;
    [SerializeField] private Button scrapCollectorButton;
    [SerializeField] private Button converterButton;
    [SerializeField] private Button storageButton;
    [SerializeField] private Button sleepingButton;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (woodCollectorButton != null)
            woodCollectorButton.onClick.AddListener(() => HandleWagonCreation(WagonType.WOOD_COLLECTOR));
        
        if (scrapCollectorButton != null)
            scrapCollectorButton.onClick.AddListener(() => HandleWagonCreation(WagonType.SCRAP_COLLECTOR));
        
        if (converterButton != null)
            converterButton.onClick.AddListener(() => HandleWagonCreation(WagonType.CONVERTER));
        
        if (storageButton != null)
            storageButton.onClick.AddListener(() => HandleWagonCreation(WagonType.STORAGE));
        
        if (sleepingButton != null)
            sleepingButton.onClick.AddListener(() => HandleWagonCreation(WagonType.SLEEPING));
    }

    private void HandleWagonCreation(WagonType wagonType)
    {
        bool canBuild = gameManager.GetBuildingManager().CanBuildWagon(wagonType);
        var buildData = gameManager.GetBuildingManager().GetBuildData(wagonType);
        Debug.Log($"Build data for {wagonType}: {(buildData != null ? "Found" : "Not Found")}");
        Debug.Log($"Can build {wagonType}: " + canBuild);

        if (canBuild)
        {
            gameManager.GetBuildingManager().TryBuildWagon(wagonType, this.transform);
        }
        gameManager.GetResourceManager().AddResourcePartial(ResourceType.WOOD, 100f);
    }

    private void OnWagonButtonClicked(Wagon wagon)
    {
        gameManager.GetCentralHumanManager().AssignWorkers(wagon, 1);
    }
}