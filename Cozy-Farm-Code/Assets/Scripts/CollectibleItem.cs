using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private FarmResourceType resourceType;
    [SerializeField] private int amount = 1;

    private bool collected = false;

    void OnMouseDown()
    {
        Collect();
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        GameManager.Instance.AddResource(resourceType, amount);

        Destroy(gameObject);
    }
}
