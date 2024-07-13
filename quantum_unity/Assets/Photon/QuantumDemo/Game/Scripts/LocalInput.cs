/*using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInput : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction jumpAction;
    private InputAction primaryAction;
    private InputAction secondaryAction;
    private InputAction moveAction;
    private InputAction lookAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if(playerInput == null)
        {
            Log.Error("Null component");
        }
        jumpAction = playerInput.actions["Jump"];
        primaryAction = playerInput.actions["PrimaryAction"];
        secondaryAction = playerInput.actions["SecondaryAction"];
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    private void OnEnable()
    {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));

        // Enable Actions
        jumpAction.Enable();
        primaryAction.Enable();
        secondaryAction.Enable();
        moveAction.Enable();
        lookAction.Enable();
    }
    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input i = new Quantum.Input();

        i.Jump = jumpAction.triggered;
        i.PrimaryAction = primaryAction.triggered;
        i.SecondaryAction = secondaryAction.triggered;

        // Note: Assuming 'Move' is Vector2, 'Look' is Vector2
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // Assuming you want a scaled integer value for direction
        i.DirectionX = (short)(moveInput.x * 10);
        i.DirectionY = (short)(moveInput.y * 10);

        // Look input might need adjustment based on your Quantum settings
        i.PitchYaw.Y = FP.FromFloat_UNSAFE(lookInput.y * 0.01f); // Adjust sensitivity
        i.PitchYaw.X = FP.FromFloat_UNSAFE(lookInput.x * 0.01f); // Adjust sensitivity

        callback.SetInput(i, DeterministicInputFlags.Repeatable);
    }
}*/

using Photon.Deterministic;
using Quantum;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInput : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private InputAction primaryAction;
    private InputAction secondaryAction;
    private InputAction moveAction;
    private InputAction lookAction;

    private Vector2 hardcodedMoveInput = Vector2.zero;
    private Vector2 hardcodedLookInput = Vector2.zero;

    private Coroutine currentPattern;
    private float patternDuration = 120f; // 2 minutes in seconds

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Log.Error("Null component");
        }
        jumpAction = playerInput.actions["Jump"];
        primaryAction = playerInput.actions["PrimaryAction"];
        secondaryAction = playerInput.actions["SecondaryAction"];
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
    }

    private void OnEnable()
    {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));

        jumpAction.Enable();
        primaryAction.Enable();
        secondaryAction.Enable();
        moveAction.Enable();
        lookAction.Enable();
    }

    private void Start()
    {
        StartPattern();
    }

    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input i = new Quantum.Input();

        i.Jump = jumpAction.triggered;
        i.PrimaryAction = primaryAction.triggered;
        i.SecondaryAction = secondaryAction.triggered;

        Vector2 moveInput = hardcodedMoveInput; // Use hardcoded input
        Vector2 lookInput = hardcodedLookInput;

        i.DirectionX = (short)(moveInput.x * 10);
        i.DirectionY = (short)(moveInput.y * 10);

        i.PitchYaw.Y = FP.FromFloat_UNSAFE(lookInput.y * 0.01f);
        i.PitchYaw.X = FP.FromFloat_UNSAFE(lookInput.x * 0.01f);

        callback.SetInput(i, DeterministicInputFlags.Repeatable);
    }

    // Start a pattern of hardcoded actions
    public void StartPattern()
    {
        if (currentPattern != null)
            StopCoroutine(currentPattern);

        currentPattern = StartCoroutine(PatternCoroutine());
    }

    // Coroutine for executing the action pattern
    private IEnumerator PatternCoroutine()
    {
        float startTime = Time.time;

        while (Time.time - startTime < patternDuration)
        {
            // 1. Move forward for 0.5 seconds
            hardcodedMoveInput = Vector2.up;
            yield return new WaitForSeconds(0.5f);

            // 2. Move right for 0.5 seconds
            hardcodedMoveInput = Vector2.right;
            yield return new WaitForSeconds(0.5f);

            // 3. Move backwards for 0.5 seconds
            hardcodedMoveInput = Vector2.down;
            yield return new WaitForSeconds(0.5f);

            // 4. Move left for 0.5 seconds
            hardcodedMoveInput = Vector2.left;
            yield return new WaitForSeconds(0.5f);
        }

        // 5. Reset input to neutral after the pattern is complete
        hardcodedMoveInput = Vector2.zero;

        currentPattern = null; // Mark pattern as finished
    }
}

