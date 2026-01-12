/*
 * BallController.cs
 * Bowling Game - Ball Physics Controller
 *
 * Handles bowling ball physics, movement, launching, and collision detection.
 * Manages ball rigidbody, applies forces from swipe input, and detects when ball stops moving.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;

    [Header("Settings")]
    [SerializeField] private float velocityThreshold = 0.1f; // Ball is considered stopped below this velocity
    [SerializeField] private Vector3 startPosition = new Vector3(0f, 0.11f, -8f);

    [Header("Launch Settings")]
    [SerializeField] private float forceMultiplier = 1.0f; // Additional multiplier for fine-tuning
    [SerializeField] private float maxForce = 25f; // Maximum force that can be applied

    private bool isLaunched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("BallController: Rigidbody component not found!");
        }
    }

    void Start()
    {
        ResetBall();
    }

    /// <summary>
    /// Launches the ball with the specified force vector
    /// </summary>
    /// <param name="force">Force vector to apply to the ball</param>
    public void Launch(Vector3 force)
    {
        if (rb == null) return;

        // Clamp force magnitude
        if (force.magnitude > maxForce)
        {
            force = force.normalized * maxForce;
        }

        // Apply multiplier
        force *= forceMultiplier;

        // Reset velocity before launching
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Apply force
        rb.AddForce(force, ForceMode.Impulse);
        isLaunched = true;

        Debug.Log($"BallController: Ball launched with force {force.magnitude:F2}N");
    }

    /// <summary>
    /// Checks if the ball is currently moving
    /// </summary>
    /// <returns>True if ball velocity is above threshold</returns>
    public bool IsMoving()
    {
        if (rb == null) return false;
        return rb.linearVelocity.magnitude > velocityThreshold;
    }

    /// <summary>
    /// Resets the ball to starting position and stops all movement
    /// </summary>
    public void ResetBall()
    {
        if (rb == null) return;

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isLaunched = false;

        Debug.Log("BallController: Ball reset to start position");
    }

    /// <summary>
    /// Detects when ball enters gutter trigger zone
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gutter"))
        {
            Debug.Log("BallController: Ball entered gutter!");
            // GameManager will handle the reset through its state machine
        }
    }

    /// <summary>
    /// Gets current ball velocity for debugging/monitoring
    /// </summary>
    public float GetVelocityMagnitude()
    {
        return rb != null ? rb.linearVelocity.magnitude : 0f;
    }

    /// <summary>
    /// Checks if ball has been launched
    /// </summary>
    public bool IsLaunched()
    {
        return isLaunched;
    }

    // Visualize velocity in Scene view for debugging
    void OnDrawGizmos()
    {
        if (rb != null && Application.isPlaying)
        {
            Gizmos.color = IsMoving() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.15f);

            if (rb.linearVelocity.magnitude > 0.01f)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, rb.linearVelocity.normalized * 0.5f);
            }
        }
    }
}
