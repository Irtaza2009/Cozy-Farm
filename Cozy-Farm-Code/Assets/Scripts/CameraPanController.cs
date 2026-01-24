using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPanController : MonoBehaviour
{
    [Header("Panning")]
    [SerializeField] private float dragSpeed = 0.01f;
    [SerializeField] private float smoothTime = 0.08f;
    [SerializeField] private bool enablePanning = true;

    [Header("Island Size")]
    [SerializeField] private float islandWidth = 10f;
    [SerializeField] private float islandHeight = 10f;

    private Camera cam;

    private Vector3 dragStartMouse;
    private Vector3 dragStartCam;
    private Vector3 velocity;

    private float minX, maxX, minY, maxY;

    private System.Action<GameManager> readyHandler;

    void Awake()
    {
        cam = GetComponent<Camera>();
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
            dragStartMouse = Input.mousePosition;
            dragStartCam = transform.position;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - dragStartMouse;

            // Convert pixels → world units
            float worldPerPixel = cam.orthographicSize * 2f / Screen.height;

            Vector3 target = dragStartCam
                - new Vector3(mouseDelta.x * worldPerPixel, mouseDelta.y * worldPerPixel, 0f)
                * dragSpeed;

            target = ClampToBounds(target);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                target,
                ref velocity,
                smoothTime
            );
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

        //enablePanning = islands > 1;

        float baseX = transform.position.x;
        float baseY = transform.position.y;

        minX = baseX - islandWidth;
        maxX = baseX + islandWidth;
        minY = baseY - islandHeight;
        maxY = baseY + islandHeight;

        if (islands >= 2)
            maxX += islandWidth;

        if (islands >= 3)
            minY -= islandHeight;

        transform.position = ClampToBounds(transform.position);
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        float halfW = cam.orthographicSize * cam.aspect;
        float halfH = cam.orthographicSize;

        pos.x = Mathf.Clamp(pos.x, minX + halfW, maxX - halfW);
        pos.y = Mathf.Clamp(pos.y, minY + halfH, maxY - halfH);

        return pos;
    }
}
