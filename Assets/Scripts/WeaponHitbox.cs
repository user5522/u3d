using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public WeaponController weaponController;
    public Camera cam;
    public float deflectionWindow = 0.1f;

    [Header("Hitbox Settings")]
    public Vector3 hitboxCenter = Vector3.zero;
    public Vector3 hitboxSize = Vector3.one;

    private bool canDeflect = false;
    private float deflectionTimer = 0f;

    void Update()
    {
        if (canDeflect)
        {
            deflectionTimer += Time.deltaTime;
            if (deflectionTimer > deflectionWindow) canDeflect = false;
        }
    }

    public void StartDeflectionWindow()
    {
        canDeflect = true;
        deflectionTimer = 0f;
    }

    void FixedUpdate()
    {
        if (canDeflect)
        {
            Vector3 worldHitboxCenter = transform.TransformPoint(hitboxCenter);
            Quaternion worldHitboxRotation = transform.rotation;

            Collider[] hitColliders = Physics.OverlapBox(worldHitboxCenter, hitboxSize / 2, worldHitboxRotation, LayerMask.GetMask("Projectile"));
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<Projectile>(out var projectile))
                {
                    Vector3 deflectionDirection = cam.transform.forward;
                    projectile.Deflect(deflectionDirection);
                    weaponController.OnSuccessfulDeflection();
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(hitboxCenter, hitboxSize);
    }
}