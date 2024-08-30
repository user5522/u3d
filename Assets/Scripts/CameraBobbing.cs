using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    [Header("References")]
    public Transform cameraHolder;
    public PlayerController playerController;
    public Rigidbody rb;

    [Header("Bobbing Configuration")]
    public float walkAmplitude;
    public float walkFrequency;
    public float sprintAmplitude;
    public float sprintFrequency;

    private float amplitude;
    private float frequency;
    private Vector3 startPos;
    private Vector3 bobOffset;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        switch (playerController.state)
        {
            case PlayerController.MovementState.walking:
                amplitude = walkAmplitude;
                frequency = walkFrequency;
                break;
            case PlayerController.MovementState.sprinting:
                amplitude = sprintAmplitude;
                frequency = sprintFrequency;
                break;
            default:
                amplitude = 0f;
                frequency = 0f;
                break;
        }

        if (playerController.grounded && playerController.state != PlayerController.MovementState.sliding)
        {
            CheckMotion();
        }
        else
        {
            ResetBob();
        }

        ApplyBob();
    }

    private Vector3 FootstepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = velocity.magnitude;
        if (speed < 0.1f)
        {
            ResetBob();
            return;
        }
        bobOffset = FootstepMotion();
    }

    private void ApplyBob()
    {
        transform.localPosition = startPos + bobOffset;
    }

    private void ResetBob()
    {
        bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, 5f * Time.deltaTime);
    }
}