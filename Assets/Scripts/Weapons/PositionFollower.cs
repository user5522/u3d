using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPosition = targetTransform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}