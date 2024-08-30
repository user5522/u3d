using UnityEngine;

public class PickupObj : MonoBehaviour
{
    public float pickupRange = 2f;
    public float throwForce = 20f;
    public Transform holdPoint;
    public Transform cam;

    private GameObject heldObject;
    private Rigidbody heldObjectRb;

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
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, pickupRange))
        {
            GameObject objectHit = hit.collider.gameObject;
            if (objectHit.CompareTag("Throwable")) PickupObject(objectHit);
        }
    }

    private void PickupObject(GameObject obj)
    {
        heldObject = obj;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        heldObjectRb.isKinematic = true;
        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void MoveHeldObject()
    {
        heldObject.transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);
        // i couldn't figure another way to fix the throwable's collider fighting 
        // with the player's collider so i just disable it when it's picked up
        heldObject.GetComponent<BoxCollider>().enabled = false;
    }

    private void ThrowObject()
    {
        heldObject.GetComponent<BoxCollider>().enabled = true;
        heldObjectRb.isKinematic = false;
        Vector3 forceDirection = cam.transform.forward;
        if (Physics.Raycast(cam.transform.position, cam.forward, out RaycastHit hit, 500f))
            forceDirection = (hit.point - holdPoint.position).normalized;

        heldObjectRb.AddForce(forceDirection * throwForce, ForceMode.Impulse);
        heldObject.transform.SetParent(null);
        heldObject = null;
    }
}