using System.IO;
using UnityEngine;

public class VirtualCameraPositionLogger : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
    [SerializeField] private string outputFilePath = "camera_position_log.txt";

    private StreamWriter writer;
    private float timer;

    private void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera not assigned. Please assign a Cinemachine Virtual Camera in the inspector.");
            return;
        }

        writer = new StreamWriter(outputFilePath, true); // Append to file
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            LogCameraPosition();
            timer = 0f;
        }
    }

    private void LogCameraPosition()
    {
        Transform cameraTransform = virtualCamera.transform;
        string logEntry = $"Timestamp: {System.DateTime.Now} - Position: {cameraTransform.position}";

        writer.WriteLine(logEntry);
        writer.Flush(); // Ensure data is written immediately
    }

    private void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close(); // Close the writer when the script is destroyed
        }
    }
}
