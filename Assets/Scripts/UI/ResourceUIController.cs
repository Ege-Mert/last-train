using TMPro;
using UnityEngine;

public class ResourceUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private TMP_Text weightText;
    [SerializeField] private TMP_Text coalTotalText;
    [SerializeField] private GameObject coalDetailPanel;
    [SerializeField] private TMP_Text coalLowText;
    [SerializeField] private TMP_Text coalMedText;
    [SerializeField] private TMP_Text coalHighText;

    private GameManager gm;

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        coalDetailPanel.SetActive(false);
    }
    
    private void Update()
    {
        if (gm != null)
        {
            RefreshResources();
        }
    }

    public void RefreshResources()
    {
        var rm = gm.GetResourceManager();

        // Resources
        float woodAmount = rm.GetResourceAmount(ResourceType.WOOD);
        float scrapAmount = rm.GetResourceAmount(ResourceType.SCRAP);
        woodText.text = $"Wood: {woodAmount:F1}";
        scrapText.text = $"Scrap: {scrapAmount:F1}";

        // Weight
        float totalWeight = rm.GetTotalWeight();
        float weightCapacity = rm.GetWeightCapacity();
        weightText.text = $"Weight: {totalWeight:F1} / {weightCapacity:F1}";

        // Color
        weightText.color = rm.GetIsOverCapacity() ? Color.red : Color.white;

        // Coal
        float low = rm.GetResourceAmount(ResourceType.COAL_LOW);
        float med = rm.GetResourceAmount(ResourceType.COAL_MED);
        float high = rm.GetResourceAmount(ResourceType.COAL_HIGH);
        float total = low + med + high;

        coalTotalText.text = $"Coal: {total:F1}";
        coalLowText.text = $"Low: {low:F1}";
        coalMedText.text = $"Med: {med:F1}";
        coalHighText.text = $"High: {high:F1}";
    }

    public void OnCoalHoverEnter()
    {
        // If you want hover to open detail
        coalDetailPanel.SetActive(true);
    }
    public void OnCoalHoverExit()
    {
        coalDetailPanel.SetActive(false);
    }
}
