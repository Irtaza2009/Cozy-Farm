using UnityEngine;
using TMPro;
using System.Collections;

public class ResourceUIText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private FarmResourceType resourceType;
    [SerializeField] private string prefix = "Eggs: ";

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
        if (type != resourceType) return;
        UpdateText(newAmount);
    }

    void UpdateText(int amount)
    {
        text.text = prefix + amount;
    }
}
