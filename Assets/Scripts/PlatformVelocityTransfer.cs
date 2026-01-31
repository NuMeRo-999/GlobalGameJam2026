using UnityEngine;

public class PlatformVelocityTransfer : MonoBehaviour
{
    private Rigidbody2D platformRb;
    private Vector2 lastPlatformPosition;
    
    private void Awake()
    {
        platformRb = GetComponent<Rigidbody2D>();
        lastPlatformPosition = platformRb.position;
    }
    
    private void FixedUpdate()
    {
        lastPlatformPosition = platformRb.position;
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // Calculate platform movement
                Vector2 platformMovement = (Vector2)platformRb.position - lastPlatformPosition;
                
                // Apply platform movement to player
                playerRb.linearVelocity += platformMovement / Time.fixedDeltaTime;
            }
        }
    }
}

