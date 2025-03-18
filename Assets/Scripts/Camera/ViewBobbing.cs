using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    [Header("Walking")]
    public float walkingEffectIntensity;
    public float walkingEffectIntensityX;
    public float walkingEffectSpeed;

    [Header("Sprinting")]
    public float sprintingEffectIntensity;
    public float sprintingEffectIntensityX;
    public float sprintingEffectSpeed;

    public PlayerController playerController;

    private float effectIntensity;
    private float effectIntensityX;
    private float effectSpeed;

    private PositionFollower followerInstance;
    private Vector3 originalOffset;
    private float sinTime;

    void Start()
    {
        followerInstance = GetComponent<PositionFollower>();
        originalOffset = followerInstance.offset;
    }

    void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Vertical"), 0f, Input.GetAxisRaw("Horizontal"));

        switch (playerController.state)
        {
            case PlayerController.MovementState.walking:
                effectIntensity = walkingEffectIntensity;
                effectIntensityX = walkingEffectIntensityX;
                effectSpeed = walkingEffectSpeed;
                break;
            case PlayerController.MovementState.sprinting:
                effectIntensity = sprintingEffectIntensity;
                effectIntensityX = sprintingEffectIntensityX;
                effectSpeed = sprintingEffectSpeed;
                break;
            case PlayerController.MovementState.wallRunningUp:
            case PlayerController.MovementState.wallRunningDown:
                effectIntensity = sprintingEffectIntensity;
                effectIntensityX = sprintingEffectIntensityX;
                effectSpeed = sprintingEffectSpeed;
                break;
            case PlayerController.MovementState.air:
                effectIntensity = 0;
                effectIntensityX = 0;
                effectSpeed = 0;
                break;
            case PlayerController.MovementState.sliding:
                effectIntensity = 0;
                effectIntensityX = 0;
                effectSpeed = 0;
                break;
            default:
                effectIntensity = walkingEffectIntensity;
                effectIntensityX = walkingEffectIntensityX;
                effectSpeed = walkingEffectSpeed;
                break;
        }

        if (inputVector.magnitude > 0f && playerController.state != PlayerController.MovementState.sliding) sinTime += Time.deltaTime * effectSpeed;
        else sinTime = 0f;

        float sinAmountY = -Mathf.Abs(Mathf.Sin(sinTime) * effectIntensity);
        Vector3 sinAmountX = effectIntensity * effectIntensityX * Mathf.Cos(sinTime) * followerInstance.transform.right;

        followerInstance.offset = new Vector3(originalOffset.x, originalOffset.y + sinAmountY, originalOffset.z);
        followerInstance.offset += sinAmountX;
    }
}
