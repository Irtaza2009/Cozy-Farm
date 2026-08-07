using UnityEngine;
using System;

[
RequireComponent(typeof(Camera))
]
public class CameraZoom : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private GameObject zoomHint;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float noExpansionMaxZoom = 5f;
    [SerializeField] private float maxZoom = 8f;
    [SerializeField] private float scrollZoomSpeed = 1f;
    [SerializeField] private float pinchZoomSpeed = 0.01f;

    public event Action<float> OnZoomChanged;

    private Camera cam;
    private float effectiveMaxZoom;
    private System.Action<GameManager> readyHandler;

    void Awake()
    {
        cam = GetComponent<Camera>();
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
        HandleZoomInput();
    }

    private void OnGameManagerReady(GameManager gm)
    {
        gm.OnResourceChanged += OnResourceChanged;
        RecalculateEffectiveZoomRange();
        ApplyZoom(cam.orthographicSize, false);
    }

    private void OnResourceChanged(FarmResourceType type, int value)
    {
        if (type == FarmResourceType.Island)
        {
            RecalculateEffectiveZoomRange();
            ApplyZoom(cam.orthographicSize, false);
        }
    }

    private void HandleZoomInput()
    {
        if (!enableZoom || cam == null || !cam.orthographic)
        {
            return;
        }

        float zoomDelta = 0f;

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 previousTouch0Position = touch0.position - touch0.deltaPosition;
            Vector2 previousTouch1Position = touch1.position - touch1.deltaPosition;

            float previousDistance = Vector2.Distance(previousTouch0Position, previousTouch1Position);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            zoomDelta = (currentDistance - previousDistance) * pinchZoomSpeed;
        }
        else
        {
            zoomDelta = Input.mouseScrollDelta.y * scrollZoomSpeed;
        }

        if (Mathf.Abs(zoomDelta) <= Mathf.Epsilon)
        {
            return;
        }

        ApplyZoom(cam.orthographicSize - zoomDelta, true);
    }

    private void ApplyZoom(float targetZoom, bool notifyListeners)
    {
        if (cam == null || !cam.orthographic)
        {
            return;
        }

        float clampedZoom = Mathf.Clamp(targetZoom, minZoom, effectiveMaxZoom);
        if (Mathf.Approximately(cam.orthographicSize, clampedZoom))
        {
            return;
        }

        cam.orthographicSize = clampedZoom;

        if (notifyListeners)
        {
            if (zoomHint != null)
            {
                zoomHint.SetActive(false);
            }

            OnZoomChanged?.Invoke(cam.orthographicSize);
        }
    }

    private void RecalculateEffectiveZoomRange()
    {
        int islands = GameManager.Instance != null
            ? GameManager.Instance.GetResource(FarmResourceType.Island)
            : 1;

        effectiveMaxZoom = islands <= 1 ? noExpansionMaxZoom : maxZoom;
    }
}
