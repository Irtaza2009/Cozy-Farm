using UnityEngine;

public class Coin : MonoBehaviour
{

    public GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    void OnMouseDown()
    {
        // Add code to increment the player's coin count
         gameManager.AddCoin();
        Destroy(gameObject);
        
    }
}
