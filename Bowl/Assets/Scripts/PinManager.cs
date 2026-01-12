/*
 * PinManager.cs
 * Bowling Game - Pin Formation Manager
 *
 * Manages all 10 bowling pins: spawning, positioning in triangle formation,
 * counting knocked down pins, and resetting for new throws.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 *
 * Updated by Claude Code on 2026-01-11
 * User prompt: Fix hidden pins still being counted (8 pins + 0 pins = 16 bug)
 * Change: Modified GetPinsKnockedDown() to only count active (visible) pins
 */

using UnityEngine;
using System.Collections.Generic;

public class PinManager : MonoBehaviour
{
    [Header("Pin Prefab")]
    [SerializeField] private GameObject pinPrefab;

    [Header("Pin Formation Settings")]
    [SerializeField] private Vector3 formationStartPosition = new Vector3(0f, 0.19f, 16f);
    [SerializeField] private float pinSpacing = 0.24f; // Distance between pins

    [Header("Pin References")]
    private List<PinController> pins = new List<PinController>();
    private GameObject pinsParent;

    void Awake()
    {
        // Create parent object for organization
        pinsParent = new GameObject("Pins");
        pinsParent.transform.SetParent(transform);
    }

    void Start()
    {
        SpawnPins();
    }

    /// <summary>
    /// Spawns all 10 pins in standard bowling triangle formation
    /// </summary>
    public void SpawnPins()
    {
        // Clear existing pins if any
        foreach (var pin in pins)
        {
            if (pin != null)
            {
                Destroy(pin.gameObject);
            }
        }
        pins.Clear();

        // Standard 10-pin bowling triangle formation
        // Row 1 (front): 1 pin
        // Row 2: 2 pins
        // Row 3: 3 pins
        // Row 4 (back): 4 pins

        Vector3 basePosition = formationStartPosition;
        int pinNumber = 1;

        // Row 1 - Front pin (pin 1)
        CreatePin(pinNumber++, basePosition);

        // Row 2 - 2 pins
        basePosition.z += pinSpacing * 1.5f;
        CreatePin(pinNumber++, basePosition + Vector3.left * (pinSpacing * 0.5f));
        CreatePin(pinNumber++, basePosition + Vector3.right * (pinSpacing * 0.5f));

        // Row 3 - 3 pins
        basePosition.z += pinSpacing * 1.5f;
        CreatePin(pinNumber++, basePosition + Vector3.left * pinSpacing);
        CreatePin(pinNumber++, basePosition);
        CreatePin(pinNumber++, basePosition + Vector3.right * pinSpacing);

        // Row 4 - 4 pins
        basePosition.z += pinSpacing * 1.5f;
        CreatePin(pinNumber++, basePosition + Vector3.left * (pinSpacing * 1.5f));
        CreatePin(pinNumber++, basePosition + Vector3.left * (pinSpacing * 0.5f));
        CreatePin(pinNumber++, basePosition + Vector3.right * (pinSpacing * 0.5f));
        CreatePin(pinNumber++, basePosition + Vector3.right * (pinSpacing * 1.5f));

        Debug.Log($"PinManager: Spawned {pins.Count} pins in formation");
    }

    /// <summary>
    /// Creates a single pin at the specified position
    /// </summary>
    private void CreatePin(int number, Vector3 position)
    {
        if (pinPrefab == null)
        {
            Debug.LogError("PinManager: Pin prefab not assigned! Please assign a pin prefab in the inspector.");
            return;
        }

        GameObject pinObject = Instantiate(pinPrefab, position, Quaternion.identity, pinsParent.transform);
        pinObject.name = $"Pin_{number:D2}";

        PinController pinController = pinObject.GetComponent<PinController>();
        if (pinController == null)
        {
            Debug.LogError($"PinManager: Pin prefab is missing PinController component!");
            return;
        }

        pins.Add(pinController);
    }

    /// <summary>
    /// Counts how many pins have been knocked down
    /// </summary>
    /// <returns>Number of knocked down pins</returns>
    public int GetPinsKnockedDown()
    {
        int knockedDown = 0;
        foreach (var pin in pins)
        {
            // Only count pins that are active (visible) in the scene
            if (pin != null && pin.gameObject.activeInHierarchy && !pin.IsStanding())
            {
                knockedDown++;
            }
        }
        return knockedDown;
    }

    /// <summary>
    /// Counts how many pins are still standing
    /// </summary>
    /// <returns>Number of standing pins</returns>
    public int GetPinsStanding()
    {
        return 10 - GetPinsKnockedDown();
    }

    /// <summary>
    /// Checks if all pins have stopped moving
    /// </summary>
    /// <returns>True if all pins are stationary</returns>
    public bool AllPinsStopped()
    {
        foreach (var pin in pins)
        {
            if (pin != null && !pin.IsStopped())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Resets all pins to their starting positions
    /// </summary>
    public void ResetAllPins()
    {
        foreach (var pin in pins)
        {
            if (pin != null)
            {
                pin.ResetPin();
            }
        }
        Debug.Log("PinManager: All pins reset");
    }

    /// <summary>
    /// Resets only the pins that are still standing (for second throw)
    /// </summary>
    public void ResetStandingPins()
    {
        foreach (var pin in pins)
        {
            if (pin != null && pin.IsStanding())
            {
                pin.ResetPin();
            }
        }
        Debug.Log($"PinManager: {GetPinsStanding()} standing pins reset");
    }

    /// <summary>
    /// Removes knocked down pins (for second throw in a frame)
    /// </summary>
    public void RemoveKnockedDownPins()
    {
        foreach (var pin in pins)
        {
            if (pin != null && !pin.IsStanding())
            {
                pin.gameObject.SetActive(false);
            }
        }
        Debug.Log($"PinManager: Removed {GetPinsKnockedDown()} knocked down pins");
    }

    /// <summary>
    /// Shows all pins (restores visibility)
    /// </summary>
    public void ShowAllPins()
    {
        foreach (var pin in pins)
        {
            if (pin != null)
            {
                pin.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Gets total number of pins
    /// </summary>
    public int GetTotalPins()
    {
        return pins.Count;
    }

    // Visualize pin formation in Scene view
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(formationStartPosition, 0.1f);
        }
    }
}
