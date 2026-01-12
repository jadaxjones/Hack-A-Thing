/*
 * GameManager.cs
 * Bowling Game - Central Game Controller
 *
 * Main game state machine and system coordinator.
 * Manages flow between waiting for input, ball rolling, scoring, and resetting.
 * Integrates ball controller, pin manager, input handler, scorer, and UI.
 *
 * Written by Claude Code on 2026-01-11
 * User prompt: Create physics-based iOS bowling game with swipe controls
 */

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private BallController ballController;
    [SerializeField] private PinManager pinManager;
    [SerializeField] private SwipeInputHandler inputHandler;
    [SerializeField] private BowlingScorer scorer;
    [SerializeField] private BowlingUI bowlingUI;

    [Header("Game Settings")]
    [SerializeField] private float ballStopCheckDelay = 1.0f; // Wait time before checking if ball stopped
    [SerializeField] private float pinSettleTime = 2.0f; // Time to wait for pins to settle
    [SerializeField] private float resetDelay = 2.0f; // Delay before resetting after frame complete

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // Game state
    public enum GameState
    {
        WaitingForInput,
        BallRolling,
        BallStopped,
        Scoring,
        Resetting
    }

    private GameState currentState = GameState.WaitingForInput;

    void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// Initializes all game systems
    /// </summary>
    private void InitializeGame()
    {
        // Validate references
        if (ballController == null)
        {
            Debug.LogError("GameManager: BallController reference missing!");
        }

        if (pinManager == null)
        {
            Debug.LogError("GameManager: PinManager reference missing!");
        }

        if (inputHandler == null)
        {
            Debug.LogError("GameManager: SwipeInputHandler reference missing!");
        }

        if (scorer == null)
        {
            Debug.LogError("GameManager: BowlingScorer reference missing!");
        }

        // Subscribe to input events
        if (inputHandler != null)
        {
            inputHandler.OnSwipeDetected += HandleSwipeInput;
        }

        // Subscribe to scorer events
        if (scorer != null)
        {
            scorer.OnScoreResult += HandleScoreResult;
            scorer.OnScoreChanged += HandleScoreChanged;
        }

        // Initialize UI
        if (bowlingUI != null)
        {
            bowlingUI.UpdateScore(0);
            bowlingUI.UpdateFrame(1, 1);
            bowlingUI.ShowMessage("Swipe to throw!", 0f); // Show indefinitely
        }

        // Set initial state
        SetState(GameState.WaitingForInput);

        if (showDebugLogs)
        {
            Debug.Log("GameManager: Game initialized");
        }
    }

    /// <summary>
    /// Handles swipe input from SwipeInputHandler
    /// </summary>
    private void HandleSwipeInput(Vector3 force)
    {
        if (currentState != GameState.WaitingForInput)
        {
            if (showDebugLogs)
            {
                Debug.Log($"GameManager: Swipe ignored - not in WaitingForInput state (current: {currentState})");
            }
            return;
        }

        // Launch ball
        if (ballController != null)
        {
            ballController.Launch(force);
            SetState(GameState.BallRolling);

            if (bowlingUI != null)
            {
                bowlingUI.ShowMessage("Ball rolling...", 0f);
            }
        }
    }

    /// <summary>
    /// Main game loop update
    /// </summary>
    void Update()
    {
        switch (currentState)
        {
            case GameState.BallRolling:
                UpdateBallRollingState();
                break;

            case GameState.BallStopped:
                UpdateBallStoppedState();
                break;
        }
    }

    /// <summary>
    /// Updates ball rolling state - checks if ball has stopped
    /// </summary>
    private void UpdateBallRollingState()
    {
        if (ballController == null) return;

        // Check if ball has stopped moving
        if (!ballController.IsMoving())
        {
            SetState(GameState.BallStopped);
            StartCoroutine(WaitForPinsToSettle());
        }
    }

    /// <summary>
    /// Updates ball stopped state - waits for pins to settle
    /// </summary>
    private void UpdateBallStoppedState()
    {
        // Waiting for pins to settle (handled by coroutine)
    }

    /// <summary>
    /// Waits for pins to settle, then scores
    /// </summary>
    private IEnumerator WaitForPinsToSettle()
    {
        if (showDebugLogs)
        {
            Debug.Log("GameManager: Ball stopped, waiting for pins to settle...");
        }

        // Wait for pins to settle
        yield return new WaitForSeconds(pinSettleTime);

        // Double-check pins are stopped
        if (pinManager != null)
        {
            while (!pinManager.AllPinsStopped())
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Transition to scoring
        SetState(GameState.Scoring);
        ScoreThrow();
    }

    /// <summary>
    /// Scores the current throw
    /// </summary>
    private void ScoreThrow()
    {
        if (pinManager == null || scorer == null) return;

        int pinsDown = pinManager.GetPinsKnockedDown();

        if (showDebugLogs)
        {
            Debug.Log($"GameManager: Scoring throw - {pinsDown} pins knocked down");
        }

        // Record throw in scorer
        string result = scorer.RecordThrow(pinsDown);

        // Check if frame is complete
        if (scorer.IsFrameComplete())
        {
            // Frame complete - reset after delay
            StartCoroutine(ResetAfterDelay());
        }
        else
        {
            // Second throw coming - remove knocked down pins and reset ball
            StartCoroutine(PrepareForSecondThrow());
        }
    }

    /// <summary>
    /// Prepares the game for the second throw
    /// </summary>
    private IEnumerator PrepareForSecondThrow()
    {
        if (showDebugLogs)
        {
            Debug.Log("GameManager: Preparing for second throw");
        }

        yield return new WaitForSeconds(1.5f);

        // Remove knocked down pins (optional - or leave them for visual)
        // pinManager.RemoveKnockedDownPins();

        // Reset ball
        if (ballController != null)
        {
            ballController.ResetBall();
        }

        // Update UI
        if (bowlingUI != null)
        {
            bowlingUI.UpdateFrame(scorer.GetCurrentFrame(), scorer.GetCurrentThrow());
            bowlingUI.ShowMessage("Second throw - Swipe to throw!", 0f);
        }

        SetState(GameState.WaitingForInput);
    }

    /// <summary>
    /// Resets game after frame complete
    /// </summary>
    private IEnumerator ResetAfterDelay()
    {
        if (showDebugLogs)
        {
            Debug.Log("GameManager: Frame complete, resetting...");
        }

        yield return new WaitForSeconds(resetDelay);

        SetState(GameState.Resetting);

        // Reset all systems
        if (ballController != null)
        {
            ballController.ResetBall();
        }

        if (pinManager != null)
        {
            pinManager.ShowAllPins();
            pinManager.ResetAllPins();
        }

        if (scorer != null)
        {
            scorer.ResetFrame();
        }

        // Update UI
        if (bowlingUI != null)
        {
            bowlingUI.UpdateFrame(1, 1);
            bowlingUI.ShowMessage("New frame - Swipe to throw!", 0f);
        }

        yield return new WaitForSeconds(0.5f);

        SetState(GameState.WaitingForInput);
    }

    /// <summary>
    /// Sets the current game state
    /// </summary>
    private void SetState(GameState newState)
    {
        if (currentState == newState) return;

        GameState oldState = currentState;
        currentState = newState;

        // Handle state transitions
        OnStateEnter(newState, oldState);

        if (showDebugLogs)
        {
            Debug.Log($"GameManager: State changed from {oldState} to {newState}");
        }
    }

    /// <summary>
    /// Called when entering a new state
    /// </summary>
    private void OnStateEnter(GameState newState, GameState oldState)
    {
        switch (newState)
        {
            case GameState.WaitingForInput:
                // Enable input
                if (inputHandler != null)
                {
                    inputHandler.SetInputEnabled(true);
                }
                break;

            case GameState.BallRolling:
                // Disable input while ball is rolling
                if (inputHandler != null)
                {
                    inputHandler.SetInputEnabled(false);
                }
                break;

            case GameState.Resetting:
                // Disable input during reset
                if (inputHandler != null)
                {
                    inputHandler.SetInputEnabled(false);
                }
                break;
        }
    }

    /// <summary>
    /// Handles score result from scorer
    /// </summary>
    private void HandleScoreResult(string result)
    {
        if (bowlingUI != null)
        {
            bowlingUI.ShowMessage(result, 2.0f);
        }
    }

    /// <summary>
    /// Handles score change from scorer
    /// </summary>
    private void HandleScoreChanged(int newScore)
    {
        if (bowlingUI != null)
        {
            bowlingUI.UpdateScore(newScore);
        }
    }

    /// <summary>
    /// Gets the current game state (for debugging)
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (inputHandler != null)
        {
            inputHandler.OnSwipeDetected -= HandleSwipeInput;
        }

        if (scorer != null)
        {
            scorer.OnScoreResult -= HandleScoreResult;
            scorer.OnScoreChanged -= HandleScoreChanged;
        }
    }
}
