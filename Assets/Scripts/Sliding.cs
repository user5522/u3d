using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObject;
    public CameraController cam;
    private Transform cameraTransform;
    private PlayerController playerController;
    private Rigidbody rb;

    [Header("Sliding")]
    public float slideForce;
    public float slideYscale;
    public float rotationSpeed;

    [Header("Ground Slam")]
    public float groundSlamForce;
    public float shakeDuration;
    public float shakeIntensity;

    private float startYscale;
    private Vector3 slideDirection;
    private bool isGroundSlamming = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        startYscale = playerObject.localScale.y;
        playerController.sliding = false;
        cameraTransform = cam.transform;
        cam.DoTilt(0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (playerController.grounded && !playerController.sliding) StartSlide();
            else if (!playerController.grounded) GroundSlam();
        }

        if ((Input.GetKeyUp(KeyCode.LeftControl) && playerController.sliding)

            || (!playerController.grounded && playerController.sliding))
            StopSlide();
        if (CanRestorePlayerHeight() && !playerController.sliding) RestorePlayerHeight();

        if (playerController.sliding) HandleSlidingMovement();

        if (isGroundSlamming && playerController.grounded)
        {
            cam.DoShake(shakeDuration, shakeIntensity);
            isGroundSlamming = false;
        }
    }

    void FixedUpdate()
    {
        if (playerController.sliding) SlidingMovement();
    }

    private void StartSlide()
    {
        playerController.sliding = true;
        playerObject.localScale = new Vector3(playerObject.localScale.x, slideYscale, playerObject.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        slideDirection = orientation.forward;
        if (rb.velocity.y >= 0) slideDirection.y = 0;
    }

    private void GroundSlam()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.down * groundSlamForce, ForceMode.Impulse);
        isGroundSlamming = true;
    }

    private void HandleSlidingMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != 0)
        {
            if (horizontalInput < 0) cam.DoTilt(5f);
            if (horizontalInput > 0) cam.DoTilt(-5f);
            cam.DoTilt(0f);
            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();
            Vector3 turnDirection = horizontalInput == 0 ? orientation.forward : horizontalInput * cameraRight;
            slideDirection = Vector3.RotateTowards(slideDirection, turnDirection, rotationSpeed * Time.deltaTime, 0f);
            slideDirection.Normalize();
        }
    }

    private void SlidingMovement()
    {
        Vector3 forceToApply;
        if (playerController.OnSlope())
            forceToApply = playerController.GetSlopeMoveDirection(slideDirection) * slideForce;
        else forceToApply = slideDirection * slideForce;
        rb.AddForce(forceToApply, ForceMode.Force);
        playerObject.forward = Vector3.Slerp(playerObject.forward, slideDirection, Time.deltaTime * 10f);
    }

    private void StopSlide()
    {
        playerController.sliding = false;
        cam.DoTilt(0f);
    }

    public Vector3 GetSlideDirection() => slideDirection;

    private bool CanRestorePlayerHeight()
    {
        Vector3 playerCenter = transform.position + (Vector3.up * (playerObject.localScale.y / 2f));
        float playerHalfNormalHeight = startYscale / 2f;

        RaycastHit hit;
        if (Physics.Raycast(playerCenter, Vector3.up, out hit, playerHalfNormalHeight + 0.1f, playerController.groundLayer))
            return hit.distance >= startYscale;
        return true;
    }

    private void RestorePlayerHeight() =>
        playerObject.localScale = new Vector3(playerObject.localScale.x, startYscale, playerObject.localScale.z);
}