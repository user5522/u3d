using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class KnockbackHandler : MonoBehaviour
{
    public float knockbackThreshold = 0.05f;
    public float recoveryDelay = 0.5f;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private bool isKnockedBack = false;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (isKnockedBack) CheckKnockbackEnd();
    }

    public void ApplyKnockback(Vector3 force)
    {
        if (!isKnockedBack)
        {
            isKnockedBack = true;
            agent.enabled = false;
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
            Invoke(nameof(StartRecovery), recoveryDelay);
        }
    }

    private void StartRecovery()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void CheckKnockbackEnd()
    {
        if (rb.velocity.magnitude <= knockbackThreshold) EndKnockback();
    }

    private void EndKnockback()
    {
        isKnockedBack = false;
        rb.isKinematic = true;
        agent.enabled = true;
        agent.Warp(transform.position);
    }
}