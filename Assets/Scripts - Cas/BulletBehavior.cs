using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 7f;          // Speed of bullet
    public float lifetime = 3f;       // Time before auto-destroy
    public int damage = 1;            // Damage dealt to player

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Move forward (bullet faces right by default in prefab)
        rb.linearVelocity = transform.right * speed;

        // Destroy after lifetime so bullets dont stack up
        Destroy(gameObject, lifetime);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit the player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            // If bullet hits player - Display log, Deal Damage and Destroy the bullet
            Debug.Log("Bullet hit player!");
            player.TakeDamage(damage);  // Deal damage
            Destroy(gameObject);        // Remove bullet
        }
        
    }
}
