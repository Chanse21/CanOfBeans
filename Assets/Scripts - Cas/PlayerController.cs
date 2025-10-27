using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f; //Movement speed
    public float jumpForce = 16f; //How high the player jumps

    private Rigidbody2D rb;
    private bool isGrounded;
    public SpriteRenderer SR;

   
    // CONTROLS A + D to MOVE Right click to teleport
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
         float moveInput = 0f;
         

         if (Input.GetKey("a"))
         {
            moveInput = -1f; //move left
           SR.flipX = true;
         }

         if (Input.GetKey("d"))
         {
            moveInput = 1f; //move right
            SR.flipX = false;

         }
          
         //Apply movement using physics velocity
         rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        // Detects if player presses space while they are moving on the ground. The player jumps.
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //Check if player landed on ground
             if (collision.gameObject.CompareTag("Ground"))
             {
                isGrounded = true;
             }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            //Check if player left the ground
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }
    }
