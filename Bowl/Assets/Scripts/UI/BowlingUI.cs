/*
 * BowlingUI.cs
 * Bowling Game - UI Controller
 *
 * Manages all UI elements: score display, frame/throw indicators, and status messages.
 * Updates UI in response to game events.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BowlingUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI frameText;
    [SerializeField] private TextMeshProUGUI throwText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Message Settings")]
    [SerializeField] private float defaultMessageDuration = 2.0f;

    private Coroutine messageCoroutine;

    void Start()
    {
        // Initialize UI
        UpdateScore(0);
        UpdateFrame(1, 1);
        ShowMessage("Swipe to throw!", 0f);
    }

    /// <summary>
    /// Updates the score display
    /// </summary>
    /// <param name="score">Current total score</param>
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        else
        {
            Debug.LogWarning("BowlingUI: Score text reference is missing!");
        }
    }

    /// <summary>
    /// Updates the frame and throw display
    /// </summary>
    /// <param name="frame">Current frame number</param>
    /// <param name="throwNumber">Current throw number (1 or 2)</param>
    public void UpdateFrame(int frame, int throwNumber)
    {
        if (frameText != null)
        {
            frameText.text = $"Frame: {frame}";
        }
        else
        {
            Debug.LogWarning("BowlingUI: Frame text reference is missing!");
        }

        if (throwText != null)
        {
            throwText.text = $"Throw: {throwNumber}";
        }
        else
        {
            Debug.LogWarning("BowlingUI: Throw text reference is missing!");
        }
    }

    /// <summary>
    /// Shows a status message
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="duration">Duration to show message (0 = indefinite)</param>
    public void ShowMessage(string message, float duration = -1f)
    {
        if (statusText == null)
        {
            Debug.LogWarning("BowlingUI: Status text reference is missing!");
            return;
        }

        // Stop any existing message coroutine
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        // Set message text
        statusText.text = message;

        // Use default duration if not specified
        if (duration < 0)
        {
            duration = defaultMessageDuration;
        }

        // Clear message after duration (if not indefinite)
        if (duration > 0)
        {
            messageCoroutine = StartCoroutine(ClearMessageAfterDelay(duration));
        }
    }

    /// <summary>
    /// Clears the status message after a delay
    /// </summary>
    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (statusText != null)
        {
            statusText.text = "";
        }

        messageCoroutine = null;
    }

    /// <summary>
    /// Clears the status message immediately
    /// </summary>
    public void ClearMessage()
    {
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
        }

        if (statusText != null)
        {
            statusText.text = "";
        }
    }

    /// <summary>
    /// Shows a temporary highlighted message (for strikes, spares)
    /// </summary>
    /// <param name="message">Message to display</param>
    /// <param name="duration">Duration to show</param>
    public void ShowHighlightMessage(string message, float duration = 2.0f)
    {
        if (statusText != null)
        {
            // Save original color
            Color originalColor = statusText.color;

            // Set highlight color (yellow for strikes/spares)
            statusText.color = Color.yellow;

            ShowMessage(message, duration);

            // Reset color after duration
            StartCoroutine(ResetColorAfterDelay(originalColor, duration));
        }
    }

    /// <summary>
    /// Resets text color after delay
    /// </summary>
    private IEnumerator ResetColorAfterDelay(Color originalColor, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (statusText != null)
        {
            statusText.color = originalColor;
        }
    }

    /// <summary>
    /// Validates UI references in editor
    /// </summary>
    void OnValidate()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("BowlingUI: Score Text reference not assigned!");
        }

        if (frameText == null)
        {
            Debug.LogWarning("BowlingUI: Frame Text reference not assigned!");
        }

        if (throwText == null)
        {
            Debug.LogWarning("BowlingUI: Throw Text reference not assigned!");
        }

        if (statusText == null)
        {
            Debug.LogWarning("BowlingUI: Status Text reference not assigned!");
        }
    }
}
