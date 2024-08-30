using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public PlayerBehaviour playerBehaviour;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerBehaviour != null) playerBehaviour.PlayerTakeDmg(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground")) Destroy(gameObject);
    }
}