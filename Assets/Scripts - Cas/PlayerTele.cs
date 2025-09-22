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

    void Start()
    {
        cam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        //Check if user presses the right mouse button at any point. 
        if (Input.GetMouseButtonDown(1) && Time.time >= nextTeleportTime)// Right-click
        {
            // TryTeleport();        OLD TELEPORT   DON'T DELETE
            // nextTeleportTime = Time.time + teleportCooldown;
            // Tell UI how long cooldown is
            // OnTeleportUsed?.Invoke(teleportCooldown);
            // Camera delay
            // FindFirstObjectByType<SmoothCameraFollow>().SuspendCamera(0.4f);
            Vector3 targetPos = GetTeleportPosition();

            // Start teleport coroutine (handles disappear, beam, reappear)
            StartCoroutine(TeleportSequence(targetPos));

            // Set cooldown
            nextTeleportTime = Time.time + teleportCooldown;

            // Tell UI how long cooldown is
            OnTeleportUsed?.Invoke(teleportCooldown);

            // Small camera suspend
            FindFirstObjectByType<SmoothCameraFollow>().SuspendCamera(0.4f);
        }

    }
    //     NEW TEST!!!!!!
    IEnumerator TeleportSequence(Vector3 targetPos)
    {
        // 1. Hide player
        if (playerSprite != null)
            playerSprite.enabled = false;

        // 2. Spawn teleport beam effect
        if (beamPrefab != null)
        {
            GameObject beam = Instantiate(beamPrefab, transform.position, Quaternion.identity);
            // Stretch/rotate beam to point to target
            Vector3 dir = (targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPos);
            beam.transform.right = dir; // Align beam
            beam.transform.localScale = new Vector3(distance, beam.transform.localScale.y, 1);

            Destroy(beam, beamDuration); // Remove beam after short duration
        }
        yield return new WaitForSeconds(disappearTime);
        transform.position = targetPos;
        if (playerSprite != null)
            playerSprite.enabled = true;
    }
    /*      OLD TELEPORT   DON'T DELETE
        void TryTeleport()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Direction from player to mouse
        Vector3 direction = (mouseWorldPos - transform.position).normalized;

        // Distance from player to click
        float distance = Vector3.Distance(transform.position, mouseWorldPos);

        Vector3 targetPos;

        if (distance <= PlayerRadius)
        {
            // Within radius → teleport directly
            targetPos = mouseWorldPos;
        }
        else
        {
            // Outside radius → teleport to edge of radius
            targetPos = transform.position + direction * PlayerRadius;
        }

        transform.position = targetPos;

    }
    */
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
