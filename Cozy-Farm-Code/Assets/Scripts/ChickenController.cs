using UnityEngine;
using System.Collections;

public class ChickenController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.2f;
    public float directionChangeTime = 3f;

    [Header("Egg")]
    public GameObject eggPrefab;
    public float minEggCooldown = 3f; //8f;
    public float maxEggCooldown =  35f; //16f;

    [Header("Animation")]
    public Animator animator;

    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private bool isSitting = false;

    private const string AnimIdle1 = "Chick_Idle_1";
    private const string AnimIdle2 = "Chick_Idle_2";
    private const string AnimMove = "Chick_Move";
    private const string AnimPeck = "Chick_Peck";
    private const string AnimSit = "Chick_Sit";
    private const string AnimStand = "Chick_Stand";
    private const string AnimSitIdle = "Chick_Sit_Idle";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure this chicken ignores collisions with existing worker colliders
        var myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
            var workers = WorkerController.WorkerColliders;
            for (int i = 0; i < workers.Count; i++)
            {
                var w = workers[i];
                if (w != null)
                    Physics2D.IgnoreCollision(myCol, w, true);
            }
        }
        PickRandomDirection();
        StartCoroutine(RandomBehaviourLoop());
        StartCoroutine(EggRoutine());
    }

    void FixedUpdate()
    {
        if (!isSitting)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    void PickRandomDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        PlayAnimation(AnimMove);

        // flip sprite
        if (moveDirection.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveDirection.x), 1, 1);
        }
    }

    IEnumerator RandomBehaviourLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, directionChangeTime));
            
            if (isSitting) continue;

            int action = Random.Range(0, 6);

            switch (action)
            {
                case 0:
                    PlayAnimation(AnimIdle1);
                    moveDirection = Vector2.zero;
                    rb.linearVelocity = Vector2.zero;
                    break;

                case 1:
                    PlayAnimation(AnimIdle2);
                    moveDirection = Vector2.zero;
                    rb.linearVelocity = Vector2.zero;
                    break;

                case 2:
                    PlayAnimation(AnimPeck);
                    moveDirection = Vector2.zero;
                    rb.linearVelocity = Vector2.zero;
                    break;

                case 3:
                case 4:
                case 5:
                    PickRandomDirection();
                    break;
            }
        }
    }

    IEnumerator EggRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minEggCooldown, maxEggCooldown);
            yield return new WaitForSeconds(waitTime);
            if (isSitting) continue;
            yield return StartCoroutine(SitAndLayEgg());
        }
    }

    IEnumerator SitAndLayEgg()
    {
        isSitting = true;
        moveDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        PlayAnimation(AnimSit);

        yield return new WaitForSeconds(2f); // time to sit down

        // Instantiate egg
        var egg = Instantiate(eggPrefab, transform.position + Vector3.down * 0.05f, Quaternion.identity);
        // ensure egg ignores workers
        if (egg != null)
        {
            var eggCol = egg.GetComponent<Collider2D>();
            if (eggCol != null)
            {
                var workers = WorkerController.WorkerColliders;
                for (int i = 0; i < workers.Count; i++)
                {
                    var w = workers[i];
                    if (w != null)
                        Physics2D.IgnoreCollision(eggCol, w, true);
                }
            }
        }

        PlayAnimation(AnimSitIdle);

        yield return new WaitForSeconds(1.5f); // sitting idle

        PlayAnimation(AnimStand);
        AudioManager.Instance?.PlayEggLay();

        yield return new WaitForSeconds(1f); // time to stand up
        
        isSitting = false;
        PickRandomDirection();
    }

    private void PlayAnimation(string clipName)
    {
        if (animator != null)
        {
            animator.Play(clipName);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
       if (isSitting) return;

       Vector2 normal = collision.contacts[0].normal;
       moveDirection = Vector2.Reflect(moveDirection, normal).normalized;
       PickRandomDirection();
    }

}
