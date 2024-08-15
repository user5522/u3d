using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObject;
    private Rigidbody rb;
    private PlayerController playerController;

    [Header("Sliding")]
    public float slideForce;

    public float slideYscale;
    private float startYscale;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        startYscale = playerObject.localScale.y;
        playerController.sliding = false;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftControl) && (horizontalInput != 0 || verticalInput != 0)) StartSlide();
        if (Input.GetKeyUp(KeyCode.LeftControl)) StopSlide();
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
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!playerController.OnSlope() || rb.velocity.y > -0.1f)
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
        else
            rb.AddForce(playerController.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
    }


    private void StopSlide()
    {
        playerController.sliding = false;
        playerObject.localScale = new Vector3(playerObject.localScale.x, startYscale, playerObject.localScale.z);
    }
}