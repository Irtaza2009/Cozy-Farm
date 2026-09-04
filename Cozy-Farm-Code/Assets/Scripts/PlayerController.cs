using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private const string AnimIdleDown = "Worker_Idle_Down";
    private const string AnimIdleUp = "Worker_Idle_Up";
    private const string AnimIdleLeft = "Worker_Idle_Left";
    private const string AnimIdleRight = "Worker_Idle_Right";

    private const string AnimMoveDown = "Worker_Move_Down";
    private const string AnimMoveUp = "Worker_Move_Up";
    private const string AnimMoveLeft = "Worker_Move_Left";
    private const string AnimMoveRight = "Worker_Move_Right";

    private Rigidbody2D rb;

    private Vector2 moveDirection;
    private Vector2 lastDirection = Vector2.down;

    private string currentAnimation;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        moveDirection = GetCardinalDirection(input);

        if (moveDirection != Vector2.zero)
        {
            lastDirection = moveDirection;
            PlayAnimation(GetMoveAnimation(moveDirection));
        }
        else
        {
            PlayAnimation(GetIdleAnimation(lastDirection));
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private Vector2 GetCardinalDirection(Vector2 input)
    {
        if (input == Vector2.zero)
            return Vector2.zero;

        // Only allow one direction at a time.
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return input.x > 0f
                ? Vector2.right
                : Vector2.left;
        }

        return input.y > 0f
            ? Vector2.up
            : Vector2.down;
    }

    private string GetMoveAnimation(Vector2 direction)
    {
        if (direction == Vector2.right)
            return AnimMoveRight;

        if (direction == Vector2.left)
            return AnimMoveLeft;

        if (direction == Vector2.up)
            return AnimMoveUp;

        return AnimMoveDown;
    }

    private string GetIdleAnimation(Vector2 direction)
    {
        if (direction == Vector2.right)
            return AnimIdleRight;

        if (direction == Vector2.left)
            return AnimIdleLeft;

        if (direction == Vector2.up)
            return AnimIdleUp;

        return AnimIdleDown;
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