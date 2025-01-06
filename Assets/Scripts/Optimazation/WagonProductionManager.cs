using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonProductionManager : MonoBehaviour
{
    private List<IWagonProduction> activeWagons = new List<IWagonProduction>();
    private List<IWagonProduction> inactiveWagons = new List<IWagonProduction>();
    private Dictionary<IWagonProduction, float> productionTimers = new Dictionary<IWagonProduction, float>();
    
    private void Start()
    {
        StartCoroutine(ProductionCoroutine());
        StartCoroutine(CheckInactiveWagonsCoroutine());
    }
    
    public void RegisterWagon(IWagonProduction wagon)
    {
        if (wagon.CanProduce())
        {
            activeWagons.Add(wagon);
        }
        else
        {
            inactiveWagons.Add(wagon);
        }
        productionTimers[wagon] = 0f;
    }
    
    public void UnregisterWagon(IWagonProduction wagon)
    {
        activeWagons.Remove(wagon);
        inactiveWagons.Remove(wagon);
        productionTimers.Remove(wagon);
    }
    
    // Main production loop with no condition checking
    private IEnumerator ProductionCoroutine()
    {
        while (true)
        {
            for (int i = activeWagons.Count - 1; i >= 0; i--)
            {
                var wagon = activeWagons[i];
                productionTimers[wagon] += Time.deltaTime;
                
                if (productionTimers[wagon] >= wagon.GetProductionInterval())
                {
                    wagon.ProcessProduction(productionTimers[wagon]);
                    productionTimers[wagon] = 0f;
                    
                    // If wagon becomes inactive after production
                    if (!wagon.CanProduce())
                    {
                        activeWagons.RemoveAt(i);
                        inactiveWagons.Add(wagon);
                    }
                }
            }
            yield return null;
        }
    }
    
    // Separate coroutine to check inactive wagons less frequently
    private IEnumerator CheckInactiveWagonsCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f); // Check every second
        
        while (true)
        {
            for (int i = inactiveWagons.Count - 1; i >= 0; i--)
            {
                var wagon = inactiveWagons[i];
                if (wagon.CanProduce())
                {
                    inactiveWagons.RemoveAt(i);
                    activeWagons.Add(wagon);
                    productionTimers[wagon] = 0f;
                }
            }
            yield return wait;
        }
    }
}