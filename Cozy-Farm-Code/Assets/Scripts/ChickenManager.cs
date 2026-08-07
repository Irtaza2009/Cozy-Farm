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
    [SerializeField] private Vector2 expandedSpawnAreaMin = new Vector2(-4f, -4f);
    [SerializeField] private Vector2 expandedSpawnAreaMax = new Vector2(4f, 4f);

    [Header("Cost Settings")]
    [SerializeField] private int baseCost = 50;
    [SerializeField] private int costIncrease = 25;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private string costPrefix = "Cost: ";
    [SerializeField] private Button buyButton;

    private Vector2 currentSpawnAreaMin;
    private Vector2 currentSpawnAreaMax;

    private System.Action<GameManager> readyHandler;

    void OnEnable()
    {
        readyHandler = OnGameManagerReady;
        GameManager.WhenReady(readyHandler);
    }

    void OnDisable()
    {
        if (readyHandler != null)
        {
            GameManager.OnInstanceReady -= readyHandler;
            readyHandler = null;
        }

        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    private void OnGameManagerReady(GameManager gm)
    {
        gm.OnResourceChanged += OnResourceChanged;
        SyncSpawnAreaForIslandLevel(gm.GetResource(FarmResourceType.Island));
        RefreshUI();
    }

    public void BuyChicken()
    {
        Debug.Log("Attempting to buy chicken...");

        int capacity = GetHenCapacity();
        int currentHens = GameManager.Instance.GetResource(FarmResourceType.Hen);
        if (capacity <= 0 || currentHens >= capacity)
        {
            Debug.Log("Hen capacity reached. Expand island to increase cap.");
            RefreshUI();
            return;
        }

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
        GameManager.Instance.AddResource(FarmResourceType.Hen, 1);
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

        if(TutorialManager.Instance.CurrentStep == TutorialStep.BuyChicken)
        {
            TutorialManager.Instance.GoToStep(TutorialStep.Complete);
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
        int capacity = GetHenCapacity();
        int hens = GameManager.Instance.GetResource(FarmResourceType.Hen);

        if (costText != null)
        {
            if (capacity > 0 && hens >= capacity)
            {
                costText.text = "Max";
            }
            else
            {
                costText.text = costPrefix + GetNextCost();
            }
        }

        if (buyButton != null)
        {
            int coins = GameManager.Instance.GetResource(FarmResourceType.Coin);
            bool hasCapacity = capacity > 0 && hens < capacity;
            buyButton.interactable = hasCapacity && coins >= GetNextCost();
        }
    }

    private int GetNextCost()
    {
        int currentHens = GameManager.Instance != null
            ? GameManager.Instance.GetResource(FarmResourceType.Hen)
            : 0;
        return baseCost + currentHens * costIncrease;
    }

    private int GetHenCapacity()
    {
        if (GameManager.Instance == null) return 0;
        int islands = GameManager.Instance.GetResource(FarmResourceType.Island);
        return islands * 10;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(currentSpawnAreaMin.x, currentSpawnAreaMax.x);
        float y = Random.Range(currentSpawnAreaMin.y, currentSpawnAreaMax.y);
        return new Vector3(x, y, 0f);
    }

    public void SyncSpawnAreaForIslandLevel(int islandLevel)
    {
        if (islandLevel >= 2)
        {
            currentSpawnAreaMin = expandedSpawnAreaMin;
            currentSpawnAreaMax = expandedSpawnAreaMax;
        }
        else
        {
            currentSpawnAreaMin = spawnAreaMin;
            currentSpawnAreaMax = spawnAreaMax;
        }
    }

    public void ForceRefreshUI()
    {
        RefreshUI();
    }
}
