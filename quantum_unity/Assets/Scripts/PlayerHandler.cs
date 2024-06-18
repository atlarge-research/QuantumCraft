using UnityEngine;
using Cinemachine;
using Quantum;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.InputSystem.LowLevel;
using System.IO;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] EntityView entityView;
    private string traceName = Path.Combine(Application.streamingAssetsPath, "InputTraces/MovingAround.inputtrace"); // Name of the trace asset'

    private InputEventTrace traceEvent;
    public void OnEntityInstantiated()
    {

        QuantumGame game = QuantumRunner.Default.Game;

        Frame frame = game.Frames.Verified;
        if (frame.TryGet(entityView.EntityRef, out PlayerLink playerLink))
        {
            if (game.PlayerIsLocal(playerLink.Player))
            {
                CinemachineVirtualCamera virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
                virtualCamera.m_Follow = transform;

                // Assign the virtual camera as a sub game object of the local player.
                // Done to have the camera only for the local player
                Vector3 cameraOffset = Vector3.zero;
                cameraOffset.y = 0.7f;
                virtualCamera.transform.position = transform.position + cameraOffset;
                virtualCamera.transform.rotation = transform.rotation;
                virtualCamera.transform.SetParent(transform);

                //traceEvent = new InputEventTrace();
                //traceEvent.ReadFrom(traceName);
                //traceEvent.Replay().PlayAllEventsAccordingToTimestamps();
                //Log.Error("Started playing back input events");



            }
        }


    }
}
