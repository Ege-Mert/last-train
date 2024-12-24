using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WagonManagementUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform containerPanel;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button assignWorkerButton;
    [SerializeField] private Button deassignWorkerButton;
    [SerializeField] private Button destroyButton;
    
    private Wagon currentWagon;
    private GameManager gameManager;

    private void Start()
    {
        Debug.Log("WagonManagementUI Started");
        
        // Get GameManager reference
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }
        
        SetupButtonListeners();
        
        // Get the wagon reference if we don't have it
        if (currentWagon == null)
        {
            currentWagon = transform.parent.parent.GetComponent<Wagon>();
            if (currentWagon != null)
            {
                Debug.Log($"Found wagon: {currentWagon.name}");
            }
            else
            {
                Debug.LogError("No wagon found on parent!");
            }
        }
    }
    
    private void SetupButtonListeners()
    {
        Debug.Log("Setting up button listeners...");

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(() => {
                Debug.Log("Upgrade button clicked!");
                OnUpgradeClick();
            });
        }
        else Debug.LogError("Upgrade button not assigned!");

        if (assignWorkerButton != null)
        {
            assignWorkerButton.onClick.AddListener(() => {
                Debug.Log("Assign worker button clicked!");
                OnAssignWorkerClick();
            });
        }
        else Debug.LogError("Assign worker button not assigned!");

        if (deassignWorkerButton != null)
        {
            deassignWorkerButton.onClick.AddListener(() => {
                Debug.Log("Deassign worker button clicked!");
                OnDeassignWorkerClick();
            });
        }
        else Debug.LogError("Deassign worker button not assigned!");

        if (destroyButton != null)
        {
            destroyButton.onClick.AddListener(() => {
                Debug.Log("Destroy button clicked!");
                OnDestroyClick();
            });
        }
        else Debug.LogError("Destroy button not assigned!");
    }
        private void OnUpgradeClick()
    {
        if (currentWagon == null)
        {
            Debug.LogError("No wagon selected for upgrade!");
            return;
        }

        try 
        {
            var upgradeComponent = currentWagon.GetComponent<UpgradeComponent>();
            Debug.Log($"UpgradeComponent found: {upgradeComponent != null}");
            
            if (upgradeComponent != null && upgradeComponent.TryUpgrade())
            {
                Debug.Log($"Successfully upgraded wagon: {currentWagon.name}");
            }
            else
            {
                Debug.LogWarning($"Failed to upgrade wagon: {currentWagon.name}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error upgrading wagon: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    private void OnAssignWorkerClick()
    {
        if (currentWagon == null || gameManager == null)
        {
            Debug.LogError("No wagon selected or GameManager not found!");
            return;
        }

        try 
        {
            var workerComponent = currentWagon.GetComponent<WorkerComponent>();
            Debug.Log($"WorkerComponent found: {workerComponent != null}");
            
            if (workerComponent != null)
            {
                var humanManager = gameManager.GetCentralHumanManager();
                if (humanManager == null)
                {
                    Debug.LogError("Failed to get HumanManager from GameManager!");
                    return;
                }

                Debug.Log($"Current workers: {workerComponent.GetCurrentWorkers()}, Max workers: {workerComponent.GetMaxWorkers()}");
                Debug.Log($"Available humans: {humanManager.GetAvailableHumans()}");
                
                if (workerComponent.AddWorkers(1))
                {
                    Debug.Log($"Successfully assigned worker to wagon: {currentWagon.name}");
                }
                else
                {
                    Debug.LogWarning($"Failed to assign worker. Check max workers limit or available humans.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error assigning worker: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    private void OnDeassignWorkerClick()
    {
        if (currentWagon == null || gameManager == null)
        {
            Debug.LogError("No wagon selected or GameManager not found!");
            return;
        }

        try 
        {
            var workerComponent = currentWagon.GetComponent<WorkerComponent>();
            Debug.Log($"WorkerComponent found: {workerComponent != null}");
            
            if (workerComponent != null)
            {
                if (workerComponent.RemoveWorkers(1))
                {
                    Debug.Log($"Successfully removed worker from wagon: {currentWagon.name}");
                }
                else
                {
                    Debug.LogWarning($"Failed to remove worker from wagon: {currentWagon.name}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error removing worker: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    private void OnDestroyClick()
    {
        if (currentWagon == null || gameManager == null)
        {
            Debug.LogError("No wagon selected or GameManager not found!");
            return;
        }

        try 
        {
            var wagonManager = gameManager.GetWagonManager();
            Debug.Log($"WagonManager found: {wagonManager != null}");
            
            if (wagonManager != null)
            {
                wagonManager.DestroyWagon(currentWagon);
                Debug.Log($"Successfully destroyed wagon: {currentWagon.name}");
            }
            else
            {
                Debug.LogError("Failed to get WagonManager from GameManager!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error destroying wagon: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    // Method to manually set the wagon (can be called from inspector or other scripts)
    public void SetWagon(Wagon wagon)
    {
        currentWagon = wagon;
        Debug.Log($"Manually set wagon to: {wagon?.name ?? "null"}");
    }
}