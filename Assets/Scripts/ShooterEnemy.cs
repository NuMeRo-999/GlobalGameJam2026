using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float projectileSpeed = 10f;

    [SerializeField] private Animator animator;


    private float nextFireTime;


    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            // Shoot in the direction the enemy is facing (based on rotation)
            Vector2 direction = transform.up;
            projectileScript.Initialize(direction, projectileSpeed);
            animator.SetTrigger("Shoot");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, transform.up * 2f);
        }
    }
}
