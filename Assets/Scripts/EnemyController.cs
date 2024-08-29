using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerObj;
    public LayerMask groundLayer, playerLayer;

    public float health;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public PlayerBehaviour playerBehaviour;
    public int attackDamage;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public Material defaultMaterial;
    public Material slowMotionMaterial;
    private Renderer enemyRenderer;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        playerObj = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();
        defaultMaterial = enemyRenderer.material;
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

        if (Time.timeScale < 1) enemyRenderer.material = slowMotionMaterial;
        else enemyRenderer.material = defaultMaterial;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 0.1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer)) walkPointSet = true;
    }

    private void ChasePlayer() => agent.SetDestination(playerObj.position);

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(playerObj);

        if (!alreadyAttacked)
        {
            playerBehaviour.PlayerTakeDmg(attackDamage);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() => alreadyAttacked = false;


    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) DestroyEnemy();
    }

    private void DestroyEnemy() => Destroy(gameObject);

}
