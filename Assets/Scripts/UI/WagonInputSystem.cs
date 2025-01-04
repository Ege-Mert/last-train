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
    [SerializeField] private ShopPanelController shopPanel;


    private void Awake()
    {
        // you could also find the raycaster at runtime if you prefer:
        // uiRaycaster = FindObjectOfType<GraphicRaycaster>();
    }
    public void OnLeftClick(InputValue value)
    {
        if (!value.isPressed) return;

        if (ClickedOnUI())
        {
            return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, wagonLayer);
        if (hit.collider != null)
        {
            var wagon = hit.collider.GetComponentInParent<Wagon>();
            if (wagon != null)
            {
                // Close shop if open
                if (shopPanel != null && shopPanel.IsShopOpen())
                {
                    shopPanel.HideShop();
                }

                if (wagonPanel.IsOpenForThisWagon(wagon))
                {
                    wagonPanel.HidePanel();
                }
                else
                {
                    wagonPanel.ShowPanel(wagon);
                }
                return;
            }
        }

        // If we clicked empty space and no UI was hit, hide both panels
        wagonPanel.HidePanel();
        if (shopPanel != null)
        {
            shopPanel.HideShop();
        }
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
