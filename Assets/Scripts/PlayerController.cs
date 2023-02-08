using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float runSpeed = 20.0f;
    public float jumpForce = 10.0f;
    public float normalFOV = 60f;
    public float runningFOV = 75f;
    public float zoomFOV = 30f;
    public float FOVLerpSpeed = 8f;
    private Rigidbody rb;
    private Vector3 movement;
    private bool isGrounded = true;

    public Transform cameraTransform;
    private Camera mainCamera;

    void Start()
    {
        Application.targetFrameRate = -1;
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward = forward.normalized;

        movement = (forward * vertical + cameraTransform.right * horizontal);
        if (Input.GetKey(KeyCode.LeftControl))
        {
            movement = movement.normalized * runSpeed * Time.deltaTime;
        }
        else
        {
            movement = movement.normalized * speed * Time.deltaTime;
        }

        if (Input.GetButton("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // FOV shittery
        // running FOV
        float targetFOV = normalFOV;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            targetFOV = runningFOV;
        }
        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * FOVLerpSpeed
        );
        // zooming FOV
        if (Input.GetKey(KeyCode.C))
        {
            targetFOV = zoomFOV;
        }
        mainCamera.fieldOfView = Mathf.Lerp(
            mainCamera.fieldOfView,
            targetFOV,
            Time.deltaTime * FOVLerpSpeed
        );
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
