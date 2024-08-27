using UnityEngine;
using TMPro;

public class IslandManager : MonoBehaviour
{
    public GameObject islandTilePrefab;
    public int tileCost = 100;
    private int islandCount = 1;

    public TextMeshProUGUI islandText;

    private GameManager gameManager;
    private Camera mainCamera;
    private float tileWidth;
    private float tileHeight;

    private float minZoom = 5f;
    private float maxZoom = 20f;
    private float zoomSpeed = 5f;

    private Vector3 dragOrigin;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        mainCamera = Camera.main;

        // Get the size of the prefab
        tileWidth = islandTilePrefab.GetComponent<Renderer>().bounds.size.x;
        tileHeight = islandTilePrefab.GetComponent<Renderer>().bounds.size.y;

        UpdateCameraSize();
    }

    void Update()
    {
        HandleZoom();
        HandlePanning();
    }

    public void BuyIslandTile()
    {
        if (gameManager.coinCount >= tileCost)
        {
            gameManager.coinCount -= tileCost;
            tileCost += 50;
            islandText.text = "Buy Island <br> Cost: " + tileCost;

            Vector3 newTilePosition = CalculateNewTilePosition();
            Instantiate(islandTilePrefab, newTilePosition, Quaternion.identity);
            islandCount++;
            UpdateBoundaries();
            UpdateCameraSize();
        }
    }

    Vector3 CalculateNewTilePosition()
    {
        // Calculate row and column for the new tile
        int row = islandCount / 2;
        int col = islandCount % 2;

        // Calculate the position based on the tile width and height
        Vector3 newPosition = new Vector3(col * tileWidth, -row * tileHeight, 0);
        return newPosition;
    }

    void UpdateBoundaries()
    {
        // Update the boundaries based on the number of island tiles
        GameObject rightBoundary = GameObject.Find("RightBoundary");
        GameObject topBoundary = GameObject.Find("TopBoundary");
        GameObject bottomBoundary = GameObject.Find("BottomBoundary");
        GameObject leftBoundary = GameObject.Find("LeftBoundary");

        int row = islandCount / 2;
        int col = islandCount % 2;

        bool spawnBelowFirstIsland = (islandCount > 2) && (row == 1 && col == 0);

        if (rightBoundary != null)
        {
            rightBoundary.transform.position = new Vector3((islandCount / 2) * tileWidth + tileWidth / 2, rightBoundary.transform.position.y, rightBoundary.transform.position.z);
        }

        if (topBoundary != null)
        {
            // Instantiate a new top boundary and position it correctly
            Vector3 newTopBoundaryPosition = new Vector3((islandCount / 2) * tileWidth, topBoundary.transform.position.y, topBoundary.transform.position.z);
            GameObject newTopBoundary = Instantiate(topBoundary, newTopBoundaryPosition, Quaternion.identity);
            newTopBoundary.name = "TopBoundary_" + islandCount;
        }

        if (bottomBoundary != null)
        {
            if (spawnBelowFirstIsland)
            {
                // Move the old bottom boundary
                bottomBoundary.transform.position = new Vector3(bottomBoundary.transform.position.x, -islandCount * tileHeight, bottomBoundary.transform.position.z);

                // Instantiate new left and right boundaries
                Vector3 newLeftBoundaryPosition = new Vector3(-tileWidth / 2, -row * tileHeight, 0);
                GameObject newLeftBoundary = Instantiate(leftBoundary, newLeftBoundaryPosition, Quaternion.identity);
                newLeftBoundary.name = "LeftBoundary_" + islandCount;

                Vector3 newRightBoundaryPosition = new Vector3(tileWidth / 2, -row * tileHeight, 0);
                GameObject newRightBoundary = Instantiate(rightBoundary, newRightBoundaryPosition, Quaternion.identity);
                newRightBoundary.name = "RightBoundary_" + islandCount;
            }
            else
            {
                // Instantiate a new bottom boundary and position it correctly
                Vector3 newBottomBoundaryPosition = new Vector3((islandCount / 2) * tileWidth, bottomBoundary.transform.position.y, bottomBoundary.transform.position.z);
                GameObject newBottomBoundary = Instantiate(bottomBoundary, newBottomBoundaryPosition, Quaternion.identity);
                newBottomBoundary.name = "BottomBoundary_" + islandCount;
            }
        }

        // Find all chicks and update their boundaries
        ChickMovement[] chicks = FindObjectsOfType<ChickMovement>();
        foreach (ChickMovement chick in chicks)
        {
            chick.UpdateBoundaries();
        }
    }

    void UpdateCameraSize()
    {
        // Calculate the new camera size based on the number of island tiles
        float newCameraSize = (islandCount * tileWidth) / 2f;
        mainCamera.orthographicSize = Mathf.Max(newCameraSize, mainCamera.orthographicSize);

        // Adjust camera position to keep the islands centered
        mainCamera.transform.position = new Vector3((islandCount * tileWidth - tileWidth) / 2f, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }

    void HandleZoom()
    {
        float scrollData;
#if UNITY_EDITOR
        scrollData = Input.GetAxis("Mouse ScrollWheel");
#else
        scrollData = Input.GetTouch(0).deltaPosition.y * 0.01f;
#endif
        mainCamera.orthographicSize -= scrollData * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }

    void HandlePanning()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 difference = dragOrigin - Input.mousePosition;
        dragOrigin = Input.mousePosition;

        Vector3 newPosition = mainCamera.transform.position + difference * 0.01f;
        newPosition.x = Mathf.Clamp(newPosition.x, -tileWidth, (islandCount / 2) * tileWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -islandCount * tileHeight, 0);

        mainCamera.transform.position = newPosition;
    }
}
