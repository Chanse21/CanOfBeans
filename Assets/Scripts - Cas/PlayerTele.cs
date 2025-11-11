using UnityEngine;
using System;
using System.Collections;

public class PlayerTele : MonoBehaviour

{
    private Camera cam; // Reference to the main camera for getting mouse world position

    [Header("Teleport")]
    public float PlayerRadius = 5f; // Maximum teleport distance from player
    public float teleportCooldown = 3f; // Time between teleports (cooldown)
    private float nextTeleportTime = 1f; // Time when player can next teleport
    public static event Action<float> OnTeleportUsed; // Event to notify UI of cooldown usage

    [Header("Effects")]
    public GameObject beamPrefab;         // Prefab used to display teleport beam
    public float beamDuration = 0.2f;     // Duration before beam is destroyed
    public SpriteRenderer playerSprite;   // Reference to the player's sprite (for hiding/reappearing)
    public float disappearTime = 0.1f;    // Time the player stays invisible when teleporting

    [Header("Teleport State")]
    public bool isTeleporting = false;    // Whether the player is currently teleporting
    public float teleportDuration = 0.2f; // Duration the player remains in a teleport state

    [Header("Teleport Chain Settings")]
    public int maxChains = 3;          // Maximum number of chained teleports
    public float chainRadius = 5f;     // How far to look for the next enemy for chaining
    public float chainDelay = 0.15f;   // Delay between chained teleports

    void Start()

    {
        // Cache the main camera at the start
        cam = Camera.main;
    }



    void Update()

    {
        // When the player right-clicks and teleport cooldown is ready
        if (Input.GetMouseButtonDown(1) && Time.time >= nextTeleportTime)
        {
            // Get target position from mouse click (limited by PlayerRadius)

            Vector3 targetPos = GetTeleportPosition();

            // Start the teleport coroutine (handles beam effect, invisibility, and movement)

            StartCoroutine(TeleportSequence(targetPos));

            // Reset the cooldown timer

            nextTeleportTime = Time.time + teleportCooldown;

            // Notify UI elements about the cooldown usage

            OnTeleportUsed?.Invoke(teleportCooldown);

            // Pause camera follow briefly during teleport for effect
            FindFirstObjectByType<SmoothCameraFollow>().SuspendCamera(0.4f);
        }
    }



    // Main teleportation process — runs as a coroutine
    IEnumerator TeleportSequence(Vector3 targetPos)

    {

        isTeleporting = true; // Mark that player is teleporting



        // 1️⃣ Hide the player's sprite for the teleport animation

        if (playerSprite != null)

            playerSprite.enabled = false;



        // Store current position for beam direction and chaining

        Vector3 currentPos = transform.position;


        // 2️⃣ Chain loop (handles multiple teleport jumps if enemies nearby)

        for (int i = 0; i < maxChains; i++)

        {
            // Cast a line between start and target to detect enemies

            RaycastHit2D[] hits = Physics2D.LinecastAll(currentPos, targetPos);

            foreach (RaycastHit2D hit in hits)

            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    // Destroy any enemy hit along teleport path

                    Debug.Log("Enemy destroyed by teleport!");

                    Destroy(hit.collider.gameObject);
                }
            }
            // 3️⃣ Spawn teleport beam visual effect

            if (beamPrefab != null)
            {
                GameObject beam = Instantiate(beamPrefab);

                LineRenderer line = beam.GetComponent<LineRenderer>();
                if (line != null)
                {
                    // Set beam start and end positions
                    line.SetPosition(0, currentPos);  // From player's current position

                    line.SetPosition(1, targetPos);   // To teleport destination
                }
                // Destroy beam after short duration to clean up
                Destroy(beam, beamDuration);
            }

            // 4️⃣ Wait while player is invisible (adds teleport delay)

            yield return new WaitForSeconds(disappearTime);



            // 5️⃣ Move player instantly to target position

            transform.position = targetPos;

            currentPos = targetPos; // Update current position for next loop

            // 6️⃣ Find next enemy for teleport chaining

            Collider2D nextEnemy = FindClosestEnemy(currentPos, chainRadius);

            if (nextEnemy != null)

            {
                // Set next teleport destination to that enemy’s position

                targetPos = nextEnemy.transform.position;

                // Small delay between chained teleports for visual pacing

                yield return new WaitForSeconds(chainDelay);
            }
            else
            {
                // No more enemies nearby — stop chaining
                break;
            }
        }
        // 7️⃣ Re-enable player visibility after teleport sequence

        if (playerSprite != null)

            playerSprite.enabled = true;

        // 8️⃣ Wait a bit before ending teleport state

        yield return new WaitForSeconds(teleportDuration);

        isTeleporting = false; // Done teleporting

    }
    // Gets the mouse position and limits teleport distance to PlayerRadius

    Vector3 GetTeleportPosition()

    {
        // Convert mouse position from screen to world space

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPos.z = 0f; // Keep on 2D plane

        // Calculate direction and distance from player to mouse

        Vector3 direction = (mouseWorldPos - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, mouseWorldPos);

        if (distance <= PlayerRadius)
        {
            // If within teleport radius, teleport directly to mouse position
            return mouseWorldPos;
        }
        else
        {
            // If outside radius, teleport to the maximum allowed edge
            return transform.position + direction * PlayerRadius;
        }
    }
    // Finds the closest enemy within a given radius for chaining teleports
    Collider2D FindClosestEnemy(Vector3 currentPos, float radius)
    {
        // Detect all colliders in a circular area around current position

        Collider2D[] enemies = Physics2D.OverlapCircleAll(currentPos, radius);

        Collider2D closest = null;

        float minDist = Mathf.Infinity;

        // Loop through all nearby colliders to find closest enemy

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(currentPos, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }
        }
        // Always return a value, even if no enemies found (returns null)
        return closest;
    }
}