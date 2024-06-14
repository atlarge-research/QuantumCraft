using Photon.Deterministic;
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
}
