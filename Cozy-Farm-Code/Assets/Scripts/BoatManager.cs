using TMPro;
using UnityEngine;

public class BoatManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoatController boat;
    [SerializeField] private Transform player;
    [SerializeField] private Behaviour playerMovement;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [SerializeField] private Transform boardingPoint;
    [SerializeField] private Vector3 boardingOffset;

    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 2f;

    [SerializeField] private TextMeshProUGUI sailHint;
    [SerializeField] private TextMeshProUGUI leaveHint;

    [Header("Landing")]
    [SerializeField] private LayerMask landLayers;
    [SerializeField] private float landCheckRadius = 1.5f;

    private bool playerNearby;

    void Awake()
    {
        if (boat == null)
            boat = GetComponent<BoatController>();

        if (player != null && playerRigidbody == null)
            playerRigidbody = player.GetComponent<Rigidbody2D>();

        SetHintVisible(sailHint, false);
        SetHintVisible(leaveHint, false);
    }

    void Update()
    {
        if (boat == null || player == null)
            return;

        // ==================================================
        // PLAYER IS ON BOAT
        // ==================================================

        if (boat.IsBoarded)
        {
            SetHintVisible(sailHint, false);

            bool canLeave = CanLeaveBoat();

            SetHintVisible(leaveHint, canLeave);

            if (canLeave && Input.GetKeyDown(KeyCode.Return))
            {
                LeaveBoat();
            }

            return;
        }

        // ==================================================
        // PLAYER IS NOT ON BOAT
        // ==================================================

        playerNearby = Vector2.Distance(
            player.position,
            boat.transform.position
        ) <= interactionRadius;

        SetHintVisible(sailHint, playerNearby);

        if (playerNearby && Input.GetKeyDown(KeyCode.Return))
        {
            BoardBoat();
        }
    }

    private void BoardBoat()
    {
        if (boat == null || player == null)
            return;

        // Stop player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Stop physics from fighting the boat while parented.
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            playerRigidbody.simulated = false;
        }

        // Parent player to boat.
        player.SetParent(boat.transform);

        // Position player on the boat.
        player.localPosition = boardingOffset;

        // Hide the interaction hint.
        SetHintVisible(sailHint, false);

        // Tell boat that it is now occupied.
        boat.SetBoarded(true);

        Debug.Log("Player boarded boat.");
    }

    private void LeaveBoat()
    {
        if (boat == null || player == null)
            return;

        // Remove player from boat.
        player.SetParent(null);

        // Put player on the landing position.
        if (boardingPoint != null)
        {
            player.position = boardingPoint.position;
        }
        else
        {
            player.position = boat.transform.position;
        }

        // Re-enable player physics.
        if (playerRigidbody != null)
        {
            playerRigidbody.simulated = true;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        // Re-enable player movement.
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Tell boat it is no longer occupied.
        boat.SetBoarded(false);

        SetHintVisible(leaveHint, false);

        Debug.Log("Player left boat.");
    }

    private bool CanLeaveBoat()
    {
        if (landLayers.value == 0)
            return false;

        if (landCheckRadius <= 0f)
            return false;

        // Check around the BOAT, not the manager object.
        return Physics2D.OverlapCircle(
            boat.transform.position,
            landCheckRadius,
            landLayers
        ) != null;
    }

    private void SetHintVisible(
        TextMeshProUGUI hint,
        bool visible)
    {
        if (hint != null)
        {
            hint.gameObject.SetActive(visible);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (boat == null)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            boat.transform.position,
            interactionRadius
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            boat.transform.position,
            landCheckRadius
        );
    }
}