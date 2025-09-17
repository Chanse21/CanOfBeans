using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    public Rigidbody rb;
    [Header("Melee Attack")]
    // Attack radius
    public float PlayerRadius = 3f;
    public float speed = 1f;

    void Start()
    {
       // rb = GetComponent<Rigidbody2D>
    }

    
    void Update()
    {
        
    }
}
