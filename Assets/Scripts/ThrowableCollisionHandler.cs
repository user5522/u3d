using UnityEngine;

public class ThrowableCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(10);
            Destroy(gameObject);
        }
    }
}