using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float directionChangeTime = 3f;

    [Header("Animation")]
    private Animator animator;
    private Rigidbody2D rb;
    [Header("Collision")]
    [Tooltip("Layers worker should not collide with.")]
    [SerializeField] private LayerMask ignoreCollisionWith;
    private const string AnimIdleDown = "Worker_Idle_Down";
    private const string AnimIdleUp = "Worker_Idle_Up";
    private const string AnimIdleLeft = "Worker_Idle_Left";
    private const string AnimIdleRight = "Worker_Idle_Right";
    private const string AnimWalkDown = "Worker_Move_Down";
    private const string AnimWalkUp = "Worker_Move_Up";
    private const string AnimWalkLeft = "Worker_Move_Left";
    private const string AnimWalkRight = "Worker_Move_Right";

    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;
    private string currentAnim = "";

    // Registry of active worker colliders so newly spawned animals/eggs can ignore them.
    public static readonly System.Collections.Generic.List<Collider2D> WorkerColliders =
        new System.Collections.Generic.List<Collider2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        var myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            // register this worker collider for others to reference
            WorkerColliders.Add(myCollider);
        }
        if (myCollider != null)
        {
            int mask = ignoreCollisionWith;

            var all = FindObjectsOfType<Collider2D>();
            for (int i = 0; i < all.Length; i++)
            {
                var other = all[i];
                if (other == myCollider) continue;
                if ((mask & (1 << other.gameObject.layer)) != 0)
                {
                    Physics2D.IgnoreCollision(myCollider, other, true);
                }
            }
        }
    }

    void OnDestroy()
    {
        var myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            WorkerColliders.Remove(myCollider);
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(h, v);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastDirection = moveInput.normalized;
            PlayWalkAnimation(lastDirection);
        }
        else
        {
            PlayIdleAnimation(lastDirection);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput.normalized * moveSpeed;
        }
    }

    private void PlayWalkAnimation(Vector2 dir)
    {
        string next = AnimWalkDown;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            next = dir.x > 0 ? AnimWalkRight : AnimWalkLeft;
        }
        else
        {
            next = dir.y > 0 ? AnimWalkUp : AnimWalkDown;
        }

        if (currentAnim != next && animator != null)
        {
            animator.Play(next);
            currentAnim = next;
        }
    }

    private void PlayIdleAnimation(Vector2 dir)
    {
        string next = AnimIdleDown;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            next = dir.x > 0 ? AnimIdleRight : AnimIdleLeft;
        }
        else
        {
            next = dir.y > 0 ? AnimIdleUp : AnimIdleDown;
        }

        if (currentAnim != next && animator != null)
        {
            animator.Play(next);
            currentAnim = next;
        }
    }

}
