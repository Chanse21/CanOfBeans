using UnityEngine;
using System.Collections;


public class GroundEnemyAi : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;         // Speed of walking
    public float patrolDistance = 3f;    // Distance to walk each way
    public float waitTime = 1f;          // Time waiting before turning

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;      // Bullet prefab
    public Transform firePoint;          // Where bullets spawn (child object in front of enemy)
    public float fireRate = 1f;          // Bullets per second
    public float visionRange = 5f;       // Distance at which enemy sees player

    [Header("References")]
    public Transform player;             // Assign Player in Inspector

    private Vector3 startPos;
    private bool movingRight = false;
    private bool isPatrolling = true;
    private bool isShooting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(PatrolRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Player in vision → stop patrol, start shooting
            if (distanceToPlayer <= visionRange)
            {
                if (isPatrolling)
                {
                    isPatrolling = false;
                    StopAllCoroutines();
                }

                if (!isShooting)
                {
                    isShooting = true;
                    StartCoroutine(ShootRoutine());
                }
            }
            else
            {
                // Player out of vision → resume patrol
                if (!isPatrolling)
                {
                    isPatrolling = true;
                    isShooting = false;
                    StopAllCoroutines();
                    StartCoroutine(PatrolRoutine());
                }
            }
        }
    }

    IEnumerator PatrolRoutine()
    {
        while (isPatrolling)
        {
            Vector3 targetPos = startPos + (movingRight ? Vector3.right : Vector3.left) * patrolDistance;

            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Reached target → wait
            yield return new WaitForSeconds(waitTime);

            // Flip direction
            movingRight = !movingRight;
        }
    }

    IEnumerator ShootRoutine()
    {
        while (isShooting)
        {
            if (bulletPrefab != null && firePoint != null && player != null)
            {
                // Calculate direction from firePoint to player
                Vector3 dir = (player.position - firePoint.position).normalized;
                // Calculate rotation so the bullet faces the player
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                // Instantiate bullet and rotate it towards player
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
                // Set bullet velocity in the same direction
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = dir * rb.linearVelocity.magnitude; // Use bullet speed from Rigidbody2D
                }
               //  Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

    // Detect when player teleports through enemy
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerTele tele = collision.GetComponent<PlayerTele>();
            if (tele != null)
            {
                // If player is in teleport "invisible" state → kill enemy
                if (!tele.playerSprite.enabled)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
