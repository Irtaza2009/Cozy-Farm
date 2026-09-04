using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BoatController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private const string IdleAnimation = "Boat_Idle";
    private const string TiedIdleAnimation = "Boat_Idle_Tied";
    private const string MoveAnimation = "Boat_Move";

    private Rigidbody2D rb;

    private Vector2 moveInput;

    private bool isBoarded;

    private string currentAnimation;

    // The boat sprite faces LEFT by default.
    private Quaternion defaultRotation;
    private Vector3 defaultScale;

    public bool IsBoarded => isBoarded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Whatever rotation the boat has in the scene is considered
        // its default rotation. In your case this should be facing LEFT.
        defaultRotation = transform.localRotation;
        defaultScale = transform.localScale;

        // Make sure the boat starts in its tied state.
        isBoarded = false;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        PlayAnimation(TiedIdleAnimation);
    }

    void Update()
    {
        // --------------------------------------------------
        // BOAT IS NOT BOARDED
        // --------------------------------------------------

        if (!isBoarded)
        {
            moveInput = Vector2.zero;

            // Make absolutely sure the boat doesn't move.
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // Always face its default direction.
            transform.localRotation = defaultRotation;
            transform.localScale = defaultScale;

            // Tied animation ONLY while unboarded.
            PlayAnimation(TiedIdleAnimation);

            return;
        }

        // --------------------------------------------------
        // BOAT IS BOARDED
        // --------------------------------------------------

        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Normalize diagonal movement.
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            UpdateFacing(moveInput);
            PlayAnimation(MoveAnimation);
        }
        else
        {
            // Normal idle while boarded.
            PlayAnimation(IdleAnimation);
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (isBoarded)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetBoarded(bool boarded)
    {
        isBoarded = boarded;
        moveInput = Vector2.zero;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (boarded)
        {
            // The boat has just been boarded.
            // Play normal idle immediately.
            PlayAnimation(IdleAnimation);
        }
        else
        {
            transform.localRotation = defaultRotation;
            transform.localScale = defaultScale;
            PlayAnimation(TiedIdleAnimation);
        }
    }

    private void UpdateFacing(Vector2 direction)
    {
        float angle;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            if (direction.x < 0f)
            {
                // LEFT = default orientation
                angle = 0f;
                transform.localScale = defaultScale;
            }
            else
            {
                // RIGHT = horizontally flipped default orientation
                angle = 0f;
                Vector3 flippedScale = defaultScale;
                flippedScale.x = -defaultScale.x;
                transform.localScale = flippedScale;
            }
        }
        else
        {
            transform.localScale = defaultScale;
            if (direction.y > 0f)
            {
                // UP
                angle = -90f;
            }
            else
            {
                // DOWN
                angle = 90f;
            }
        }

        transform.localRotation =
            defaultRotation * Quaternion.Euler(0f, 0f, angle);
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null)
            return;

        if (currentAnimation == animationName)
            return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }
}