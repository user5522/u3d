using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(HighlightInSlowMode))]
public class ShooterEnemyController : MonoBehaviour
{
    public float detectionRadius;
    public float timeBetweenShots;
    public int projectilesPerBurst;
    public float burstCooldown;
    public float projectileSpeed;
    public int attackDamage;
    public GameObject projectilePrefab;
    public GameObject player;

    private Transform playerTransform;
    private PlayerBehaviour playerBehaviour;
    private bool isPlayerInRange = false;
    private bool canShoot = true;

    void Start()
    {
        playerTransform = player.transform;
        playerBehaviour = player.GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        CheckPlayerInRange();
        if (isPlayerInRange)
        {
            gameObject.transform.LookAt(playerTransform);
            if (canShoot) StartCoroutine(ShootBurst());
        }
    }

    void CheckPlayerInRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerInRange = distanceToPlayer <= detectionRadius;
    }

    IEnumerator ShootBurst()
    {
        canShoot = false;
        for (int i = 0; i < projectilesPerBurst; i++)
        {
            ShootProjectile();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(burstCooldown);
        canShoot = true;
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        if (projectile.TryGetComponent<Rigidbody>(out var rb)) rb.velocity = direction * projectileSpeed;

        // idk why but sometimes it doesn't have the script
        if (!projectile.TryGetComponent<Projectile>(out var projectileScript)) projectileScript = projectile.AddComponent<Projectile>();

        projectileScript.damage = attackDamage;
        projectileScript.playerBehaviour = playerBehaviour;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

