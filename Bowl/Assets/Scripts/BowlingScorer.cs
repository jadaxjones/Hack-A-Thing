/*
 * BowlingScorer.cs
 * Bowling Game - Scoring System
 *
 * Implements standard bowling scoring rules for single-frame mode.
 * Tracks throws, detects strikes and spares, calculates score.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 *
 * Updated by Claude Code on 2026-01-11
 * User prompt: Fix score resetting to 0 between frames
 * Change: Removed totalScore reset in ResetFrame() method to persist score across frames
 *
 * Updated by Claude Code on 2026-01-11
 * User prompt: Fix frame counter not incrementing
 * Change: Changed ResetFrame() to increment currentFrame instead of resetting to 1
 *
 * Updated by Claude Code on 2026-01-11
 * User prompt: Update score after every throw and track frame-by-frame scores (format: #|#|# = total)
 * Changes:
 *   - Added frameScores list to track running totals
 *   - Modified RecordThrow() to update totalScore after EVERY throw (not just frame complete)
 *   - Added GetFormattedScore() method to format score as "10|20|28 = 58"
 *
 * Updated by Claude Code on 2026-01-11
 * User prompt: Fix score format - each pipe number should be THAT frame's score, not running total
 * Changes:
 *   - Changed frameScores to store individual frame scores (e.g., [10, 7, 10] not [10, 17, 27])
 *   - Added currentFrameScore to track score during current frame
 *   - Updated GetFormattedScore() to show completed frames + current frame in progress
 *   - Display updates every throw: "10|7|5 = 22" means frames scored 10, 7, and currently 5
 */

using UnityEngine;
using System;
using System.Collections.Generic;

public class BowlingScorer : MonoBehaviour
{
    [Header("Frame State")]
    private int currentFrame = 1;
    private int currentThrow = 1;
    private int firstThrowPins = 0;
    private int secondThrowPins = 0;
    private int totalScore = 0;
    private bool frameComplete = false;
    private List<int> frameScores = new List<int>(); // Score for each completed frame (not running total)
    private int currentFrameScore = 0; // Score for the current frame being played

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
            currentFrameScore = pinsDown; // Set current frame score
            totalScore += pinsDown;

            if (pinsDown == 10)
            {
                // Strike!
                frameScores.Add(10); // Add THIS frame's score (10) to completed frames
                frameComplete = true;
                result = "STRIKE!";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: STRIKE! Frame score: 10, Total score: {totalScore}");
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
                    Debug.Log($"BowlingScorer: First throw: {pinsDown} pins, current frame: {currentFrameScore}, total: {totalScore}");
                }

                OnScoreResult?.Invoke(result);
                OnScoreChanged?.Invoke(totalScore); // Update UI after first throw
            }
        }
        else if (currentThrow == 2)
        {
            // Second throw
            secondThrowPins = pinsDown;
            currentFrameScore += pinsDown; // Add to current frame score
            totalScore += pinsDown;

            int totalPins = firstThrowPins + secondThrowPins;

            if (totalPins == 10)
            {
                // Spare!
                result = "SPARE!";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: SPARE! ({firstThrowPins} + {secondThrowPins}) Frame score: {currentFrameScore}, Total: {totalScore}");
                }
            }
            else
            {
                // Normal scoring
                result = $"Total: {totalPins} pin{(totalPins == 1 ? "" : "s")}";

                if (showDebugLogs)
                {
                    Debug.Log($"BowlingScorer: Frame complete: {totalPins} pins ({firstThrowPins} + {secondThrowPins}). Frame score: {currentFrameScore}, Total: {totalScore}");
                }
            }

            frameScores.Add(currentFrameScore); // Add THIS frame's score to completed frames
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
        currentFrame++; // Increment to next frame
        currentThrow = 1;
        firstThrowPins = 0;
        secondThrowPins = 0;
        currentFrameScore = 0; // Reset for new frame
        // DO NOT reset totalScore - it should persist across frames!
        frameComplete = false;

        if (showDebugLogs)
        {
            Debug.Log($"BowlingScorer: Frame reset - now on frame {currentFrame}");
        }

        // Don't invoke OnScoreChanged here - score hasn't changed
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

    /// <summary>
    /// Gets formatted score string with frame breakdown
    /// Format: "10|7|5 = 22" where each number is that frame's score
    /// If in middle of frame, includes current frame score: "10|7|3 = 20"
    /// </summary>
    public string GetFormattedScore()
    {
        // No completed frames yet
        if (frameScores.Count == 0 && currentFrameScore == 0)
        {
            return $"Score: {totalScore}";
        }

        // Build the frame breakdown
        string frameBreakdown = "";

        if (frameScores.Count > 0)
        {
            frameBreakdown = string.Join("|", frameScores);
        }

        // If we're in the middle of a frame (not complete), add current frame score
        if (!frameComplete && currentFrameScore > 0)
        {
            if (frameBreakdown.Length > 0)
            {
                frameBreakdown += "|";
            }
            frameBreakdown += currentFrameScore.ToString();
        }

        // If we only have current frame score (no completed frames yet)
        if (frameBreakdown.Length == 0)
        {
            frameBreakdown = currentFrameScore.ToString();
        }

        return $"{frameBreakdown} = {totalScore}";
    }
}
