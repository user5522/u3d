using UnityEngine;

public class PickupObj : MonoBehaviour
{
    public float pickupRange = 2f;
    public float throwForce = 20f;
    public Transform holdPoint;
    public Transform cam;

    private GameObject heldObject;
    private Rigidbody heldObjectRb;

    private Collider heldObjectCollider;
    private Collider heldObjectTriggerCollider;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null) TryPickupObject();
            else ThrowObject();
        }
        if (heldObject != null) MoveHeldObject();
    }

    private void TryPickupObject()
    {
        RaycastHit[] hits = Physics.SphereCastAll(cam.transform.position, 0.5f, cam.transform.forward, pickupRange);
        foreach (RaycastHit hit in hits)
        {
            GameObject objectHit = hit.collider.gameObject;
            if (objectHit.CompareTag("Throwable"))
            {
                PickupObject(objectHit);
                break;
            }
        }
    }

    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        heldObjectRb.isKinematic = true;

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        heldObjectCollider = heldObject.GetComponent<Collider>();
        heldObjectCollider.enabled = false;

        heldObjectTriggerCollider = heldObject.GetComponent<SphereCollider>();
        heldObjectTriggerCollider.enabled = false;
    }

    private void MoveHeldObject() => heldObject.transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);

    private void ThrowObject()
    {
        heldObjectCollider.enabled = true;
        heldObjectTriggerCollider.enabled = true;

        heldObjectRb.isKinematic = false;
        Vector3 forceDirection = cam.transform.forward;

        if (Physics.Raycast(cam.transform.position, cam.forward, out RaycastHit hit, 500f))
            forceDirection = (hit.point - holdPoint.position).normalized;

        heldObjectRb.AddForce(forceDirection * throwForce, ForceMode.Impulse);
        heldObject.transform.SetParent(null);
        heldObject = null;
    }

    public bool IsPickingUpObject() => heldObject != null;
}
