using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IslandExpansionManager : MonoBehaviour
{
    [Header("Fence Sections (in order)")]
    [SerializeField] private GameObject[] fenceDividers;

    [Header("Cost Settings")]
    [SerializeField] private int baseCost = 500;
    [SerializeField] private int costIncrease = 500;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private string costPrefix = "Cost: ";
    [SerializeField] private Button expandButton;

    private int currentLevel = 0;

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged += OnResourceChanged;
        }

        RefreshUI();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

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

        RefreshUI();

        Debug.Log("Island expanded to level " + currentLevel);
    }

    private void OnResourceChanged(FarmResourceType type, int amount)
    {
        if (type != FarmResourceType.Coin) return;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (costText != null)
        {
            if (currentLevel >= fenceDividers.Length)
            {
                costText.text = "Max";
            }
            else
            {
                costText.text = costPrefix + GetNextCost();
            }
        }

        if (expandButton != null)
        {
            if (currentLevel >= fenceDividers.Length)
            {
                expandButton.interactable = false;
            }
            else
            {
                int coins = GameManager.Instance.GetResource(FarmResourceType.Coin);
                expandButton.interactable = coins >= GetNextCost();
            }
        }
    }

    private int GetNextCost()
    {
        return baseCost + currentLevel * costIncrease;
    }
}
