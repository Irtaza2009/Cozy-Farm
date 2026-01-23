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
        gm.OnResourceChanged += OnResourceChanged;
        RefreshUI();
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
        GameManager.Instance.AddResource(FarmResourceType.Island, 1);

        fenceDividers[currentLevel].SetActive(false);
        currentLevel++;

        RefreshUI();

        Debug.Log("Island expanded to level " + currentLevel);
        
        if (chickenManager != null)
        {
            chickenManager.ForceRefreshUI();
        }
    }

    private void OnResourceChanged(FarmResourceType type, int amount)
    {
        if (type != FarmResourceType.Coin && type != FarmResourceType.Island) return;

        if (type == FarmResourceType.Island)
        {
            SyncFromResource();
        }

        RefreshUI();
    }

    private void SyncFromResource()
    {
        int islandLevel = Mathf.Clamp(GameManager.Instance.GetResource(FarmResourceType.Island), 0, fenceDividers.Length);
        currentLevel = islandLevel - 1; // first level is base island

        // Disable already-opened fence sections based on saved level.
        for (int i = 0; i < fenceDividers.Length; i++)
        {
            bool shouldBeOpen = i < currentLevel;
            if (fenceDividers[i] != null)
            {
                fenceDividers[i].SetActive(!shouldBeOpen);
            }
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
