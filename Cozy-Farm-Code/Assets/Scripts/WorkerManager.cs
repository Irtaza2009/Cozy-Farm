using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkerManager : MonoBehaviour
{
    [Header("Worker")]
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool useSpawnArea;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;

    [Header("Cost")]
    [SerializeField] private int baseCost = 100;
    [SerializeField] private int costIncrease = 50;

    [Header("UI")]
    [SerializeField] private Button hireButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private string costPrefix = "Cost: ";

    private int hiredWorkers;
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
        RefreshUI();
    }

    public void HireWorker()
    {
        if (!GameManager.HasInstance || workerPrefab == null)
        {
            return;
        }

        int cost = GetNextCost();
        if (GameManager.Instance.GetResource(FarmResourceType.Coin) < cost)
        {
            return;
        }

        GameManager.Instance.AddResource(FarmResourceType.Coin, -cost);
        Instantiate(workerPrefab, GetSpawnPosition(), Quaternion.identity);
        hiredWorkers++;
        RefreshUI();
    }

    private void OnResourceChanged(FarmResourceType type, int amount)
    {
        if (type == FarmResourceType.Coin)
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        int cost = GetNextCost();

        if (costText != null)
        {
            costText.text = costPrefix + cost;
        }

        if (hireButton != null)
        {
            bool canAfford = GameManager.HasInstance &&
                GameManager.Instance.GetResource(FarmResourceType.Coin) >= cost;
            hireButton.interactable = canAfford && workerPrefab != null;
        }
    }

    private int GetNextCost()
    {
        return baseCost + hiredWorkers * costIncrease;
    }

    private Vector3 GetSpawnPosition()
    {
        if (useSpawnArea)
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            return new Vector3(x, y, 0f);
        }

        return spawnPoint != null ? spawnPoint.position : transform.position;
    }
}
