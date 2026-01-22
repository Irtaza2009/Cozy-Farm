using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private ChickenManager chickenManager;

    private bool isShopOpen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        shopPanel.SetActive(false);
        isShopOpen = false;
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);

        if (isShopOpen && chickenManager != null)
        {
            chickenManager.ForceRefreshUI();
        }
    }

    public void CloseShop()
    {
        isShopOpen = false;
        shopPanel.SetActive(false);
    }

    public void OpenShop()
    {
        isShopOpen = true;
        shopPanel.SetActive(true);
    }
}
