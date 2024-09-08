using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class Blast : MonoBehaviour
{
    [Header("Blast Configuration")]
    public float speed;
    public float blastForce;
    public float maxRadius;
    public float height;

    private LineRenderer lr;
    private float radius;
    private int pointCount = 30;
    private float pointsAngle;
    private Vector3[] positions = new Vector3[30];
    private Collider[] col;
    private bool isExpanding = true;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = pointCount;
        FindPoints();
    }

    void FindPoints()
    {
        pointsAngle = 360f / pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            float angle = pointsAngle * i * Mathf.Deg2Rad;
            positions[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        }
        positions[pointCount - 1] = positions[0];
    }

    void FixedUpdate()
    {
        if (isExpanding)
        {
            if (radius <= maxRadius)
            {
                ApplyForce();
                SetPositions();
                radius += speed;
                lr.widthMultiplier = (maxRadius - radius) / maxRadius;
            }
            else
            {
                isExpanding = false;
                Destroy(gameObject);
            }
        }
    }

    void SetPositions()
    {
        for (int i = 0; i < pointCount; i++)
        {
            lr.SetPosition(i, positions[i] * radius);
        }
    }

    void ApplyForce()
    {
        Vector3 halfExtents = new Vector3(radius, height / 2f, radius);
        col = Physics.OverlapBox(transform.position, halfExtents, Quaternion.identity);
        foreach (Collider c in col)
        {
            Vector3 direction = (c.transform.position - transform.position).normalized;
            Vector3 force = direction * blastForce;

            if (c.TryGetComponent(out KnockbackHandler knockbackHandler))
            {
                knockbackHandler.ApplyKnockback(force);
            }
            else if (c.TryGetComponent(out Rigidbody colRb))
            {
                colRb.AddForce(force, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        DrawWireCylinder(Vector3.zero, maxRadius, height);
    }

    void DrawWireCylinder(Vector3 center, float radius, float height)
    {
        float halfHeight = height / 2f;
        DrawCircle(center + Vector3.up * halfHeight, radius);
        DrawCircle(center - Vector3.up * halfHeight, radius);
        for (int i = 0; i < 4; i++)
        {
            float angle = i * Mathf.PI / 2;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(center + pos + Vector3.up * halfHeight, center + pos - Vector3.up * halfHeight);
        }
    }

    void DrawCircle(Vector3 center, float radius)
    {
        int segments = 32;
        Vector3 prevPos = center + new Vector3(radius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 pos = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
        }
    }
}