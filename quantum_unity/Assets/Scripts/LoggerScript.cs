using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;

public class CustomLogger : MonoBehaviour
{
    private static CustomLogger instance;
    private StreamWriter writer;
    private string logFilePath;

    public static CustomLogger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CustomLogger>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("CustomLogger");
                    instance = obj.AddComponent<CustomLogger>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        try
        {
            // Ensure only one instance exists
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Make this instance the singleton and persist across scenes
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Get the current process ID and format the file name
            int processId = Process.GetCurrentProcess().Id;
            logFilePath = $"latency_output/output_250ms/log_{processId}.txt";

            // Create the StreamWriter to write to the log file
            writer = new StreamWriter(logFilePath, true); // Append to the file if it exists
        }
        catch (System.Exception e)
        {
            //ignore
        }
    }

    public void Log(string message)
    {
        try
        {
            writer.WriteLine(message);
        }
        catch (System.Exception e)
        {
            //ignore
        }
    }

    private void OnDisable()
    {
        // Close the StreamWriter when the script is disabled
        try { writer.Close(); } catch (System.Exception e) { }
    }
}
