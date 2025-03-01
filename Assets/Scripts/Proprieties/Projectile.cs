using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public PlayerBehaviour playerBehaviour;
    public float deflectionSpeed;
    public bool isDeflected = false;
    public int damageMultiplier;
    public float lifetime = 5f;

    private EnemyHealth enemyHealth;
    private Rigidbody rb;
    private float lifetimeTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lifetimeTimer = 0f;
        gameObject.layer = LayerMask.NameToLayer("Projectile");
    }

    void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerBehaviour != null) playerBehaviour.PlayerTakeDmg(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy") && isDeflected)
        {
            enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage * damageMultiplier);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
            Destroy(gameObject);
    }

    public void Deflect(Vector3 deflectionDirection)
    {
        if (!isDeflected)
        {
            isDeflected = true;
            rb.linearVelocity = deflectionDirection * deflectionSpeed;
            transform.forward = deflectionDirection;
        }
    }
}