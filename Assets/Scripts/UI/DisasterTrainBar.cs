
using UnityEngine;

public class DisasterTrainBar : MonoBehaviour
{
    [SerializeField] private RectTransform greenFill;
    [SerializeField] private RectTransform redFill;
    [SerializeField] private RectTransform barBackground;
    private GameManager gm;

    public void Initialize(GameManager gameManager)
    {
        gm = gameManager;
    }
    
    private void Update()
    {
        RefreshBar();
    }

    public void RefreshBar()
    {
        float trainDist = gm.GetGameProgressManager().GetDistance();
        float disasterDist = gm.GetDisasterManager().GetDisasterPosition();
        float finalDist = gm.gameBalanceData.finalDestinationDistance;

        float barWidth = barBackground.sizeDelta.x; 
        float trainWidth = barWidth * (trainDist / finalDist);
        float disasterWidth = barWidth * (disasterDist / finalDist);

        // set greenFill width:
        greenFill.sizeDelta = new Vector2(trainWidth, greenFill.sizeDelta.y);
        // set redFill width
        redFill.sizeDelta = new Vector2(disasterWidth, redFill.sizeDelta.y);
    }
}
