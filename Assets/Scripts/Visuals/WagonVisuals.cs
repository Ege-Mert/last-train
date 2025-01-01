using UnityEngine;

public class WagonVisuals : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private SpriteRenderer mainSprite;
    [SerializeField] private Transform frontConnector;
    [SerializeField] private Transform backConnector;
    
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite damagedSprite;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 500f; // Increased for snappier movement
    [SerializeField] private float snapDistance = 0.01f;
    
    private Vector3 targetPosition;
    private Wagon wagonComponent;
    private DurabilityComponent durabilityComponent;
    private bool isInitialized = false;
    private bool isMoving = false;

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

    private void ValidateSetup()
    {
        if (frontConnector == null || backConnector == null)
        {
            Debug.LogError($"Wagon {gameObject.name} is missing connector references!");
        }
    }

    public void Initialize()
    {
        isInitialized = true;
    }

    public Vector3 GetFrontConnectorPosition() => frontConnector != null ? frontConnector.position : transform.position;
    public Vector3 GetBackConnectorPosition() => backConnector != null ? backConnector.position : transform.position + Vector3.right;

    public void MoveTo(Vector3 newPosition)
    {
        if (!isInitialized) return;
        
        targetPosition = newPosition;
        isMoving = true;
        Debug.Log($"Moving wagon {gameObject.name} to {targetPosition}");
    }

    public bool IsMoving() => isMoving;

    private void Update()
    {
        if (!isInitialized || !isMoving) return;

        if (Vector3.Distance(transform.position, targetPosition) > snapDistance)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else if (transform.position != targetPosition)
        {
            transform.position = targetPosition;
            isMoving = false;
            // Notify that movement is complete
            SendMessageUpwards("OnWagonMovementComplete", gameObject, SendMessageOptions.DontRequireReceiver);
        }

        UpdateDurabilityVisual();
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
}