using UnityEngine;
using System.Collections.Generic;

public class SessionTracker : MonoBehaviour
{
    private float sessionStartTime = 0f;
    private List<string> leftClickTimestamps = new List<string>();

    void Start()
    {
        sessionStartTime = Time.time;
        TelemetryManager.Instance.LogEvent("session_start", new Dictionary<string, object>
        {
            {"startTime", System.DateTime.UtcNow.ToString("o")}
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
        TelemetryManager.Instance.LogEvent("session_end", new Dictionary<string, object>
        {
            {"duration_sec", sessionDuration},
            {"endTime", System.DateTime.UtcNow.ToString("o")},
            {"leftClickTimestamps", leftClickTimestamps}
        });
    }
}
