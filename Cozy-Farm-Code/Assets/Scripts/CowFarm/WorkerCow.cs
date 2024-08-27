using UnityEngine;

public class WorkerCow : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 direction;
    private float minX, maxX, minY, maxY;
    private GameManager gameManager;
    private Animator animator;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        UpdateBoundaries();
        ChangeDirection();
        InvokeRepeating("ChangeDirection", 2f, 2f);
    }

    void Update()
    {
        UpdateBoundaries();
        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;
    }

    void ChangeDirection()
    {
        // Choose a random direction (up, down, left, right)
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                direction = Vector2.up;
                animator.Play("WorkerUp");
                break;
            case 1:
                direction = Vector2.down;
                animator.Play("WorkerDown");
                break;
            case 2:
                direction = Vector2.left;
                animator.Play("WorkerLeft");
                break;
            case 3:
                direction = Vector2.right;
                animator.Play("WorkerRight");
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Milk"))
        {
            gameManager.CollectMilk(other.gameObject); // Collect milk
        }
    }

    public void UpdateBoundaries()
    {
        minX = GameObject.Find("LeftBoundary").transform.position.x + 0.5f;
        maxX = GameObject.Find("RightBoundary").transform.position.x - 0.5f;
        minY = GameObject.Find("BottomBoundary").transform.position.y + 0.5f;
        maxY = GameObject.Find("TopBoundary").transform.position.y - 0.5f;
    }
}
