using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Camera Follow Settings")]
    public float smoothTime = 0.3f; // Default lag time
    public float teleportDelay = 0.1f; // Length of pause after action
    public float maxOffSet = 3f; // Max distance player can move from the camera

    private Vector3 velocity = Vector3.zero;
    private Vector3 offset;
    private bool isSuspended = false;
    
    void Start()
    {
        if (target != null)
            offset = transform.position - target.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        if(!isSuspended)
        {

            Vector3 targetPos = target.position + offset;
            targetPos.z = transform.position.z;

            float dynamicSmooth = smoothTime;
            // Measure how far the camera is from where it should be
            float distance = Vector3.Distance(transform.position, targetPos);

            if (distance > maxOffSet)
            {
                // Reacts faster if target is too far
                dynamicSmooth = smoothTime / 2f; 
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dynamicSmooth);
        }
    }
    // Call this method to pause camera movement
    public void SuspendCamera(float duration)
    {
        if (!isSuspended)
            StartCoroutine(SuspendCoroutine(duration));
    }
    // Coroutine handles the timed suspension of camera following
    private System.Collections.IEnumerator SuspendCoroutine(float duration)
    {
        isSuspended = true; // Stop tracking target
        yield return new WaitForSeconds(duration); //Wait for x seconds 
        isSuspended = false; // Continue tracking
    }
 
}
