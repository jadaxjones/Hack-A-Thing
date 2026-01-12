/*
 * LaneController.cs
 * Bowling Game - Lane Manager
 *
 * Manages bowling lane boundaries, gutters, and bumpers.
 * Handles bumper toggle functionality for accessibility.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;

public class LaneController : MonoBehaviour
{
    [Header("Lane References")]
    [SerializeField] private GameObject leftBumper;
    [SerializeField] private GameObject rightBumper;
    [SerializeField] private GameObject leftGutter;
    [SerializeField] private GameObject rightGutter;

    [Header("Bumper Settings")]
    [SerializeField] private bool bumpersEnabled = false;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    void Start()
    {
        // Set initial bumper state
        SetBumpersEnabled(bumpersEnabled);
    }

    /// <summary>
    /// Enables or disables bumpers
    /// </summary>
    /// <param name="enabled">True to enable bumpers, false to disable</param>
    public void SetBumpersEnabled(bool enabled)
    {
        bumpersEnabled = enabled;

        // Toggle bumper GameObjects
        if (leftBumper != null)
        {
            leftBumper.SetActive(enabled);
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("LaneController: Left bumper reference is missing!");
        }

        if (rightBumper != null)
        {
            rightBumper.SetActive(enabled);
        }
        else if (showDebugLogs)
        {
            Debug.LogWarning("LaneController: Right bumper reference is missing!");
        }

        if (showDebugLogs)
        {
            Debug.Log($"LaneController: Bumpers {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    /// Toggles bumpers on/off
    /// </summary>
    public void ToggleBumpers()
    {
        SetBumpersEnabled(!bumpersEnabled);
    }

    /// <summary>
    /// Gets current bumper state
    /// </summary>
    /// <returns>True if bumpers are enabled</returns>
    public bool AreBumpersEnabled()
    {
        return bumpersEnabled;
    }

    /// <summary>
    /// Validates lane setup (called in editor)
    /// </summary>
    void OnValidate()
    {
        // Ensure bumpers and gutters are properly assigned
        if (leftBumper == null || rightBumper == null)
        {
            Debug.LogWarning("LaneController: Bumper references not assigned in inspector!");
        }

        if (leftGutter == null || rightGutter == null)
        {
            Debug.LogWarning("LaneController: Gutter references not assigned in inspector!");
        }
    }

    // Visualize lane boundaries in Scene view
    void OnDrawGizmos()
    {
        if (leftBumper != null && rightBumper != null)
        {
            Gizmos.color = bumpersEnabled ? Color.green : Color.red;

            // Draw bumper positions
            if (leftBumper.activeSelf || !Application.isPlaying)
            {
                Gizmos.DrawWireCube(leftBumper.transform.position, leftBumper.transform.localScale);
            }

            if (rightBumper.activeSelf || !Application.isPlaying)
            {
                Gizmos.DrawWireCube(rightBumper.transform.position, rightBumper.transform.localScale);
            }
        }
    }
}
