/*
 * BowlingScorer.cs
 * Bowling Game - Scoring System
 *
 * Implements standard bowling scoring rules for single-frame mode.
 * Tracks throws, detects strikes and spares, calculates score.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;
using System;

public class BowlingScorer : MonoBehaviour
{
    [Header("Frame State")]
    private int currentFrame = 1;
    private int currentThrow = 1;
    private int firstThrowPins = 0;
    private int secondThrowPins = 0;
    private int totalScore = 0;
    private bool frameComplete = false;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // Events
    public event Action<string> OnScoreResult; // Fires with message like "STRIKE!", "SPARE!", "5 pins"
    public event Action<int> OnScoreChanged; // Fires when total score changes
    public event Action OnFrameComplete; // Fires when frame is complete

    /// <summary>
    /// Records a throw and calculates the result
    /// </summary>
    /// <param name="pinsDown">Number of pins knocked down in this throw</param>
    /// <returns>Result message (STRIKE, SPARE, or pin count)</returns>
    public string RecordThrow(int pinsDown)
    {
        string result = "";

        if (currentThrow == 1)
        {
            // First throw
            firstThrowPins = pinsDown;

            if (pinsDown == 10)
            {
                // Strike!
                totalScore += 10;
                frameComplete = true;
                result = "STRIKE!";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: STRIKE! Total score: {totalScore}");
                }

                OnScoreResult?.Invoke(result);
                OnScoreChanged?.Invoke(totalScore);
                OnFrameComplete?.Invoke();
            }
            else
            {
                // Normal first throw, continue to second throw
                currentThrow = 2;
                result = $"{pinsDown} pin{(pinsDown == 1 ? "" : "s")}";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: First throw: {pinsDown} pins");
                }

                OnScoreResult?.Invoke(result);
            }
        }
        else if (currentThrow == 2)
        {
            // Second throw
            secondThrowPins = pinsDown;
            int totalPins = firstThrowPins + secondThrowPins;

            if (totalPins == 10)
            {
                // Spare!
                totalScore += 10;
                result = "SPARE!";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: SPARE! ({firstThrowPins} + {secondThrowPins}) Total score: {totalScore}");
                }
            }
            else
            {
                // Normal scoring
                totalScore += totalPins;
                result = $"Total: {totalPins} pin{(totalPins == 1 ? "" : "s")}";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: Frame complete: {totalPins} pins ({firstThrowPins} + {secondThrowPins}). Total score: {totalScore}");
                }
            }

            frameComplete = true;
            OnScoreResult?.Invoke(result);
            OnScoreChanged?.Invoke(totalScore);
            OnFrameComplete?.Invoke();
        }

        return result;
    }

    /// <summary>
    /// Gets the current total score
    /// </summary>
    public int GetCurrentScore()
    {
        return totalScore;
    }

    /// <summary>
    /// Gets the current frame number
    /// </summary>
    public int GetCurrentFrame()
    {
        return currentFrame;
    }

    /// <summary>
    /// Gets the current throw number (1 or 2)
    /// </summary>
    public int GetCurrentThrow()
    {
        return currentThrow;
    }

    /// <summary>
    /// Checks if the current frame is complete
    /// </summary>
    public bool IsFrameComplete()
    {
        return frameComplete;
    }

    /// <summary>
    /// Gets pins knocked down in first throw
    /// </summary>
    public int GetFirstThrowPins()
    {
        return firstThrowPins;
    }

    /// <summary>
    /// Gets pins knocked down in second throw
    /// </summary>
    public int GetSecondThrowPins()
    {
        return secondThrowPins;
    }

    /// <summary>
    /// Resets the scorer for a new frame
    /// </summary>
    public void ResetFrame()
    {
        currentFrame = 1;
        currentThrow = 1;
        firstThrowPins = 0;
        secondThrowPins = 0;
        totalScore = 0;
        frameComplete = false;

        if (showDebugLogs)
        {
            Debug.Log("BowlingScorer: Frame reset");
        }

        OnScoreChanged?.Invoke(totalScore);
    }

    /// <summary>
    /// Gets a summary of the current frame state
    /// </summary>
    public string GetFrameSummary()
    {
        if (frameComplete)
        {
            if (firstThrowPins == 10)
            {
                return "Strike (X)";
            }
            else if (firstThrowPins + secondThrowPins == 10)
            {
                return $"Spare ({firstThrowPins} + {secondThrowPins})";
            }
            else
            {
                return $"{firstThrowPins} + {secondThrowPins} = {firstThrowPins + secondThrowPins}";
            }
        }
        else if (currentThrow == 2)
        {
            return $"First throw: {firstThrowPins}";
        }
        else
        {
            return "Ready";
        }
    }

    /// <summary>
    /// Checks if this was a strike
    /// </summary>
    public bool WasStrike()
    {
        return frameComplete && firstThrowPins == 10;
    }

    /// <summary>
    /// Checks if this was a spare
    /// </summary>
    public bool WasSpare()
    {
        return frameComplete && currentThrow == 2 && (firstThrowPins + secondThrowPins == 10);
    }
}
