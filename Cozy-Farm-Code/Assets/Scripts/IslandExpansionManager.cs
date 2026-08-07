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
    [SerializeField] private ChickenManager chickenManager;

    private int currentLevel = 0;
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
        SyncFromResource();
        if (chickenManager != null)
        {
            chickenManager.SyncSpawnAreaForIslandLevel(currentLevel + 1);
        }
        gm.OnResourceChanged += OnResourceChanged;
        RefreshUI();
    }

    public void ExpandIsland()
    {
        if (currentLevel >= fenceDividers.Length)
            return;

        int cost = GetNextCost();

        if (GameManager.Instance.GetResource(FarmResourceType.Coin) < cost)
            return;

        GameManager.Instance.AddResource(FarmResourceType.Coin, -cost);
        GameManager.Instance.AddResource(FarmResourceType.Island, 1);

        if (chickenManager != null)
            chickenManager.ForceRefreshUI();
    }

    private void OnResourceChanged(FarmResourceType type, int amount)
    {
        if (type != FarmResourceType.Coin && type != FarmResourceType.Island) return;

        if (type == FarmResourceType.Island)
        {
            SyncFromResource();
            if (chickenManager != null)
            {
                chickenManager.SyncSpawnAreaForIslandLevel(currentLevel + 1);
            }
        }

        RefreshUI();
    }

    private void SyncFromResource()
    {
        int islandLevel = Mathf.Clamp(
            GameManager.Instance.GetResource(FarmResourceType.Island),
            1,
            fenceDividers.Length + 1
        );

        // Number of removed fences
        currentLevel = islandLevel - 1;

        for (int i = 0; i < fenceDividers.Length; i++)
        {
            // i = 0 → removed when islandLevel >= 2
            bool unlocked = islandLevel >= i + 2;
            fenceDividers[i].SetActive(!unlocked);
        }
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
