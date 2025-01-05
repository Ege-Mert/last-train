using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedometerUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image needleImage;
    [SerializeField] private TMP_Text speedText;
    
    [Header("Needle Settings")]
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;
    [SerializeField] private float needleDampening = 5f; // Smoothing factor
    
    [Header("Speed Thresholds")]
    [SerializeField] private float lowSpeedThreshold = 0.3f;
    [SerializeField] private float mediumSpeedThreshold = 0.7f;
    [SerializeField] private Color lowSpeedColor = Color.red;
    [SerializeField] private Color mediumSpeedColor = Color.yellow;
    [SerializeField] private Color highSpeedColor = Color.green;

    private GameManager gm;
    private TrainBase trainBase;
    private float targetAngle;
    private float currentAngle;

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        trainBase = gm.GetTrainBase();
        if (needleImage != null)
            currentAngle = needleImage.rectTransform.localEulerAngles.z;
    }

    private void Update()
    {
        if (trainBase == null) return;
        RefreshSpeedometer();
    }

    public void RefreshSpeedometer()
    {
        float currentSpeed = trainBase.GetCurrentSpeed();
        float maxSpeed = trainBase.GetMaxSpeed();

        // Update speed text if available
        if (speedText != null)
            speedText.text = $"{currentSpeed:F1} km/h";

        // Calculate target angle
        float normalized = Mathf.InverseLerp(0, maxSpeed, currentSpeed);
        targetAngle = Mathf.Lerp(minAngle, maxAngle, normalized);

        // Smooth needle movement
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * needleDampening);

        // Update needle rotation
        if (needleImage != null)
        {
            needleImage.rectTransform.localEulerAngles = new Vector3(0, 0, currentAngle);
            
            // Update color based on speed
            if (normalized <= lowSpeedThreshold)
                needleImage.color = lowSpeedColor;
            else if (normalized <= mediumSpeedThreshold)
                needleImage.color = mediumSpeedColor;
            else
                needleImage.color = highSpeedColor;
        }
    }
}