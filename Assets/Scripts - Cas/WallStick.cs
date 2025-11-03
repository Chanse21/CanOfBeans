using UnityEngine;

public class WallStick : MonoBehaviour
{
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 10f;
    public Transform groundCheck; // Assign a Transform for ground detection
    public LayerMask groundLayer; // Assign the LayerMask for ground objects

    private Rigidbody2D rb;
    private bool isWallSliding;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Example: Check for wall sliding input
        if (isWallSliding && !isGrounded && Input.GetAxisRaw("Horizontal") != 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }

        // Example: Wall jump
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(-Mathf.Sign(rb.transform.localScale.x) * wallJumpForce, wallJumpForce);
            isWallSliding = false; // Exit wall slide state
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && !isGrounded) // Assuming walls have "Wall" tag
        {
            isWallSliding = true;
            // Potentially reduce gravity scale here
            rb.gravityScale = 5f; 
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWallSliding = false;
            // Reset gravity scale
            rb.gravityScale = 5f; 
        }
    }
}