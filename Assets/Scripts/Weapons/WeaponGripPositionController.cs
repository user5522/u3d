using UnityEngine;

public class WeaponGripPositionController : MonoBehaviour
{
    public WeaponController weaponController;
#nullable enable
    public PickupObj? pickupObj;
    public Transform? pickingUpPosition;
#nullable disable
    public Transform drawnPosition;
    public Transform withdrawnPosition;
    public float transitionSpeed = 5f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start() => UpdateTargetTransform(weaponController.IsWeaponDrawn());

    private void Update()
    {
        UpdateTargetTransform(weaponController.IsWeaponDrawn());
        float interpolationFactor = transitionSpeed > 0 ? Time.deltaTime * transitionSpeed : 1f;
        transform.SetPositionAndRotation(
            Vector3.Lerp(transform.position, targetPosition, interpolationFactor),
            Quaternion.Slerp(transform.rotation, targetRotation, interpolationFactor)
        );
    }

    private void UpdateTargetTransform(bool isDrawn)
    {
        if (pickupObj != null && pickupObj.IsPickingUpObject())
        {
            targetPosition = pickingUpPosition.position;
            targetRotation = pickingUpPosition.rotation;
        }
        else if (isDrawn)
        {
            targetPosition = drawnPosition.position;
            targetRotation = drawnPosition.rotation;
        }
        else
        {
            targetPosition = withdrawnPosition.position;
            targetRotation = withdrawnPosition.rotation;
        }
    }
}