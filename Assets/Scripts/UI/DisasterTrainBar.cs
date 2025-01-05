using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisasterTrainBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Slider disasterSlider;
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text disasterDistanceText;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color dangerColor = Color.red;
    [SerializeField] private float warningThreshold = 0.3f; // Distance in percentage when color changes to yellow
    [SerializeField] private float dangerThreshold = 0.15f; // Distance in percentage when color changes to red

    private GameManager gm;
    private Image progressFillImage;
    private float finalDistance;

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        finalDistance = gm.gameBalanceData.finalDestinationDistance;
        
        // Setup sliders
        progressSlider.minValue = 0;
        progressSlider.maxValue = finalDistance;
        disasterSlider.minValue = 0;
        disasterSlider.maxValue = finalDistance;

        // Cache the fill image reference
        progressFillImage = progressSlider.fillRect.GetComponent<Image>();
    }

    private void Update()
    {
        if (gm == null) return;
        RefreshBar();
    }

    public void RefreshBar()
    {
        float trainDist = gm.GetGameProgressManager().GetDistance();
        float disasterDist = gm.GetDisasterManager().GetDisasterPosition();
        
        // Update sliders
        progressSlider.value = trainDist;
        disasterSlider.value = disasterDist;

        // Update distance texts
        if (distanceText != null)
            distanceText.text = $"{trainDist:F1}m";
        if (disasterDistanceText != null)
            disasterDistanceText.text = $"{disasterDist:F1}m";

        // Calculate distance between train and disaster
        float distance = trainDist - disasterDist;
        float distancePercent = distance / finalDistance;

        // Update color based on distance
        if (progressFillImage != null)
        {
            if (distancePercent <= dangerThreshold)
                progressFillImage.color = dangerColor;
            else if (distancePercent <= warningThreshold)
                progressFillImage.color = warningColor;
            else
                progressFillImage.color = normalColor;
        }
    }
}