using UnityEngine;

public class ShooterEnemy : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Direction")]
    [SerializeField] private bool shootRight = true;

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
            Vector2 direction = shootRight ? Vector2.right : Vector2.left;
            projectileScript.Initialize(direction, projectileSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 direction = shootRight ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(firePoint.position, direction * 2f);
        }
    }
}
