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
            WorkerColliders.Add(myCollider);

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

    [Header("Collection")]
    [SerializeField] private float collectRadius = 0.5f;
    [SerializeField] private LayerMask collectibleLayers;
    [SerializeField] private float collectCheckInterval = 0.2f;

    private float collectTimer = 0f;

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

        collectTimer += Time.deltaTime;
        if (collectTimer >= collectCheckInterval)
        {
            collectTimer = 0f;
            TryCollectNearby();
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput.normalized * moveSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryCollect(collision.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other);
    }

    private void TryCollect(Collider2D col)
    {
        if (col == null) return;

        var collectible = col.GetComponent<CollectibleItem>();
        if (collectible == null) return;

        if (collectible.ResourceType == FarmResourceType.Egg)
        {
            AudioManager.Instance?.PlayClick();
            collectible.Collect();
        }
    }

    private void TryCollectNearby()
    {
        if (collectRadius <= 0f) return;

        int mask = collectibleLayers;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, collectRadius, mask);
        for (int i = 0; i < hits.Length; i++)
        {
            var item = hits[i].GetComponent<CollectibleItem>();
            if (item != null && item.ResourceType == FarmResourceType.Egg)
            {
                AudioManager.Instance?.PlayClick();
                item.Collect();
            }
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
