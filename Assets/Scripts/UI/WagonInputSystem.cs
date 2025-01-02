using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;         // For GraphicRaycaster
using UnityEngine.EventSystems; // For PointerEventData, RaycastResult
using System.Collections.Generic;

public class WagonInputSystem : MonoBehaviour
{
    [SerializeField] private LayerMask wagonLayer;
    [SerializeField] private WagonPanelController wagonPanel;

    // Reference the main UI's GraphicRaycaster
    [SerializeField] private GraphicRaycaster uiRaycaster;

    private void Awake()
    {
        // you could also find the raycaster at runtime if you prefer:
        // uiRaycaster = FindObjectOfType<GraphicRaycaster>();
    }

    public void OnLeftClick(InputValue value)
    {
        if (!value.isPressed) return;

        // Step 1) Check if the click hits any UI
        if (ClickedOnUI())
        {
            // We clicked on the UI, so do nothing special for wagons
            // (Don't hide the panel, don't open a new one)
            return;
        }

        // Step 2) If no UI was hit, do wagon detection
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, wagonLayer);
        if (hit.collider != null)
        {
            var wagon = hit.collider.GetComponentInParent<Wagon>();
            if (wagon != null)
            {
                // Show single UI panel for this wagon
                wagonPanel.ShowPanel(wagon);
                return;
            }
        }

        // Step 3) If no wagon was hit, hide the panel
        wagonPanel.HidePanel();
    }

    private bool ClickedOnUI()
    {
        // Create a pointer event data for the current mouse position
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        Vector2 mousePos = Mouse.current.position.ReadValue();
        pointerData.position = mousePos;

        // RaycastAll into the UI
        List<RaycastResult> uiHits = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerData, uiHits);

        // If we got any hits, it means we clicked on some UI element
        return uiHits.Count > 0;
    }
}
