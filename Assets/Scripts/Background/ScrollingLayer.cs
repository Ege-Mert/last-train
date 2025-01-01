using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScrollingLayer : MonoBehaviour
{
    [Tooltip("How strongly this layer reacts to train speed. 1 = same speed, 0.5 = half speed, etc.")]
    [Range(0f, 2f)]
    public float parallaxFactor = 1f;

    private float spriteWidth;
    private float startX;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastCameraPosition;
    private Camera mainCamera;

    public void Scroll(float speed, float deltaTime)
    {
        if (mainCamera == null) return;

        // Handle train movement scrolling
        float trainMoveDist = speed * parallaxFactor * deltaTime;
        transform.Translate(-trainMoveDist, 0f, 0f);

        // Handle camera movement parallax
        Vector3 cameraDelta = mainCamera.transform.position - lastCameraPosition;
        if (cameraDelta.x != 0)
        {
            float cameraParallaxDist = cameraDelta.x * (1f - parallaxFactor);
            transform.Translate(cameraParallaxDist, 0f, 0f);
        }
        lastCameraPosition = mainCamera.transform.position;

        // Handle looping based on camera position
        LoopAroundCamera();
    }

    private void LoopAroundCamera()
    {
        // Get the camera's view bounds
        float cameraX = mainCamera.transform.position.x;
        float viewportHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        
        // Get the screen point of our position
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        
        // Calculate our position relative to camera viewport
        float distanceFromCamera = transform.position.x - cameraX;

        // If we're too far to the left, move right
        if (viewportPoint.x < -0.5f)  // If completely off screen to the left
        {
            float newX = transform.position.x + spriteWidth;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        // If we're too far to the right, move left
        else if (viewportPoint.x > 1.5f)  // If completely off screen to the right
        {
            float newX = transform.position.x - spriteWidth;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Calculate sprite width in world space
        float spritePixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        float spritePixelWidth = spriteRenderer.sprite.rect.width;
        spriteWidth = (spritePixelWidth / spritePixelsPerUnit) * transform.localScale.x;

        // Record initial positions
        startX = transform.position.x;
        if (mainCamera != null)
        {
            lastCameraPosition = mainCamera.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        // Debug visualization
        if (spriteRenderer != null)
        {
            Gizmos.color = Color.yellow;
            Bounds bounds = spriteRenderer.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}