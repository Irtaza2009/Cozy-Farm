using UnityEngine;

public class ChickMovement : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 direction;
    public GameObject eggPrefab;
    public float layEggInterval = 3f;

    private float minX, maxX, minY, maxY;

    void Start()
    {
        UpdateBoundaries();
        ChangeDirection();
        InvokeRepeating("ChangeDirection", 2f, 2f);
        InvokeRepeating("LayEgg", layEggInterval, layEggInterval);
    }

    void Update()
    {
        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime;

        // Clamp the new position within the boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
    }

    void ChangeDirection()
    {
        float angle = Random.Range(0, 360);
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void LayEgg()
    {
        Instantiate(eggPrefab, transform.position, Quaternion.identity);
    }

    public void UpdateBoundaries()
    {
        minX = GameObject.Find("LeftBoundary").transform.position.x + 0.5f;
        maxX = GameObject.Find("RightBoundary").transform.position.x - 0.5f;
        minY = GameObject.Find("BottomBoundary").transform.position.y + 0.5f;
        maxY = GameObject.Find("TopBoundary").transform.position.y - 0.5f;
    }
}
