using UnityEngine;

public class TeleportCusor : MonoBehaviour
{
    [Header("References")]
    public Transform player;           // Reference to the player's transform
    public Camera mainCamera;          // Reference to the main camera
    public SpriteRenderer cursorSprite; // SpriteRenderer for cursor
    public float followSmoothness = 10f; // How quickly cursor follows target position

    [Header("Settings")]
    public float teleportRadius = 5f;   // Match this to PlayerTele.PlayerRadius

    private Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if (mainCamera == null)
            mainCamera = Camera.main;

        Cursor.visible = false; // Hide system cursor 
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Get mouse position in world space
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Find direction and distance from player to mouse
        Vector3 offset = mouseWorld - player.position;
        float distance = offset.magnitude;

        // Clamp to teleport radius
         if (distance > teleportRadius)
        {
            offset = offset.normalized * teleportRadius;
        }

        // Calculate target position
        targetPosition = player.position + offset;

        // Smoothly move cursor
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSmoothness * Time.deltaTime);
    }
}
