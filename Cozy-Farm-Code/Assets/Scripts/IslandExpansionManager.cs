using UnityEngine;

public class IslandExpansionManager : MonoBehaviour
{
    [Header("Fence Sections (in order)")]
    [SerializeField] private GameObject[] fenceDividers;

    [Header("Cost Settings")]
    [SerializeField] private int baseCost = 50;
    [SerializeField] private int costIncrease = 50;

    private int currentLevel = 0;

    public void ExpandIsland()
    {
        Debug.Log("Attempting to expand island...");
        if (currentLevel >= fenceDividers.Length)
            return;

        int cost = baseCost + currentLevel * costIncrease;

        if (GameManager.Instance.GetResource(FarmResourceType.Coin) < cost)
        {            
            Debug.Log("Not enough coins to expand island. Coins: " + GameManager.Instance.GetResource(FarmResourceType.Coin));
            return;
        }


        GameManager.Instance.AddResource(FarmResourceType.Coin, -cost);

        fenceDividers[currentLevel].SetActive(false);
        currentLevel++;

        Debug.Log("Island expanded to level " + currentLevel);
    }
}
