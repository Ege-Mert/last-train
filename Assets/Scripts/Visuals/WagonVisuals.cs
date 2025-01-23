using UnityEngine;
using System.Collections.Generic;

public class WagonVisuals : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private Transform frontConnector;
    [SerializeField] private Transform backConnector;
    
    [Header("Wheel Setup")]
    [SerializeField] private List<Animator> wheelAnimators;
    [SerializeField] private float maxWheelRotationSpeed = 360f; // Match with TrainVisualManager
    [SerializeField] private float trainMaxSpeed = 10f;
    
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite damagedSprite;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 500f;
    [SerializeField] private float snapDistance = 0.01f;
    
    private Vector3 targetPosition;
    private Wagon wagonComponent;
    private DurabilityComponent durabilityComponent;
    private bool isInitialized = false;
    private bool isMoving = false;
    private TrainBase trainBase;

    private void Awake()
    {
        wagonComponent = GetComponentInParent<Wagon>();
        durabilityComponent = GetComponentInParent<DurabilityComponent>();
        targetPosition = transform.position;
        
        if (mainSprite != null && normalSprite != null)
        {
            mainSprite.sprite = normalSprite;
        }
        
        ValidateSetup();
    }

    public void Initialize()
    {
        isInitialized = true;
        
        // Get TrainBase reference through Wagon's GameManager getter
        if (wagonComponent != null)
        {
            GameManager gm = wagonComponent.GetGameManager();
            if (gm != null)
            {
                trainBase = gm.GetTrainBase();
            }
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (isMoving)
        {
            if (Vector3.Distance(transform.position, targetPosition) > snapDistance)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
                isMoving = false;
                SendMessageUpwards("OnWagonMovementComplete", gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }

        UpdateWheelAnimations();
        UpdateDurabilityVisual();
    }

    private void UpdateWheelAnimations()
    {
        if (trainBase == null || wheelAnimators == null) return;

        float currentSpeed = trainBase.GetCurrentSpeed();
        float speedRatio = Mathf.Clamp01(currentSpeed / trainMaxSpeed);
        
        // Use the same animation speed calculation as TrainVisualManager
        float animationSpeed = speedRatio * maxWheelRotationSpeed;

        foreach (var animator in wheelAnimators)
        {
            if (animator != null)
            {
                // Set the animation speed directly like TrainVisualManager
                animator.speed = animationSpeed;
            }
        }
    }

    public Vector3 GetFrontConnectorPosition() => frontConnector != null ? frontConnector.position : transform.position;
    public Vector3 GetBackConnectorPosition() => backConnector != null ? backConnector.position : transform.position + Vector3.right;

    public void MoveTo(Vector3 newPosition)
    {
        if (!isInitialized) return;
        targetPosition = newPosition;
        isMoving = true;
    }

    private void UpdateDurabilityVisual()
    {
        if (durabilityComponent == null || mainSprite == null) return;

        float durabilityPercent = durabilityComponent.GetDurabilityPercent();
        bool shouldShowDamaged = durabilityPercent <= 0.3f;

        if (shouldShowDamaged && mainSprite.sprite != damagedSprite && damagedSprite != null)
        {
            mainSprite.sprite = damagedSprite;
        }
        else if (!shouldShowDamaged && mainSprite.sprite != normalSprite && normalSprite != null)
        {
            mainSprite.sprite = normalSprite;
        }
    }

    private void ValidateSetup()
    {
        if (frontConnector == null || backConnector == null)
        {
            Debug.LogError($"Wagon {gameObject.name} is missing connector references!");
        }

        if (wheelAnimators == null || wheelAnimators.Count == 0)
        {
            Debug.LogWarning($"Wagon {gameObject.name} has no wheel animators assigned!");
        }
    }
}