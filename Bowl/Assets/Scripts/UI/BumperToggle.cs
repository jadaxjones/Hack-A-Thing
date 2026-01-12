/*
 * BumperToggle.cs
 * Bowling Game - Bumper Toggle UI Controller
 *
 * Handles UI toggle button for enabling/disabling bumpers.
 * Communicates with LaneController to toggle bumper state.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;
using UnityEngine.UI;

public class BumperToggle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LaneController laneController;
    [SerializeField] private Toggle toggle;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    void Awake()
    {
        // Get toggle component if not assigned
        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }

        if (toggle == null)
        {
            Debug.LogError("BumperToggle: Toggle component not found!");
        }
    }

    void Start()
    {
        // Validate lane controller reference
        if (laneController == null)
        {
            Debug.LogError("BumperToggle: LaneController reference is missing!");
            return;
        }

        // Set initial toggle state to match lane controller
        if (toggle != null)
        {
            toggle.isOn = laneController.AreBumpersEnabled();

            // Subscribe to toggle value changes
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    /// <summary>
    /// Called when toggle value changes
    /// </summary>
    /// <param name="value">New toggle value</param>
    private void OnToggleValueChanged(bool value)
    {
        if (laneController == null) return;

        laneController.SetBumpersEnabled(value);

        if (showDebugLogs)
        {
            Debug.Log($"BumperToggle: Bumpers {(value ? "enabled" : "disabled")} via UI");
        }
    }

    /// <summary>
    /// Enables or disables the toggle interactability
    /// </summary>
    /// <param name="enabled">True to enable interaction, false to disable</param>
    public void SetInteractable(bool enabled)
    {
        if (toggle != null)
        {
            toggle.interactable = enabled;

            if (showDebugLogs)
            {
                Debug.Log($"BumperToggle: Interactability {(enabled ? "enabled" : "disabled")}");
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from toggle events
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }

    void OnValidate()
    {
        if (laneController == null)
        {
            Debug.LogWarning("BumperToggle: LaneController reference not assigned!");
        }

        if (toggle == null)
        {
            toggle = GetComponent<Toggle>();
        }
    }
}
