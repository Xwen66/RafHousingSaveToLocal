using UnityEngine;
using System.Collections.Generic;

public class SessionTracker : MonoBehaviour
{
    public static string SessionStartTime { get; private set; } = "";
    public static string SessionEndTime { get; private set; } = "";
    private string sessionStartTimeString = "";
    private float sessionStartTime = 0f;
    private List<string> leftClickTimestamps = new List<string>();

    public static void EnsureInitialized()
    {
        if (string.IsNullOrEmpty(SessionStartTime))
        {
            SessionStartTime = System.DateTime.UtcNow.ToString("o");
        }
    }

    void Start()
    {
        sessionStartTime = Time.time;
        sessionStartTimeString = System.DateTime.UtcNow.ToString("o");
        SessionStartTime = sessionStartTimeString;
        TelemetryManager.Instance.LogEvent("session_start", new Dictionary<string, object>
        {
            {"startTime", sessionStartTimeString}
        });
    }

    void Update()
    {
        // Log timestamp on left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            leftClickTimestamps.Add(System.DateTime.UtcNow.ToString("o"));
        }
    }

    private void OnApplicationQuit()
    {
        float sessionDuration = Time.time - sessionStartTime;
        string sessionEndTimeString = System.DateTime.UtcNow.ToString("o");
        SessionEndTime = sessionEndTimeString;
        var sessionLog = new Dictionary<string, object>
        {
            {"eventName", "session_telemetry"},
            {"startTime", sessionStartTimeString},
            {"endTime", sessionEndTimeString},
            {"duration_sec", sessionDuration},
            {"leftClickTimestamps", leftClickTimestamps}
        };
        TelemetryManager.Instance.LogEvent("session_telemetry", sessionLog);
    }
}
