using UnityEngine;

public class Egg : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnMouseDown()
    {
        gameManager.CollectEgg(this.gameObject);
    }
}
