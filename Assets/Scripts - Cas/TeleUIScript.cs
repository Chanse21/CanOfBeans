using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeleUIScript : MonoBehaviour
{
    [Header("Teleport Cooldown UI")]
    public Slider cooldownBar;          
    public TextMeshProUGUI cooldownText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float cooldownDuration;
    private float cooldownRemaining;
    
    void OnEnable()
    {
        // Subscribe to player event
        PlayerTele.OnTeleportUsed += StartCooldown;
    }

    void OnDisable()
    {
        // Unsubscribe when disabled
        PlayerTele.OnTeleportUsed -= StartCooldown;
    }


    // Update is called once per frame
    void Update()
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
            float progress = 1f - (cooldownRemaining / cooldownDuration);

            if (cooldownBar != null)
                cooldownBar.value = progress;

            if (cooldownText != null)
                cooldownText.text = $"Teleport ready in {cooldownRemaining:F1}s";
        }
        else
        {
            if (cooldownBar != null)
                cooldownBar.value = 1f;

            if (cooldownText != null)
                cooldownText.text = "Teleport ready!";
        }
    }

    void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownRemaining = duration;

        if (cooldownBar != null)
            cooldownBar.value = 0f;
    }

    
    
}
