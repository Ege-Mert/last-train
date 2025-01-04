
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerUI : MonoBehaviour
{
    [SerializeField] private Image needleImage;  // the needle
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;

    private GameManager gm;
    private TrainBase trainBase;

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
        trainBase = gm.GetTrainBase();
    }

    public void RefreshSpeedometer()
    {
        if (trainBase == null) return;
        float currentSpeed = trainBase.GetCurrentSpeed();
        float maxSpeed = trainBase.GetMaxSpeed(); // or trainBase.maxSpeed if defined

        float normalized = Mathf.InverseLerp(0, maxSpeed, currentSpeed);
        float angle = Mathf.Lerp(minAngle, maxAngle, normalized);

        // rotate the needle
        needleImage.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
