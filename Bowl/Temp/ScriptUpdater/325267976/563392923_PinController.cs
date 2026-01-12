/*
 * PinController.cs
 * Bowling Game - Individual Pin Controller
 *
 * Manages individual bowling pin behavior and state detection.
 * Determines if a pin is standing or knocked down based on rotation and position.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;

public class PinController : MonoBehaviour
{
    [Header("Fall Detection Settings")]
    [SerializeField] private float rotationThreshold = 45f; // Pin is down if tilted more than this angle
    [SerializeField] private float heightThreshold = 0.05f; // Pin is down if below this Y position
    [SerializeField] private float velocityThreshold = 0.1f; // Pin must be stopped to check state

    [Header("References")]
    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    [Header("State")]
    private bool isKnockedDown = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"PinController ({gameObject.name}): Rigidbody component not found!");
        }

        // Store initial position and rotation for reset
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        // Continuously check if pin should be considered knocked down
        if (!isKnockedDown)
        {
            isKnockedDown = !IsStanding();
        }
    }

    /// <summary>
    /// Checks if the pin is currently standing upright
    /// </summary>
    /// <returns>True if pin is standing, false if knocked down</returns>
    public bool IsStanding()
    {
        // Check if pin is below height threshold
        if (transform.position.y < heightThreshold)
        {
            return false;
        }

        // Check rotation on X and Z axes (pin should be roughly vertical)
        Vector3 rotation = transform.eulerAngles;

        // Normalize angles to -180 to 180 range
        float xAngle = rotation.x > 180 ? rotation.x - 360 : rotation.x;
        float zAngle = rotation.z > 180 ? rotation.z - 360 : rotation.z;

        // If pin is tilted beyond threshold on either axis, it's down
        if (Mathf.Abs(xAngle) > rotationThreshold || Mathf.Abs(zAngle) > rotationThreshold)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the pin has stopped moving
    /// </summary>
    /// <returns>True if pin velocity is below threshold</returns>
    public bool IsStopped()
    {
        if (rb == null) return true;
        return rb.linearVelocity.magnitude < velocityThreshold && rb.angularVelocity.magnitude < velocityThreshold;
    }

    /// <summary>
    /// Resets the pin to its starting position and rotation
    /// </summary>
    public void ResetPin()
    {
        if (rb == null) return;

        transform.position = startPosition;
        transform.rotation = startRotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isKnockedDown = false;

        Debug.Log($"PinController ({gameObject.name}): Pin reset");
    }

    /// <summary>
    /// Gets whether this pin has been knocked down
    /// </summary>
    public bool IsKnockedDown()
    {
        return isKnockedDown;
    }

    /// <summary>
    /// Gets the current tilt angle of the pin (for debugging)
    /// </summary>
    public float GetTiltAngle()
    {
        Vector3 rotation = transform.eulerAngles;
        float xAngle = rotation.x > 180 ? rotation.x - 360 : rotation.x;
        float zAngle = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        return Mathf.Max(Mathf.Abs(xAngle), Mathf.Abs(zAngle));
    }

    // Visualize pin state in Scene view for debugging
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = IsStanding() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.1f);
        }
    }
}
