using UnityEngine;

public class EggHatching : MonoBehaviour
{
    public GameObject chickPrefab;

    private GameManager gameManager;

    public float hatchTime = 10f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        // Start the hatching process
        Invoke("Hatch", hatchTime);
    }

    void Hatch()
{
    // Instantiate a chick at the egg's position
    GameObject chick = Instantiate(chickPrefab, transform.position, Quaternion.identity);

    // Notify the game manager to add the new chick to the list
    gameManager.AddChickToList(chick);

    // Destroy the egg
    Destroy(gameObject);
}

}
