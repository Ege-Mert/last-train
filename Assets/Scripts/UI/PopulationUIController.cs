using UnityEngine;
using TMPro;

public class PopulationUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text populationText; 
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color overCapacityColor = Color.red;

    private GameManager gm;

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        
    }
    
    private void Update()
    {
        RefreshPopulationUI();
    }

    public void RefreshPopulationUI()
    {
        var cm = gm.GetCentralHumanManager();
        int totalPop = cm.GetTotalHumans();
        int avail = cm.GetAvailableHumans();
        int capacity = cm.GetSleepingCapacity();

        // Example display: "Humans: 40 (available) / 50 (total) - capacity 45"
        // If total > capacity, text is red
        populationText.text = $"{avail} / {capacity}";

        if (gm.GetCentralHumanManager().IsOverCapacity())
            populationText.color = Color.red;
        else
            populationText.color = Color.white;
    }
}
