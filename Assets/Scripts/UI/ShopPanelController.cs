using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ShopPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemUIPrefab;
    [SerializeField] private List<ShopItemSO> allShopItems;
    
    [Header("Category UI")]
    [SerializeField] private Transform categoryButtonContainer; // Parent for category buttons
    [SerializeField] private Button categoryButtonPrefab;
    [SerializeField] private List<ShopCategorySO> categories; // All available categories
    
    [Header("Visual Settings")]
    [SerializeField] private Color selectedCategoryColor = new Color(0.7f, 0.7f, 1f);
    [SerializeField] private Color defaultCategoryColor = Color.white;
    
    [Header("References")]
    [SerializeField] private WagonPanelController wagonPanel;
    
    private GameManager gameManager;
    private string currentCategory;
    private bool isShopOpen = false;
    private Dictionary<string, Button> categoryButtons = new Dictionary<string, Button>();

    public void Initialize(GameManager gm)
    {
        if (gm == null)
        {
            Debug.LogError("Trying to initialize ShopPanelController with null GameManager!");
            return;
        }

        gameManager = gm;
        SetupCategories();
        HideShop();
    }

    private void SetupCategories()
    {
        // Clear existing buttons
        foreach (Transform child in categoryButtonContainer)
        {
            Destroy(child.gameObject);
        }
        categoryButtons.Clear();

        // Create buttons for each category
        foreach (var category in categories)
        {
            Button btn = Instantiate(categoryButtonPrefab, categoryButtonContainer);
            
            // Setup button visuals
            if (btn.TryGetComponent<Image>(out var btnImage))
            {
                btnImage.color = defaultCategoryColor;
            }
            
            // Setup button text
            if (btn.GetComponentInChildren<TMPro.TMP_Text>() is TMPro.TMP_Text btnText)
            {
                btnText.text = category.categoryName;
            }
            
            // Setup button icon if available
            if (category.categoryIcon != null && btn.transform.Find("Icon")?.GetComponent<Image>() is Image iconImage)
            {
                iconImage.sprite = category.categoryIcon;
                iconImage.gameObject.SetActive(true);
            }

            // Store reference and add listener
            categoryButtons[category.categoryName] = btn;
            string categoryName = category.categoryName; // Create local variable for closure
            btn.onClick.AddListener(() => ShowCategory(categoryName));
        }

        // Select first category by default if any exist
        if (categories.Count > 0)
        {
            currentCategory = categories[0].categoryName;
        }
    }

    public void ShowShop()
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager is null in ShopPanelController! Did you forget to call Initialize?");
            return;
        }

        // Close wagon panel if open
        if (wagonPanel != null)
        {
            wagonPanel.HidePanel();
        }

        shopPanel.SetActive(true);
        isShopOpen = true;
        ShowCategory(currentCategory);
    }

    public void HideShop()
    {
        shopPanel.SetActive(false);
        isShopOpen = false;
    }

    public bool IsShopOpen() => isShopOpen;

    // Toggle shop visibility
    public void ToggleShop()
    {
        if (isShopOpen)
        {
            HideShop();
        }
        else
        {
            ShowShop();
        }
    }

    private void ShowCategory(string categoryName)
    {
        currentCategory = categoryName;
        
        // Update button visuals
        foreach (var kvp in categoryButtons)
        {
            if (kvp.Value.TryGetComponent<Image>(out var btnImage))
            {
                btnImage.color = kvp.Key == categoryName ? selectedCategoryColor : defaultCategoryColor;
            }
        }

        // Clear existing items
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // Filter and sort items
        var categoryItems = allShopItems
            .Where(item => item?.category?.categoryName == categoryName)
            .OrderBy(item => item.itemName)
            .ToList();

        // Create UI for each item
        foreach (var item in categoryItems)
        {
            GameObject go = Instantiate(shopItemUIPrefab, itemContainer);
            if (go.TryGetComponent<ShopItemUI>(out var ui))
            {
                ui.Setup(item, gameManager);

                // Check if player can afford the item
                bool canAfford = CanAffordItem(item);
                ui.SetInteractable(canAfford);
            }
        }
    }

    private bool CanAffordItem(ShopItemSO item)
    {
        if (gameManager == null || item == null) return false;

        var resourceManager = gameManager.GetResourceManager();
        foreach (var cost in item.cost)
        {
            if (resourceManager.GetResourceAmount(cost.type) < cost.amount)
            {
                return false;
            }
        }
        return true;
    }

    // Call this when resources change to update button interactability
    public void RefreshAffordability()
    {
        if (itemContainer == null) return;

        foreach (Transform child in itemContainer)
        {
            if (child.TryGetComponent<ShopItemUI>(out var ui))
            {
                ui.SetInteractable(CanAffordItem(ui.GetItemData()));
            }
        }
    }
}