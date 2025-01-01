using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainVisualManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wagonAttachPoint;
    [SerializeField] private float wagonSpacing = 0.1f;
    
    [Header("Train Base")]
    [SerializeField] private SpriteRenderer trainBaseSprite;
    [SerializeField] private Transform[] trainWheels;
    
    [Header("Animation")]
    [SerializeField] private float maxWheelRotationSpeed = 360f;
    [SerializeField] private float trainMaxSpeed = 10f;

    private GameManager gameManager;
    private List<WagonVisuals> wagonVisuals = new List<WagonVisuals>();
    private bool isInitialized = false;
    // private bool isReorganizing = false;
    private int movingWagonsCount = 0;

    public Transform GetWagonAttachPoint() => wagonAttachPoint;

    public void Initialize(GameManager gm)
    {
        gameManager = gm;
        
        if (wagonAttachPoint == null)
        {
            Debug.LogError("WagonAttachPoint not assigned to TrainVisualManager!");
            return;
        }

        // Subscribe to wagon events
        if (gameManager?.GetWagonManager() != null)
        {
            gameManager.GetWagonManager().OnWagonAdded += HandleWagonAdded;
            gameManager.GetWagonManager().OnWagonRemoved += HandleWagonRemoved;
        }
        
        isInitialized = true;
        
        // Initialize existing wagons
        StartCoroutine(InitializeExistingWagons());
    }

    private IEnumerator InitializeExistingWagons()
    {
        yield return new WaitForSeconds(0.1f); // Wait for everything to settle
        
        var wagons = gameManager.GetWagonManager().GetActiveWagons();
        foreach (var wagon in wagons)
        {
            var visuals = wagon.GetComponentInChildren<WagonVisuals>();
            if (visuals != null && !wagonVisuals.Contains(visuals))
            {
                HandleWagonAdded(wagon);
            }
        }
    }

    private void HandleWagonAdded(Wagon wagon)
    {
        var visuals = wagon.GetComponentInChildren<WagonVisuals>();
        if (visuals != null && !wagonVisuals.Contains(visuals))
        {
            wagonVisuals.Add(visuals);
            visuals.Initialize();
            StartCoroutine(ReorganizeWagonsDelayed());
        }
    }

    private void HandleWagonRemoved(Wagon wagon)
    {
        var visuals = wagon.GetComponentInChildren<WagonVisuals>();
        if (visuals != null)
        {
            wagonVisuals.Remove(visuals);
            // Start reorganization after removal
             // Reorganize wagons for every wagon count

            StartCoroutine(ReorganizeWagonsDelayed());

        }
    }

    private IEnumerator ReorganizeWagonsDelayed()
    {
        // Wait a frame to ensure all transforms are updated
        yield return null;
            StartCoroutine(ReorganizeWagons());
        
    }

    private IEnumerator ReorganizeWagons()
    {
        if (wagonAttachPoint == null)
        {
            yield break;
        }

        Vector3 nextPosition = wagonAttachPoint.position;
        movingWagonsCount = 0;

        // Move each wagon to its proper position
        for (int i = 0; i < wagonVisuals.Count; i++)
        {
            var visuals = wagonVisuals[i];
            if (visuals == null) continue;

            Vector3 frontConnectorOffset = visuals.GetFrontConnectorPosition() - visuals.transform.position;
            Vector3 targetPos = nextPosition - frontConnectorOffset;

            // Update next position using back connector
            nextPosition = visuals.GetBackConnectorPosition() + (Vector3.right * wagonSpacing);

            visuals.MoveTo(targetPos);
            movingWagonsCount++;
        }

        // Wait until all wagons complete their movement
        while (movingWagonsCount > 0)
        {
            yield return null;
        }
    }

    private void Update()
    {
        if (!isInitialized) return;
        UpdateWheelRotations();
    }

    private void UpdateWheelRotations()
    {
        if (gameManager?.GetTrainBase() == null) return;

        float currentSpeed = gameManager.GetTrainBase().GetCurrentSpeed();
        float speedRatio = Mathf.Clamp01(currentSpeed / trainMaxSpeed);
        float rotationAmount = speedRatio * maxWheelRotationSpeed * Time.deltaTime;

        foreach (var wheel in trainWheels)
        {
            if (wheel != null)
            {
                wheel.Rotate(0, 0, -rotationAmount);
            }
        }
    }
}