using UnityEngine;

public class WallStick : MonoBehaviour
{

    [Header("Wall Stick Settings")]

    public float wallStickGravityScale = 0f; // Gravity scale when player is sticking to the wall (0 means no falling)

    public float normalGravityScale = 5f;    // Normal gravity when player is not on the wall

    public float wallJumpForce = 10f;        // Force applied when jumping off a wall



    [Header("Ground Check")]

    public Transform groundCheck;            // Transform used to check if the player is standing on the ground

    public LayerMask groundLayer;            // Layer mask to detect what counts as ground



    private Rigidbody2D rb;                  // Reference to the player's Rigidbody2D component

    private bool isWallSticking;             // Tracks if the player is currently sticking to a wall

    private bool isGrounded;                 // Tracks if the player is touching the ground



    void Start()

    {

        // Get the Rigidbody2D component at the start of the game

        rb = GetComponent<Rigidbody2D>();



        // Set the default gravity to normal

        rb.gravityScale = normalGravityScale;

    }



    void Update()

    {

        // Check if the player is on the ground using a small circle at the groundCheck position

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);



        // If the player is on the wall and presses the jump button, perform a wall jump

        if (isWallSticking && Input.GetButtonDown("Jump"))

        {

            // Restore normal gravity after jumping off the wall

            rb.gravityScale = normalGravityScale;



            // Apply a force away from the wall and upward

            // Mathf.Sign(transform.localScale.x) checks the direction the player is facing

            rb.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpForce, wallJumpForce);



            // The player is no longer sticking to the wall after jumping

            isWallSticking = false;

        }

    }



    void FixedUpdate()

    {

        // This runs every physics update

        // If the player is sticking to a wall, stop their vertical velocity so they don't slide down

        if (isWallSticking)

        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        }

    }



    void OnCollisionEnter2D(Collision2D collision)

    {

        // When the player first collides with an object

        // Check if it’s tagged as "Wall" and the player isn’t grounded

        if (collision.gameObject.CompareTag("Wall") && !isGrounded)

        {

            // Player starts sticking to the wall

            isWallSticking = true;



            // Disable gravity while sticking so they don’t fall

            rb.gravityScale = wallStickGravityScale;



            // Stop all motion to make sure the player sticks instantly

            rb.linearVelocity = Vector2.zero;

        }

    }



    void OnCollisionExit2D(Collision2D collision)

    {

        // When the player stops touching the wall

        if (collision.gameObject.CompareTag("Wall"))

        {

            // The player is no longer sticking

            isWallSticking = false;



            // Restore normal gravity so the player can fall again

            rb.gravityScale = normalGravityScale;

        }

    }

}