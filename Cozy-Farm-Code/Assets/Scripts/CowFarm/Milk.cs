using UnityEngine;

public class Milk : MonoBehaviour
{
    private GameManagerCowFarm gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManagerCowFarm>();
    }

    void OnMouseDown()
    {
        gameManager.CollectMilk(this.gameObject);
    }
}
