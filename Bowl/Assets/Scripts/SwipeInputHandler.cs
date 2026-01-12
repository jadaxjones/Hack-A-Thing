/*
 * SwipeInputHandler.cs
 * Bowling Game - Touch Input Handler
 *
 * Detects swipe gestures on touchscreen, calculates velocity and direction,
 * and converts 2D touch input into 3D force vectors for ball launching.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SwipeInputHandler : MonoBehaviour
{
    [Header("Swipe Settings")]
    [SerializeField] private float minimumSwipeDistance = 50f; // Minimum pixels for valid swipe
    [SerializeField] private float forceMultiplier = 0.01f; // Maps pixel velocity to Newton force
    [SerializeField] private float maxSwipeTime = 1.0f; // Maximum time for a swipe (seconds)

    [Header("Force Settings")]
    [SerializeField] private float minForce = 3f; // Minimum force applied
    [SerializeField] private float maxForce = 20f; // Maximum force applied
    [SerializeField] private float lateralForceRatio = 0.5f; // Lateral force as ratio of forward force

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // Input tracking
    private Vector2 swipeStartPosition;
    private float swipeStartTime;
    private bool isTracking = false;

    // Events
    public event Action<Vector3> OnSwipeDetected;

    // Input Actions (using Unity's new Input System)
    private InputAction touchPressAction;
    private InputAction touchPositionAction;

    void Awake()
    {
        // Set up input actions manually
        // Note: In the Unity Editor, you should also set up InputSystem_Actions.inputactions
        touchPressAction = new InputAction(
            name: "TouchPress",
            binding: "<Touchscreen>/primaryTouch/press"
        );

        touchPositionAction = new InputAction(
            name: "TouchPosition",
            type: InputActionType.Value,
            binding: "<Touchscreen>/primaryTouch/position"
        );

        // Subscribe to events
        touchPressAction.started += OnTouchStarted;
        touchPressAction.canceled += OnTouchEnded;

        // Enable for mouse support in editor
        if (Application.isEditor)
        {
            var mousePress = new InputAction(
                name: "MousePress",
                binding: "<Mouse>/leftButton"
            );
            mousePress.started += OnTouchStarted;
            mousePress.canceled += OnTouchEnded;
            mousePress.Enable();

            var mousePosition = new InputAction(
                name: "MousePosition",
                type: InputActionType.Value,
                binding: "<Mouse>/position"
            );
            touchPositionAction = mousePosition;
        }
    }

    void OnEnable()
    {
        touchPressAction?.Enable();
        touchPositionAction?.Enable();
    }

    void OnDisable()
    {
        touchPressAction?.Disable();
        touchPositionAction?.Disable();
    }

    /// <summary>
    /// Called when touch/press begins
    /// </summary>
    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        if (touchPositionAction == null) return;

        swipeStartPosition = touchPositionAction.ReadValue<Vector2>();
        swipeStartTime = Time.time;
        isTracking = true;

        if (showDebugLogs)
        {
            Debug.Log($"SwipeInputHandler: Touch started at {swipeStartPosition}");
        }
    }

    /// <summary>
    /// Called when touch/press ends
    /// </summary>
    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        if (!isTracking || touchPositionAction == null) return;

        Vector2 swipeEndPosition = touchPositionAction.ReadValue<Vector2>();
        float swipeDuration = Time.time - swipeStartTime;
        isTracking = false;

        // Validate swipe
        if (!IsValidSwipe(swipeStartPosition, swipeEndPosition, swipeDuration))
        {
            if (showDebugLogs)
            {
                Debug.Log("SwipeInputHandler: Invalid swipe (too short or too slow)");
            }
            return;
        }

        // Calculate force
        Vector3 force = CalculateForce(swipeStartPosition, swipeEndPosition, swipeDuration);

        // Fire event
        OnSwipeDetected?.Invoke(force);

        if (showDebugLogs)
        {
            Debug.Log($"SwipeInputHandler: Valid swipe detected! Force: {force.magnitude:F2}N, Direction: {force.normalized}");
        }
    }

    /// <summary>
    /// Validates if the swipe meets minimum requirements
    /// </summary>
    private bool IsValidSwipe(Vector2 startPos, Vector2 endPos, float duration)
    {
        float distance = Vector2.Distance(startPos, endPos);

        if (distance < minimumSwipeDistance)
        {
            return false;
        }

        if (duration > maxSwipeTime)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Calculates 3D force vector from 2D swipe data
    /// </summary>
    private Vector3 CalculateForce(Vector2 startPos, Vector2 endPos, float duration)
    {
        // Calculate 2D delta and velocity
        Vector2 swipeDelta = endPos - startPos;
        Vector2 swipeVelocity = swipeDelta / duration;

        // Map 2D screen space to 3D world space
        // Y (vertical screen swipe) → Z (forward in world)
        // X (horizontal screen swipe) → X (lateral in world)
        float forwardComponent = swipeVelocity.y * forceMultiplier;
        float lateralComponent = swipeVelocity.x * forceMultiplier * lateralForceRatio;

        // Create 3D force vector
        Vector3 force = new Vector3(lateralComponent, 0f, forwardComponent);

        // Clamp force magnitude
        float forceMagnitude = force.magnitude;
        if (forceMagnitude < minForce)
        {
            force = force.normalized * minForce;
        }
        else if (forceMagnitude > maxForce)
        {
            force = force.normalized * maxForce;
        }

        return force;
    }

    /// <summary>
    /// Enables or disables input handling
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        if (enabled)
        {
            touchPressAction?.Enable();
            touchPositionAction?.Enable();
        }
        else
        {
            touchPressAction?.Disable();
            touchPositionAction?.Disable();
            isTracking = false;
        }

        if (showDebugLogs)
        {
            Debug.Log($"SwipeInputHandler: Input {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    /// Draws debug visualization in Scene view
    /// </summary>
    void OnGUI()
    {
        if (!showDebugLogs || !isTracking) return;

        // Draw swipe line during touch
        if (touchPositionAction != null)
        {
            Vector2 currentPosition = touchPositionAction.ReadValue<Vector2>();
            // Simple debug text
            GUI.Label(new Rect(10, 10, 300, 20), $"Swipe distance: {Vector2.Distance(swipeStartPosition, currentPosition):F1}px");
        }
    }

    void OnDestroy()
    {
        // Clean up
        if (touchPressAction != null)
        {
            touchPressAction.started -= OnTouchStarted;
            touchPressAction.canceled -= OnTouchEnded;
            touchPressAction.Dispose();
        }

        if (touchPositionAction != null)
        {
            touchPositionAction.Dispose();
        }
    }
}
