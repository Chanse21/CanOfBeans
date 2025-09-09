using UnityEngine;
using System;


public class PlayerTele : MonoBehaviour
{
      
    
    private Camera cam;

    [Header("Teleport")]
    public float PlayerRadius = 5f; 
    public float teleportCooldown = 3f;  // time between teleports
    private float nextTeleportTime = 0f; // when player can teleport again
    
    public static event Action<float> OnTeleportUsed;

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
            TryTeleport();
            nextTeleportTime = Time.time + teleportCooldown;

            // Tell UI how long cooldown is
            OnTeleportUsed?.Invoke(teleportCooldown);
        }
        
    }
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

}
