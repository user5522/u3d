using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private GameObject grabbedObject;
    private Rigidbody grabbedObjectRigidbody;
    public int forceMultiplier;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbedObject == null)
            {
                Grab();
            }
            else
            {
                Throw();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("grab"))
        {
            grabbedObject = other.gameObject;
            grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == grabbedObject)
        {
            grabbedObject = null;
        }
    }

    private void Grab()
    {
        if (grabbedObject == null)
        {
            return;
        }

        grabbedObjectRigidbody.isKinematic = true;
        grabbedObject.transform.SetParent(transform);
    }

    private void Throw()
    {
        if (!IsInvoking("ThrowCoroutine"))
        {
            StartCoroutine(ThrowCoroutine());
        }
    }

    private IEnumerator ThrowCoroutine()
    {
        grabbedObject.transform.SetParent(null);
        yield return new WaitForSeconds(0.1f);
        grabbedObjectRigidbody.isKinematic = false;
        grabbedObjectRigidbody.AddForce(transform.forward * forceMultiplier);
        grabbedObject = null;
    }
}
