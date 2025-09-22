using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 5f;          // Speed of bullet
    public float lifetime = 3f;       // Time before auto-destroy
    public int damage = 1;            // Damage dealt to player

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Move forward (bullet faces right by default in prefab)
        rb.linearVelocity = transform.right * speed;
        // Ignore collisions with platforms
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject plat in platforms)
        {
            Collider2D platCol = plat.GetComponent<Collider2D>();
            Collider2D bulletCol = GetComponent<Collider2D>();
            if (platCol != null && bulletCol != null)
                Physics2D.IgnoreCollision(bulletCol, platCol);
        }

        // Destroy after a few seconds
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if bullet hit the player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy bullet on hit
        }

        // Optional: destroy if bullet hits environment
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
