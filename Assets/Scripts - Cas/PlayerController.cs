using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]

    public float moveSpeed = 8f;     // Normal walking speed

    public float runSpeed = 18f;     // Running speed when holding Left Shift

    private float currentSpeed;      // The speed currently being used (walk or run)

    public float jumpForce = 16f;    // How high the player jumps



    private Rigidbody2D rb;          // Reference to the Rigidbody2D component

    private bool isGrounded;         // Tracks whether the player is standing on the ground

    public SpriteRenderer SR;        // Used to flip the player's sprite when facing left or right



    void Start()

    {

        // Get the Rigidbody2D component at the start

        rb = GetComponent<Rigidbody2D>();



        // Initialize the current movement speed to the default walking speed

        currentSpeed = moveSpeed;

    }



    void Update()

    {

        float moveInput = 0f; // Store player’s horizontal input (left/right)



        // --- MOVEMENT INPUT ---

        // Move left when pressing 'A'

        if (Input.GetKey("a"))

        {

            moveInput = -1f;    // Move in the negative X direction

            SR.flipX = true;    // Flip sprite to face left

        }



        // Move right when pressing 'D'

        if (Input.GetKey("d"))

        {

            moveInput = 1f;     // Move in the positive X direction

            SR.flipX = false;   // Flip sprite to face right

        }



        // --- RUNNING INPUT ---

        // If the player is holding the Left Shift key, increase movement speed

        if (Input.GetKey(KeyCode.LeftShift))

        {

            currentSpeed = runSpeed;  // Run speed

        }

        else

        {

            currentSpeed = moveSpeed; // Normal walking speed

        }



        // --- APPLY MOVEMENT ---

        // Apply horizontal movement by directly setting the Rigidbody’s velocity

        // This makes the player move smoothly and interact with physics

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);



        // --- JUMP INPUT ---

        // Allow jumping only if the player is currently grounded

        // The player can press Space or W to jump

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)

        {

            // Apply an upward force for jumping

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        }

    }

    // --- GROUND DETECTION ---

    // Called when the player collides with another object

    void OnCollisionEnter2D(Collision2D collision)

    {

        // If the object has the "Ground" tag, mark the player as grounded

        if (collision.gameObject.CompareTag("Ground"))

        {

            isGrounded = true;

        }

    }
    // Called when the player stops touching an object

    void OnCollisionExit2D(Collision2D collision)

    {

        // If the player is no longer touching the ground, mark as not grounded

        if (collision.gameObject.CompareTag("Ground"))

        {

            isGrounded = false;

        }

    }

}