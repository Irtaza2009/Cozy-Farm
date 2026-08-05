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
        AudioManager.Instance?.PlayClick();
        Collect();
    }

    public void Collect()
    {
        if (collected) return;
        collected = true;

        GameManager.Instance.AddResource(resourceType, amount);

        if(resourceType == FarmResourceType.Egg && TutorialManager.Instance.CurrentStep == TutorialStep.CollectEgg)
        {
            TutorialManager.Instance.GoToStep(TutorialStep.OpenShop);
        }

        Destroy(gameObject);
    }
}
