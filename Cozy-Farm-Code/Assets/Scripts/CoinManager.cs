using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    [Header("Sell Eggs")] 
    [SerializeField] private int coinsPerEgg = 2;
    [SerializeField] private Button sellButton;
    [SerializeField] private TextMeshProUGUI getText;
    [SerializeField] private string getPrefix = "Get: ";

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

    public void SellEggs()
    {
        if (!GameManager.HasInstance) return;

        int eggs = GameManager.Instance.GetResource(FarmResourceType.Egg);
        if (eggs <= 0) return;

        int coinsToAdd = eggs * coinsPerEgg;
        GameManager.Instance.AddResource(FarmResourceType.Egg, -eggs);
        GameManager.Instance.AddResource(FarmResourceType.Coin, coinsToAdd);

        if(TutorialManager.Instance.CurrentStep == TutorialStep.SellEggs)
        {
            TutorialManager.Instance.GoToStep(TutorialStep.BuyChicken);
        }

        RefreshUI();
    }

    private void OnResourceChanged(FarmResourceType type, int amount)
    {
        if (type == FarmResourceType.Egg)
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        if (!GameManager.HasInstance)
        {
            if (sellButton != null) sellButton.interactable = false;
            if (getText != null) getText.text = getPrefix + "0";
            return;
        }

        int eggs = GameManager.Instance.GetResource(FarmResourceType.Egg);
        int potentialCoins = eggs * coinsPerEgg;

        if (getText != null)
        {
            getText.text = getPrefix + potentialCoins;
        }

        if (sellButton != null)
        {
            sellButton.interactable = eggs > 0;
        }
    }
}
