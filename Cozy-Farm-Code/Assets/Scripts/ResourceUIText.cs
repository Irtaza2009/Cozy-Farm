using UnityEngine;
using TMPro;
using System.Collections;

public class ResourceUIText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private FarmResourceType resourceType;
    [SerializeField] private string prefix = "Eggs: ";
    [SerializeField] private bool showHenCapacity = false; // when true and resourceType == Hen, display current/max

    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        StartCoroutine(WaitForGameManager());
    }

    IEnumerator WaitForGameManager()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        GameManager.Instance.OnResourceChanged += OnResourceChanged;
        UpdateText(GameManager.Instance.GetResource(resourceType));
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    void OnResourceChanged(FarmResourceType type, int newAmount)
    {
        if (type == resourceType || (showHenCapacity && resourceType == FarmResourceType.Hen && type == FarmResourceType.Island))
        {
            UpdateText(GameManager.Instance.GetResource(resourceType));
        }
    }

    void UpdateText(int amount)
    {
        if (showHenCapacity && resourceType == FarmResourceType.Hen)
        {
            int islands = GameManager.Instance != null ? GameManager.Instance.GetResource(FarmResourceType.Island) : 0;
            int capacity = islands * 5;
            text.text = prefix + amount + "/" + capacity;
        }
        else
        {
            text.text = prefix + amount;
        }
    }
}
