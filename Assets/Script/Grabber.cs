using UnityEngine;

public class Grabber : MonoBehaviour
{
    public float grabDuration = 2.5f;
    public float grabCooldown = 10f;
    public float grabRange = 3f;
    public LayerMask pushableLayer;
    public Transform grabPoint; // Assign this in Inspector

    private bool isGrabbing = false;
    private bool isOnCooldown = false;
    private Rigidbody grabbedObject = null;
    private float grabTimer = 0f;
    private float cooldownTimer = 0f;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
        HandleGrabTimer();
        HandleCooldown();
        FollowGrabPoint();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1) && !isGrabbing && !isOnCooldown)
        {
            TryGrabObject();
        }
    }

    void TryGrabObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange, pushableLayer))
        {
            Rigidbody targetRb = hit.collider.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                grabbedObject = targetRb;

                // Lift object slightly (optional visual feedback)
                Vector3 lifted = hit.point + Vector3.up * 0.5f;
                grabbedObject.MovePosition(lifted);

                grabbedObject.velocity = Vector3.zero;
                grabbedObject.isKinematic = true;

                // Parent to grab point
                grabbedObject.transform.SetParent(grabPoint);

                isGrabbing = true;
                grabTimer = grabDuration;
            }
        }
    }

    void FollowGrabPoint()
    {
        if (isGrabbing && grabbedObject != null)
        {
            grabbedObject.MovePosition(grabPoint.position);
        }
    }

    void HandleGrabTimer()
    {
        if (isGrabbing)
        {
            grabTimer -= Time.deltaTime;
            if (grabTimer <= 0f)
            {
                ReleaseObject();
            }
        }
    }

    void HandleCooldown()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
            }
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
        isOnCooldown = true;
        cooldownTimer = grabCooldown;
    }
}
