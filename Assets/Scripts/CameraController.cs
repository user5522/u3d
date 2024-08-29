using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DOTween.SetTweensCapacity(500, 50);
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue) => cam.DOFieldOfView(endValue, 0.25f);
    public void DoTilt(float zTilt) => transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    public void DoShake(float duration, float shakeIntensity) => transform.DOShakePosition(duration, shakeIntensity);
}