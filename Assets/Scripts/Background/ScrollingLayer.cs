using UnityEngine;

/// <summary>
/// A single scrolling layer that moves left based on
/// an externally provided speed and a parallax factor.
/// If it moves too far left, it repositions to the right for seamless looping.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ScrollingLayer : MonoBehaviour
{
    [Tooltip("How strongly this layer reacts to train speed. 1 = same speed, 0.5 = half speed, etc.")]
    [Range(0f, 2f)]
    public float parallaxFactor = 1f;

    private float spriteWidth;  // width of the sprite used for looping
    private float startX;       // initial x-position
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Call this each frame from the manager, passing the current train speed.
    /// </summary>
    public void Scroll(float speed, float deltaTime)
    {
        // We move left by speed * parallaxFactor
        float moveDist = speed * parallaxFactor * deltaTime;

        // shift our transform to the left
        transform.Translate(-moveDist, 0f, 0f);

        // figure out how far we've moved from the start
        float distanceFromStart = transform.position.x - startX;

        // if we've moved left enough to go beyond one full sprite width,
        // we reposition to the right to loop seamlessly
        if (distanceFromStart <= -spriteWidth)
        {
            float overshoot = distanceFromStart + spriteWidth; 
            // if distanceFromStart = -10 and spriteWidth = 8, overshoot = -2
            // we add spriteWidth to move it back, but keep any leftover offset
            transform.position = new Vector3(startX + overshoot, transform.position.y, transform.position.z);
        }
        else if (distanceFromStart >= spriteWidth)
        {
            // if we somehow went too far right (less common unless speed is negative),
            // we handle that as well
            float overshoot = distanceFromStart - spriteWidth;
            transform.position = new Vector3(startX + overshoot, transform.position.y, transform.position.z);
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // measure the sprite width in world space
        float spritePixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        float spritePixelWidth = spriteRenderer.sprite.rect.width; 
        // rect.width is the pixel width of the sprite
        spriteWidth = (spritePixelWidth / spritePixelsPerUnit) * transform.localScale.x;

        // record initial x position
        startX = transform.position.x;
    }
}
