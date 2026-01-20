using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChickenManager : MonoBehaviour
{
    [Header("Chicken")]
    [SerializeField] private GameObject chickenPrefab;
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;

    [Header("Cost Settings")]
    [SerializeField] private int baseCost = 50;
    [SerializeField] private int costIncrease = 25;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private string costPrefix = "Cost: ";
    [SerializeField] private Button buyButton;

    private int chickenCount = 0;

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

    public void BuyChicken()
    {
        Debug.Log("Attempting to buy chicken...");

        int cost = GetNextCost();
        int coins = GameManager.Instance.GetResource(FarmResourceType.Coin);

        if (coins < cost)
        {
            Debug.Log("Not enough coins to buy chicken. Coins: " + coins);
            return;
        }

        GameManager.Instance.AddResource(FarmResourceType.Coin, -cost);

        Instantiate(chickenPrefab, spawnPosition, Quaternion.identity);
        chickenCount++;

        RefreshUI();

        Debug.Log("Chicken bought. Total chickens: " + chickenCount);
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
            costText.text = costPrefix + GetNextCost();
        }

        if (buyButton != null)
        {
            int coins = GameManager.Instance.GetResource(FarmResourceType.Coin);
            buyButton.interactable = coins >= GetNextCost();
        }
    }

    private int GetNextCost()
    {
        return baseCost + chickenCount * costIncrease;
    }
}
