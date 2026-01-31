using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;

    private Vector2 direction;
    private float speed;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask collisionLayers;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        // Rotar el sprite según la dirección
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Si no tiene Rigidbody, mover manualmente
        if (rb == null)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        // Verificar si colisionó con el jugador
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Matar al jugador
            playerHealth.Die();
            Debug.Log("Projectile hit player!");
        }

        // Destruir el proyectil al colisionar con cualquier cosa
        Destroy(gameObject);
    }
}
