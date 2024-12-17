using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Main Controls")]
    [SerializeField] private Button createWagonButton;
    [SerializeField] private WagonType wagonTypeToSpawn;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        SetupCreateWagonButton();
    }

    private void SetupCreateWagonButton()
    {
        if (createWagonButton != null)
        {
            createWagonButton.onClick.AddListener(HandleWagonCreation);
        }
    }

    private void HandleWagonCreation()
    {
        bool canBuild = gameManager.GetBuildingManager().CanBuildWagon(wagonTypeToSpawn); 
        var buildData = gameManager.GetBuildingManager().GetBuildData(wagonTypeToSpawn);
        Debug.Log($"Build data for {wagonTypeToSpawn}: {(buildData != null ? "Found" : "Not Found")}");
        Debug.Log($"Can build {wagonTypeToSpawn}: " + canBuild);

        // If can build, try building it:
        if (canBuild)
        {
            gameManager.GetBuildingManager().TryBuildWagon(wagonTypeToSpawn, this.transform);
        }
        gameManager.GetResourceManager().AddResource(ResourceType.WOOD, 100f);
    }

    private void OnWagonButtonClicked(Wagon wagon)
    {
        // Using existing passenger adding logic from Wagon
        gameManager.GetCentralHumanManager().AssignWorkers(wagon, 1);
    }
}
