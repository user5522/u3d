using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(HighlightInSlowMode))]
public class SoldierEnemyController : MonoBehaviour
{
    // NavMesh and movement
    public NavMeshAgent agent;
    public float walkPointRange = 10f;
    private Vector3 walkPoint;
    private bool walkPointSet;

    // Player detection
    public Transform player;
    public float sightRange = 15f;
    public float attackRange = 10f;
    public LayerMask groundLayer, playerLayer;
    private bool playerInSightRange, playerInAttackRange;

    // Shooting
    public float timeBetweenShots = 0.5f;
    public int projectilesPerBurst = 3;
    public float burstCooldown = 2f;
    public float projectileSpeed = 20f;
    public int attackDamage = 10;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private bool canShoot = true;

    // References
    private PlayerBehaviour playerBehaviour;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerBehaviour = player.GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        // Check if player is in range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patrolling();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInSightRange && playerInAttackRange)
            AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (agent.enabled && walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (agent.enabled)
            agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Ensure the enemy doesn't move while attacking
        if (agent.enabled)
            agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (canShoot)
            StartCoroutine(ShootBurst());
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
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Vector3 direction = (player.position - firePoint.position).normalized;

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
            rb.velocity = direction * projectileSpeed;

        if (!projectile.TryGetComponent<Projectile>(out var projectileScript))
            projectileScript = projectile.AddComponent<Projectile>();

        projectileScript.damage = attackDamage;
        projectileScript.playerBehaviour = playerBehaviour;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}