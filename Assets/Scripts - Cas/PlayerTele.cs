using UnityEngine;
using System;
using System.Collections;

public class PlayerTele : MonoBehaviour
{
      
    
    private Camera cam;

    [Header("Teleport")]
    public float PlayerRadius = 5f; // Players teleportation radius
    public float teleportCooldown = 3f;  // Time delay between teleports
    private float nextTeleportTime = 1f; // when player can teleport again
    
    public static event Action<float> OnTeleportUsed;

    [Header("Effects")]
    public GameObject beamPrefab;           // Prefab for teleport beam (set in Inspector)
    public float beamDuration = 0.2f;       // How long the beam shows
    public SpriteRenderer playerSprite;     // Reference to the player’s sprite (drag in Inspector)
    public float disappearTime = 0.1f;      // How long player stays invisible

    [Header("Teleport State")]
    public bool isTeleporting = false;
    public float teleportDuration = 0.2f; // how long the player is considered 'teleporting'

    void Start()
    {
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        // Right-click to teleport if cooldown is up
        if (Input.GetMouseButtonDown(1) && Time.time >= nextTeleportTime)
        {
            
            Vector3 targetPos = GetTeleportPosition();

            // Start teleport coroutine (handles disappear, beam, reappear, enemy)
            StartCoroutine(TeleportSequence(targetPos));

            // Set cooldown
            nextTeleportTime = Time.time + teleportCooldown;

            // Tell UI how long cooldown is
            OnTeleportUsed?.Invoke(teleportCooldown);

            // Small camera suspend
            FindFirstObjectByType<SmoothCameraFollow>().SuspendCamera(0.4f);
        }

    }
    IEnumerator TeleportSequence(Vector3 targetPos)
    {
        isTeleporting = true; // ✅ Start teleport state

        // 1. Hide player
        if (playerSprite != null)
            playerSprite.enabled = false;

        // 2. Check for enemies along the teleport path
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, targetPos);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy destroyed by teleport!");
                Destroy(hit.collider.gameObject);
            }
        }
        // 3. Spawn teleport beam effect
        if (beamPrefab != null)
        {
            GameObject beam = Instantiate(beamPrefab);
            LineRenderer line = beam.GetComponent<LineRenderer>();

            if (line != null)
            {
                // Set start and end positions
                line.SetPosition(0, transform.position);  // Player’s current position
                line.SetPosition(1, targetPos);           // Teleport destination
            }

            Destroy(beam, beamDuration); // Remove beam after short duration
        }
        // 4. Wait while invisible
        yield return new WaitForSeconds(disappearTime);
        // 5. Move player
        transform.position = targetPos;
        // 6. Re-enable sprite
        if (playerSprite != null)
            playerSprite.enabled = true;

            // 7. End teleport state ✅
        yield return new WaitForSeconds(teleportDuration);
        isTeleporting = false;
    }
    
    Vector3 GetTeleportPosition()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Direction from player to mouse
        Vector3 direction = (mouseWorldPos - transform.position).normalized;

        // Distance from player to click
        float distance = Vector3.Distance(transform.position, mouseWorldPos);

        if (distance <= PlayerRadius)
        {
            // Within radius → teleport directly
            return mouseWorldPos;
        }
        else
        {
            // Outside radius → teleport to edge of radius
            return transform.position + direction * PlayerRadius;
        }
    }
}
