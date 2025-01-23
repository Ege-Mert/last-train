using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarketButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite normalSprite;  // Default sprite
    public Sprite hoverSprite;   // Hover sprite

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>(); // Get the Image component of the button
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.sprite = hoverSprite; // Change to hover sprite
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.sprite = normalSprite; // Change back to normal sprite
    }
}
