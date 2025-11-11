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



    // 🔹 New chain settings

    [Header("Teleport Chain Settings")]

    public int maxChains = 3;          // How many enemies can be hit in a chain

    public float chainRadius = 5f;     // How far to look for the next enemy

    public float chainDelay = 0.15f;   // Delay between chained teleports



    void Start()

    {

        cam = Camera.main;

    }



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

        isTeleporting = true;



        // 1. Hide player

        if (playerSprite != null)

            playerSprite.enabled = false;



        // 🔹 Track position for chaining

        Vector3 currentPos = transform.position;



        // 🔹 Chain loop

        for (int i = 0; i < maxChains; i++)

        {

            // 2. Check for enemies along the teleport path

            RaycastHit2D[] hits = Physics2D.LinecastAll(currentPos, targetPos);

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

                    line.SetPosition(0, currentPos);  // Player’s current position

                    line.SetPosition(1, targetPos);   // Teleport destination

                }



                Destroy(beam, beamDuration);

            }



            // 4. Wait while invisible

            yield return new WaitForSeconds(disappearTime);



            // 5. Move player

            transform.position = targetPos;

            currentPos = targetPos;



            // 🔹 Look for the next enemy to chain to

            Collider2D nextEnemy = FindClosestEnemy(currentPos, chainRadius);

            if (nextEnemy != null)

            {

                targetPos = nextEnemy.transform.position;

                yield return new WaitForSeconds(chainDelay);

            }

            else

            {

                break; // no more enemies nearby → end chain

            }

        }



        // 6. Re-enable sprite

        if (playerSprite != null)

            playerSprite.enabled = true;



        // 7. End teleport state

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



    // 🔹 Helper to find the closest enemy for chain teleport

    Collider2D FindClosestEnemy(Vector3 currentPos, float radius)

    {

        Collider2D[] enemies = Physics2D.OverlapCircleAll(currentPos, radius);

        Collider2D closest = null;

        float minDist = Mathf.Infinity;



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
        //Always return a value, even if enemies found
        return closest;
    }
}