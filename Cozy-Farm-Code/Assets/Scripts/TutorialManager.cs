using UnityEngine;
using TMPro;

public enum TutorialStep
{
    None,
    CollectEgg,
    OpenShop,
    SellEggs,
    BuyChicken,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private GameObject pointer;
    [SerializeField] private TMPro.TextMeshProUGUI hintText;

    private TutorialStep currentStep;
    private const string PlayerPrefKey = "Tutorial_Completed";

    void Awake()
    {
        Instance = this;

        if (hintText != null) hintText.gameObject.SetActive(false);
        if (pointer != null) pointer.SetActive(false);
    }

    void Start()
    {
        if (PlayerPrefs.GetInt(PlayerPrefKey, 0) == 1)
        {
            currentStep = TutorialStep.Complete;
            if (hintText != null) hintText.gameObject.SetActive(false);
            if (pointer != null) pointer.SetActive(false);
            this.enabled = false;
            return;
        }

        StartTutorial();
    }

    void StartTutorial()
    {
        GoToStep(TutorialStep.CollectEgg);
    }

    public void GoToStep(TutorialStep step)
    {
        currentStep = step;
        
        switch (step)
        {
            case TutorialStep.CollectEgg:
                hintText.text ="Tap on an egg to collect it.";
                HidePointer();
                break;
            case TutorialStep.OpenShop:
                hintText.text = "Open the shop to sell your eggs.";
                if (UIManager.Instance != null && UIManager.Instance.ShopButton != null)
                {
                    PointTo(UIManager.Instance.ShopButton.transform);
                }
                else
                {
                    HidePointer();
                }
                break;
            case TutorialStep.SellEggs:
                hintText.text = "Sell your eggs to earn coins.";
                if (UIManager.Instance != null && UIManager.Instance.SellButton != null)
                {
                    PointTo(UIManager.Instance.SellButton.transform, Vector3.right * 400f);
                }
                else
                {
                    HidePointer();
                }
                break;
            case TutorialStep.BuyChicken:
                hintText.text = "Buy a new chicken to increase egg production.";
                if (UIManager.Instance != null && UIManager.Instance.BuyChickenButton != null)
                {
                    PointTo(UIManager.Instance.BuyChickenButton.transform, Vector3.right * 400f);
                }
                else
                {
                    HidePointer();
                }
                break;
            case TutorialStep.Complete:
                hintText.gameObject.SetActive(false);
                pointer.SetActive(false);

                PlayerPrefs.SetInt(PlayerPrefKey, 1);
                PlayerPrefs.Save();
                // Disable the component after completion so it doesn't run anymore.
                this.enabled = false;
                break;
        }
    }

    void PointTo(Transform target)
    {
        PointTo(target, Vector3.right * 200f);
    }

    void PointTo(Transform target, Vector3 offset)
    {
        pointer.SetActive(true);
        pointer.transform.position = target.position + offset;
    }

    void HidePointer()
    {
        pointer.SetActive(false);
    }

    public TutorialStep CurrentStep => currentStep;
}
