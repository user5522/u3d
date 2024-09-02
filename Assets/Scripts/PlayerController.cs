using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    public CameraController cam;
    private Sliding slidingScript;

    [Header("Movement")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;

    private float desiredMovementSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Ground")]
    public float playerHeight;
    public LayerMask groundLayer;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;
    private float horizontalInput;
    private float verticalInput;

    private Vector3 movementDirection;

    [HideInInspector]
    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        wallRunning,
        air,
    }

    [HideInInspector] public MovementState state;

    [HideInInspector] public bool sliding;
    [HideInInspector] public bool wallrunning;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        slidingScript = GetComponent<Sliding>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, groundLayer);
        MovementInput();
        SpeedControl();
        StateHandler();

        rb.drag = grounded ? groundDrag : 0f;
    }

    void FixedUpdate() => Move();

    public float GetSlopeAngle()
    {
        if (OnSlope()) return Vector3.Angle(Vector3.up, slopeHit.normal);
        return 0f;
    }

    private void MovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            Jump();
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        cam.DoFov(80f);
        if (wallrunning)
        {
            state = MovementState.wallRunning;
            desiredMovementSpeed = wallrunSpeed;
        }
        else if (sliding)
        {
            state = MovementState.sliding;
            float slopeAngle = GetSlopeAngle();
            if (slopeAngle > 0 && slopeAngle < maxSlopeAngle)
                desiredMovementSpeed = Mathf.Lerp(walkSpeed, slideSpeed, slopeAngle / maxSlopeAngle);
            else desiredMovementSpeed = sprintSpeed;
        }
        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            desiredMovementSpeed = sprintSpeed;
            if (rb.velocity.magnitude != 0) cam.DoFov(90f);
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMovementSpeed = walkSpeed;
        }
        else state = MovementState.air;

        if (Mathf.Abs(desiredMovementSpeed - lastDesiredMoveSpeed) > 8 && movementSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else movementSpeed = desiredMovementSpeed;

        lastDesiredMoveSpeed = desiredMovementSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMovementSpeed - movementSpeed);
        float startValue = movementSpeed;

        while (time < difference)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMovementSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1f + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else time += Time.deltaTime * speedIncreaseMultiplier;
            yield return null;
        }

        movementSpeed = desiredMovementSpeed;
    }

    private void Move()
    {
        if (sliding)
        {
            Vector3 slideDir = slidingScript.GetSlideDirection();

            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection(slideDir) * movementSpeed * 20f, ForceMode.Force);
                if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else rb.AddForce(10f * movementSpeed * slideDir, ForceMode.Force);
        }
        else
        {
            movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(40f * movementSpeed * GetSlopeMoveDirection(movementDirection), ForceMode.Force);
                if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else if (grounded) rb.AddForce(10f * movementSpeed * movementDirection, ForceMode.Force);
            else if (!grounded) rb.AddForce(10f * airMultiplier * movementSpeed * movementDirection, ForceMode.Force);
        }

        // turn off gravity when on slope to avoid sliding off the slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > movementSpeed) rb.velocity = rb.velocity.normalized * movementSpeed;
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVelocity.magnitude > movementSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * .5f + .3f, groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction) => Vector3.ProjectOnPlane(direction, slopeHit.normal);
}
