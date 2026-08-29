using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float directionChangeTime = 3f;

    [Header("Auto Collection")]
    [SerializeField] private float eggSearchRadius = 20f;
    [SerializeField] private float eggSearchInterval = 0.25f;
    [SerializeField] private float pathAlignmentTolerance = 0.08f;
    [SerializeField, Range(0f, 1f)] private float idleChance = 0.8f;
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float obstacleAvoidanceDuration = 0.5f;

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

    private Vector2 moveDirection;
    private Vector2 lastDirection = Vector2.down;
    private string currentAnim = "";
    private float eggSearchTimer;
    private float directionTimer;
    private float obstacleAvoidanceTimer;
    private bool isSeekingEgg;
    private bool isIdle;
    private CollectibleItem targetEgg;
    private int pathAxis;

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
        eggSearchTimer += Time.deltaTime;
        directionTimer -= Time.deltaTime;
        obstacleAvoidanceTimer -= Time.deltaTime;

        if (obstacleAvoidanceTimer <= 0f && eggSearchTimer >= eggSearchInterval)
        {
            eggSearchTimer = 0f;
            FindNearestEgg();
        }

        if (targetEgg != null)
        {
            UpdateEggPath();
        }
        else if (!isSeekingEgg && directionTimer <= 0f)
        {
            PickRandomBehavior();
        }

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            lastDirection = moveDirection.normalized;
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
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void PickRandomDirection()
    {
        isSeekingEgg = false;
        isIdle = false;
        targetEgg = null;
        int direction = Random.Range(0, 4);
        moveDirection = direction switch
        {
            0 => Vector2.left,
            1 => Vector2.right,
            2 => Vector2.up,
            _ => Vector2.down
        };
        directionTimer = directionChangeTime;
    }

    private void PickRandomBehavior()
    {
        if (Random.value < idleChance)
        {
            isIdle = true;
            moveDirection = Vector2.zero;
            directionTimer = idleDuration;
            return;
        }

        PickRandomDirection();
    }

    private void FindNearestEgg()
    {
        if (eggSearchRadius <= 0f) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            eggSearchRadius,
            collectibleLayers);

        CollectibleItem nearestEgg = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            CollectibleItem egg = hits[i].GetComponent<CollectibleItem>();
            if (egg == null || egg.ResourceType != FarmResourceType.Egg) continue;

            float distanceSqr = (egg.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestEgg = egg;
                nearestDistanceSqr = distanceSqr;
            }
        }

        if (nearestEgg != null)
        {
            if (targetEgg != nearestEgg)
            {
                targetEgg = nearestEgg;
                pathAxis = Random.Range(0, 2);
            }

            isSeekingEgg = true;
            isIdle = false;
        }
        else
        {
            targetEgg = null;
            isSeekingEgg = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCollect(collision.collider);

        if (collision.collider.GetComponent<CollectibleItem>() != null)
        {
            return;
        }

        if (collision.contactCount == 0)
        {
            return;
        }

        Vector2 normal = collision.GetContact(0).normal;
        targetEgg = null;
        isSeekingEgg = false;
        moveDirection = GetCardinalDirection(normal);
        lastDirection = moveDirection;
        directionTimer = directionChangeTime;
        obstacleAvoidanceTimer = obstacleAvoidanceDuration;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<CollectibleItem>() != null ||
            collision.contactCount == 0 || obstacleAvoidanceTimer > 0f)
        {
            return;
        }

        targetEgg = null;
        isSeekingEgg = false;
        moveDirection = GetCardinalDirection(collision.GetContact(0).normal);
        lastDirection = moveDirection;
        directionTimer = directionChangeTime;
        obstacleAvoidanceTimer = obstacleAvoidanceDuration;
    }

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x >= 0f ? Vector2.right : Vector2.left;
        }

        return direction.y >= 0f ? Vector2.up : Vector2.down;
    }

    private void UpdateEggPath()
    {
        Vector2 toEgg = targetEgg.transform.position - transform.position;

        if (pathAxis == 0 && Mathf.Abs(toEgg.x) <= pathAlignmentTolerance)
        {
            pathAxis = 1;
        }
        else if (pathAxis == 1 && Mathf.Abs(toEgg.y) <= pathAlignmentTolerance)
        {
            pathAxis = 0;
        }

        moveDirection = pathAxis == 0
            ? new Vector2(Mathf.Sign(toEgg.x), 0f)
            : new Vector2(0f, Mathf.Sign(toEgg.y));

        if (moveDirection == Vector2.zero)
        {
            targetEgg = null;
            isSeekingEgg = false;
        }
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
