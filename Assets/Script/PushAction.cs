using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAction : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 8f;
    [SerializeField] private float pushRange = 1.2f;
    [SerializeField] private float pushRadius = 0.6f;
    [SerializeField] private LayerMask pushableLayers;
    [SerializeField] private float cooldown = 1f;

    private float lastPushTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            TryPush();
        }
    }

    private void TryPush()
    {
        if (Time.time - lastPushTime < cooldown) return;

        Vector3 pushOrigin = transform.position + transform.forward * 0.5f;
        pushOrigin.y += 0.5f; // Push from chest height

        RaycastHit[] hits = Physics.SphereCastAll(
            pushOrigin,
            pushRadius,
            transform.forward,
            pushRange,
            pushableLayers
        );

        foreach (RaycastHit hit in hits)
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = (hit.point - pushOrigin).normalized;
                pushDirection.y = 0.2f; // Slight upward angle
                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }

        lastPushTime = Time.time;
    }

    // Visualize in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 pushOrigin = transform.position + transform.forward * 0.5f;
        pushOrigin.y += 0.5f;
        Gizmos.DrawSphere(pushOrigin + transform.forward * pushRange/2, pushRadius);
    }
}