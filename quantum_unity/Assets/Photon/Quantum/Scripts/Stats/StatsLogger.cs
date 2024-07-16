using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuantumLogger : MonoBehaviour
{
    int nProcessID = Process.GetCurrentProcess().Id;

    private StreamWriter logWriter;
    private string logFilePath = "";

    Stopwatch _networkTimer;

    private float timer = 0f;

    void Start()
    {
        logFilePath = $"output_28players/stats_log_{nProcessID}.csv"; // Unique file per process

        // create event system if none exists in the scene
        var eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems == null || eventSystems.Length == 0)
        {
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
        }

        try
        {
            logWriter = new StreamWriter(logFilePath, true); // Append if file exists
            logWriter.WriteLine("Timestamp,FrameVerified,FramePredicted,PredictedFrames,Ping,UpdateTime(ms),InputOffset,ResimulatedFrames,SimulationState,NetworkIn(bytes/s),NetworkOut(bytes/s)"); // Header
        }
        catch (IOException e)
        {
            UnityEngine.Debug.LogError($"Error opening log file: {e.Message}");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f && QuantumRunner.Default)
        {
            var gameInstance = QuantumRunner.Default.Game;
            timer = 0f; // Reset the timer

            if (gameInstance?.Session?.Game == null)
            {
                return;
            }

            // Log Game Stats (CSV format)
            string logLine = $"{DateTime.Now},{gameInstance.Session.FrameVerified?.Number ?? 0},{gameInstance.Session.FramePredicted?.Number ?? 0},{gameInstance.Session.PredictedFrames},{gameInstance.Session.Stats.Ping},{Math.Round(gameInstance.Session.Stats.UpdateTime * 1000, 2)},{gameInstance.Session.Stats.Offset},{gameInstance.Session.Stats.ResimulatedFrames},{(gameInstance.Session.IsStalling ? "Stalling" : "Running")},";

            // Log Network Stats (if available)
            if (QuantumRunner.Default.NetworkClient != null && QuantumRunner.Default.NetworkClient.IsConnected)
            {
                QuantumRunner.Default.NetworkClient.LoadBalancingPeer.TrafficStatsEnabled = true;

                if (_networkTimer == null)
                {
                    _networkTimer = Stopwatch.StartNew();
                }
                else
                {
                    double elapsedSeconds = _networkTimer.Elapsed.TotalSeconds;
                    if (elapsedSeconds > 0)  // Avoid division by zero
                    {
                        logLine += $"{(int)(QuantumRunner.Default.NetworkClient.LoadBalancingPeer.TrafficStatsIncoming.TotalPacketBytes / elapsedSeconds)},{(int)(QuantumRunner.Default.NetworkClient.LoadBalancingPeer.TrafficStatsOutgoing.TotalPacketBytes / elapsedSeconds)}";
                    }
                    else
                    {
                        logLine += "0,0";
                    }
                }
            }

            try
            {
                logWriter.WriteLine(logLine); // Write the line to the CSV file
            }
            catch (IOException e)
            {
                UnityEngine.Debug.LogError($"Error writing to log file: {e.Message}");
            }
        }
    }

    void OnApplicationQuit() // Or OnDestroy if appropriate
    {
        if (logWriter != null)
        {
            logWriter.Close();
        }
    }

    public void ResetNetworkStats()
    {
        _networkTimer = null;

        if (QuantumRunner.Default != null && QuantumRunner.Default.NetworkClient != null && QuantumRunner.Default.NetworkClient.IsConnected)
        {
            QuantumRunner.Default.NetworkClient.LoadBalancingPeer.TrafficStatsReset();
        }
    }
}
