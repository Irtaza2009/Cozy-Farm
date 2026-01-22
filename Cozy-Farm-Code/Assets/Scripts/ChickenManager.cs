using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChickenManager : MonoBehaviour
{
    [Header("Chicken")]
    [SerializeField] private GameObject chickenPrefab;
    [SerializeField] private GameObject nestEggPrefab;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-2f, -2f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(2f, 2f);

    [Header("Cost Settings")]
    [SerializeField] private int baseCost = 50;
    [SerializeField] private int costIncrease = 25;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private string costPrefix = "Cost: ";
    [SerializeField] private Button buyButton;

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

        Vector3 spawnPos = GetRandomSpawnPosition();

        // Spawn nest egg; it will hatch into a chicken after a delay.
        var nest = Instantiate(nestEggPrefab, spawnPos, Quaternion.identity);
        var nestEgg = nest != null ? nest.GetComponent<NestEgg>() : null;
        if (nestEgg != null)
        {
            nestEgg.SetChickenPrefab(chickenPrefab);
        }
        else
        {
            Debug.LogWarning("Nest egg prefab missing NestEgg component; spawning chicken immediately as fallback.", nest);
            Instantiate(chickenPrefab, spawnPos, Quaternion.identity);
            GameManager.Instance.AddResource(FarmResourceType.Hen, 1);
        }

        RefreshUI();

        int totalHens = GameManager.Instance.GetResource(FarmResourceType.Hen);
        Debug.Log("Nest placed. Current hens: " + totalHens);
    }

    private void OnResourceChanged(FarmResourceType type, int amount)
    {
        if (type == FarmResourceType.Coin || type == FarmResourceType.Hen)
        {
            RefreshUI();
        }
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
        int currentHens = GameManager.Instance != null
            ? GameManager.Instance.GetResource(FarmResourceType.Hen)
            : 0;
        return baseCost + currentHens * costIncrease;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector3(x, y, 0f);
    }
}
