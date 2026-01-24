using UnityEngine;
using System.Collections;

public class NestEgg : MonoBehaviour
{
    [Header("Hatching")]
    [SerializeField] private GameObject chickenPrefab;
    [SerializeField] private float hatchDelay = 10f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    void Start()
    {
        StartCoroutine(HatchRoutine());
    }

    public void SetChickenPrefab(GameObject prefab)
    {
        chickenPrefab = prefab;
    }

    private IEnumerator HatchRoutine()
    {
        yield return new WaitForSeconds(hatchDelay);
        AudioManager.Instance?.PlayEggCrack();

         yield return new WaitForSeconds(2f); // wait for cracking sound

        if (chickenPrefab == null)
        {
            Debug.LogWarning("NestEgg has no chicken prefab assigned; skipping hatch.", this);
            Destroy(gameObject);
            yield break;
        }

        Instantiate(chickenPrefab, transform.position + spawnOffset, Quaternion.identity);
        
        Destroy(gameObject);
    }
}
