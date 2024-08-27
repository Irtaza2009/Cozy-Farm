using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Animator animator;
    private Vector2 movement;

    private float minX, maxX, minY, maxY;
    private GameManager gameManager;

    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;

    private bool isOnSpot = false;
    private string currentSpotTag = "";

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();

        UpdateBoundaries();

        dialogBox.SetActive(false);
    }

    void Update()
    {
        ProcessInputs();
        AnimateMovement();

        if (Input.GetKeyDown(KeyCode.Return) && isOnSpot)
        {
            CheckStandingSpot();
        }

         if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckPlotInteraction();
        }


        UpdateBoundaries();
    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");
    }

    void AnimateMovement()
    {
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("isMoving", true);

            if (movement.x > 0)
            {
                animator.Play("WorkerRight");
            }
            else if (movement.x < 0)
            {
                animator.Play("WorkerLeft");
            }
            else if (movement.y > 0)
            {
                animator.Play("WorkerUp");
            }
            else if (movement.y < 0)
            {
                animator.Play("WorkerDown");
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
            animator.Play("WorkerIdle");
        }
    }

    void Move()
    {
        Vector3 newPosition = transform.position + (Vector3)movement * moveSpeed * Time.deltaTime;

        // Clamp the new position within the boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Egg"))
        {
            gameManager.CollectEgg(other.gameObject); // Collect egg
        }
        else if (other.CompareTag("Milk"))
        {
            gameManager.CollectMilk(other.gameObject); // Collect milk
        }
        else if (other.CompareTag("ChickenSpot") || other.CompareTag("CowSpot"))
        {
            isOnSpot = true;
            currentSpotTag = other.CompareTag("ChickenSpot") ? "ChickenSpot" : "CowSpot";
            ShowDialog(currentSpotTag);
        }
        else if (other.CompareTag("Plot"))
        {
            isOnSpot = true;
            currentSpotTag =  other.CompareTag("Plot") ? "Plot" : "";
            ShowDialog(currentSpotTag);
        }
        else if (other.CompareTag("GardenSpot"))
        {
            isOnSpot = true;
            currentSpotTag =  other.CompareTag("GardenSpot") ? "GardenSpot" : "";
            ShowDialog(currentSpotTag);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ChickenSpot") || other.CompareTag("CowSpot") || other.CompareTag("Plot") || other.CompareTag("GardenSpot"))
        {
            isOnSpot = false;
            currentSpotTag = "";
            dialogBox.SetActive(false);
        }
    }

    public void UpdateBoundaries()
    {
        minX = GameObject.Find("LeftBoundary").transform.position.x + 0.5f;
        maxX = GameObject.Find("RightBoundary").transform.position.x - 0.5f;
        minY = GameObject.Find("BottomBoundary").transform.position.y + 0.5f;
        maxY = GameObject.Find("TopBoundary").transform.position.y - 0.5f;
    }

    void CheckStandingSpot()
    {
        if (currentSpotTag == "ChickenSpot")
        {
            SceneManager.LoadScene("ChickenFarm");
        }
        else if (currentSpotTag == "CowSpot")
        {
            SceneManager.LoadScene("CowFarm");
        }
        else if (currentSpotTag == "GardenSpot")
        {
            SceneManager.LoadScene("Garden");
        }
    }

    void ShowDialog(string spotTag)
    {
        dialogBox.SetActive(true);
        if (spotTag == "ChickenSpot")
        {
            dialogText.text = "Press Enter to enter Chicken Farm";
        }
        else if (spotTag == "CowSpot")
        {
            dialogText.text = "Press Enter to enter Cow Farm";
        }
        else if (spotTag == "GardenSpot")
        {
            dialogText.text = "Press Enter to enter Garden";
        }
        else if (spotTag == "Plot")
        {
            dialogText.text = "Press Enter to plant or harvest";
        }
    }

    void CheckPlotInteraction()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var hitCollider in hitColliders)
        {
            Plot plot = hitCollider.GetComponent<Plot>();
            if (plot != null)
            {
                if (plot.Harvest())
                {
                    gameManager.CollectFruit(); // Collect the fruit or vegetable
                }
                else
                {
                    plot.PlantSeed(); // Plant a seed
                }
            }
        }
    }



}
