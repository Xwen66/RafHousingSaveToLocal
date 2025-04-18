using UnityEngine;
using System.Collections.Generic;
public class SessionTracker : MonoBehaviour
{
    private float sessionStartTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sessionStartTime = Time.time;
        
        TelemetryManager.Instance.LogEvent("session_start", new Dictionary<string, object>
        {
            {"startTime", System.DateTime.UtcNow.ToString("o")}
        });
    }

    // Update is called once per frame
    private void OnApplicationQuit()
    {
        float sessionDuration = Time.time - sessionStartTime;
        TelemetryManager.Instance.LogEvent("session_end", new Dictionary<string, object>
        {
            {"duration_sec", sessionDuration},
            {"endTime", System.DateTime.UtcNow.ToString("o")}
        });
    }
}
