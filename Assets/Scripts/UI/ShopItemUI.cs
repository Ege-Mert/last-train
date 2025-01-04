using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buyButton;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Color unaffordableColor = new Color(0.7f, 0.7f, 0.7f);
    
    private ShopItemSO itemData;
    private GameManager gm;
    private Color defaultTextColor;

    public void Setup(ShopItemSO data, GameManager gameManager)
    {
        if (gameManager == null || data == null)
        {
            Debug.LogError("Null reference in ShopItemUI Setup!");
            return;
        }

        itemData = data;
        gm = gameManager;
        defaultTextColor = costText ? costText.color : Color.white;

        if (icon) icon.sprite = data.itemIcon;
        if (nameText) nameText.text = data.itemName;
        if (descriptionText) descriptionText.text = data.description;

        UpdateCostText();
        
        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    private void UpdateCostText()
    {
        if (!costText || itemData == null) return;

        string costString = "";
        foreach (var cost in itemData.cost)
        {
            costString += $"{cost.type}: {cost.amount}\n";
        }
        costText.text = costString.TrimEnd('\n');
    }

    public void SetInteractable(bool interactable)
    {
        if (buyButton) buyButton.interactable = interactable;
        if (costText) costText.color = interactable ? defaultTextColor : unaffordableColor;
    }

    public ShopItemSO GetItemData() => itemData;

    private void OnBuyClicked()
    {
        if (!gm || !itemData) return;

        bool success = gm.GetWagonManager().TryBuildWagon(itemData.wagonType);
        
        if (success)
        {
            Debug.Log($"Built {itemData.itemName}!");
            // Refresh the shop panel to update affordability
            if (transform.parent.parent.TryGetComponent<ShopPanelController>(out var shop))
            {
                shop.RefreshAffordability();
            }
        }
        else
        {
            Debug.Log($"Could not build {itemData.itemName}.");
        }
    }
}