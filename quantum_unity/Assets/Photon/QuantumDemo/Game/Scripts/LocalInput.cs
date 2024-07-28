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
    private bool leftMouseClicked = false;
    private bool rightMouseClicked = false;


    private Coroutine currentPattern;
    private float patternDuration = 480f; // 8 minutes in seconds

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
        // i.PrimaryAction = primaryAction.triggered;
        // i.SecondaryAction = secondaryAction.triggered;

        i.PrimaryAction = leftMouseClicked;
        i.SecondaryAction = rightMouseClicked;

        Vector2 moveInput = hardcodedMoveInput; 
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
            // 1. Move forward and look right for 0.1 seconds
            hardcodedMoveInput = Vector2.up;
            yield return new WaitForSeconds(0.1f);

            // 2. Click the left mouse button
            leftMouseClicked = true; // Simulate left click

            // 3. Move right and look down for 0.1 seconds
            hardcodedMoveInput = Vector2.right;
            hardcodedLookInput = Vector2.down + Vector2.down;   
            yield return new WaitForSeconds(0.1f);

            // 4. Click the right mouse button
            rightMouseClicked = true; // Simulate right click

            // 5. Move backwards and look left for 0.1 seconds
            hardcodedMoveInput = Vector2.down;
            hardcodedLookInput = Vector2.left;    
            yield return new WaitForSeconds(0.1f);

            // 6. Move left and look up for 0.1 seconds
            hardcodedMoveInput = Vector2.left;
            hardcodedLookInput = Vector2.up;    
            yield return new WaitForSeconds(0.1f);
        }

        // Reset all inputs to neutral
        hardcodedMoveInput = Vector2.zero;
        hardcodedLookInput = Vector2.zero;
        leftMouseClicked = false;
        rightMouseClicked = false;

        currentPattern = null;
    }
}
