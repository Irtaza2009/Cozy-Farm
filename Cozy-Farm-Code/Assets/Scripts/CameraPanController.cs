using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPanController : MonoBehaviour
{
    [Header("Panning")]
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private bool enablePanning = false;

    [Header("Island Size")]
    [SerializeField] private float islandWidth = 10f;
    [SerializeField] private float islandHeight = 10f;

    private Camera cam;
    private Vector3 lastMouseWorld;
    private Vector3 originPosition;

    private float minX, maxX, minY, maxY;

    private System.Action<GameManager> readyHandler;

    void Awake()
    {
        cam = GetComponent<Camera>();
        originPosition = transform.position; // use scene placement as starting anchor
        RecalculateBounds();
    }

    void OnEnable()
    {
        readyHandler = OnGameManagerReady;
        GameManager.WhenReady(readyHandler);
    }

    void OnDisable()
    {
        if (readyHandler != null)
        {
            GameManager.OnInstanceReady -= readyHandler;
            readyHandler = null;
        }

        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    void Update()
    {
        if (!enablePanning) return;

        if (Input.GetMouseButtonDown(0))
        {
            lastMouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = lastMouseWorld - currentMouseWorld;

            Vector3 targetPos = transform.position + delta * dragSpeed;
            targetPos.z = transform.position.z;

            transform.position = ClampToBounds(targetPos);
            lastMouseWorld = currentMouseWorld;
        }
    }

    private void OnGameManagerReady(GameManager gm)
    {
        gm.OnResourceChanged += OnResourceChanged;
        RecalculateBounds();
    }

    private void OnResourceChanged(FarmResourceType type, int value)
    {
        if (type == FarmResourceType.Island)
        {
            RecalculateBounds();
        }
    }

    private void RecalculateBounds()
    {
        int islands = GameManager.Instance != null
            ? GameManager.Instance.GetResource(FarmResourceType.Island)
            : 1;

        enablePanning = islands > 1;

        // Base island rectangle centered on the scene placement of the camera.
        minX = originPosition.x - islandWidth;
        maxX = originPosition.x + islandWidth;
        minY = originPosition.y - islandHeight;
        maxY = originPosition.y + islandHeight;

        if (islands >= 2)
        {
            // Extend to the right for island 2.
            maxX += islandWidth;
        }

        if (islands >= 3)
        {
            // Extend downward for island 3.
            minY -= islandHeight;
        }

        // Clamp camera immediately
        transform.position = ClampToBounds(transform.position);
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        float camHalfWidth = cam.orthographicSize * cam.aspect;
        float camHalfHeight = cam.orthographicSize;

        pos.x = Mathf.Clamp(pos.x, minX + camHalfWidth, maxX - camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, minY + camHalfHeight, maxY - camHalfHeight);

        return pos;
    }
}
