using UnityEngine;

public class Grabber : MonoBehaviour
{
    [Header("Grab Settings")]
    public float grabRadius = 1.2f;
    public float grabDuration = 2.5f;
    public float grabCooldown = 10f;
    public LayerMask pushableLayer;
    public Transform grabPoint;

    private float cooldownTimer = 0f;
    private float grabTimer = 0f;

    private bool isGrabbing = false;
    private Rigidbody grabbedObject = null;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isGrabbing)
        {
            grabTimer -= Time.deltaTime;
            if (grabbedObject != null)
            {
                grabbedObject.MovePosition(grabPoint.position);
            }

            if (grabTimer <= 0f)
            {
                ReleaseObject();
            }
        }
        else if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0f)
        {
            TryGrabNearbyObject();
        }
    }

    void TryGrabNearbyObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius, pushableLayer);

        if (colliders.Length == 0) return;

        // Find the closest object
        float closestDist = float.MaxValue;
        Rigidbody closestRb = null;

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                float dist = Vector3.Distance(transform.position, rb.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestRb = rb;
                }
            }
        }

        if (closestRb != null)
        {
            grabbedObject = closestRb;
            grabbedObject.isKinematic = true;
            grabbedObject.velocity = Vector3.zero;
            grabbedObject.transform.position += Vector3.up * 0.5f; // Slight lift
            grabbedObject.transform.SetParent(grabPoint);

            isGrabbing = true;
            grabTimer = grabDuration;
            cooldownTimer = grabCooldown;

            Debug.Log("Grabbed: " + grabbedObject.name);
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.SetParent(null);
            grabbedObject.isKinematic = false;
            grabbedObject = null;
        }

        isGrabbing = false;
        Debug.Log("Released object");
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the grab radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}
