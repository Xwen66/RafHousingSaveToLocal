using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using System;
using System.IO;

public class TelemetryManager : MonoBehaviour
{
    string serverURL = "http://localhost:3000/telemetry";
    private string telemetryLogPath = "TelemetryLogs.json";

    public static TelemetryManager Instance {get;private set;}

    private Queue<Dictionary<string, object>> eventQueue; 
    private bool isSending = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            eventQueue = new Queue<Dictionary<string, object>>();
        }
        else{
            Destroy(gameObject);
        }
    }

    public void LogEvent(string eventName, Dictionary<string, object> parameters)
    {
        if(parameters == null)
        {
            parameters = new Dictionary<string, object>();
        }

        // Use SessionManager's AuthToken if available
        string userToken = SessionManager.Instance?.AuthToken ?? "unknown_user";

        parameters["eventName"] = eventName;
        parameters["sessionId"] = System.Guid.NewGuid().ToString();
        parameters["deviceTime"] = System.DateTime.UtcNow.ToString("o");
        parameters["userToken"] = userToken;

        eventQueue.Enqueue(parameters);

        // Save locally as JSON
        SaveTelemetryEventLocally(parameters);

        if(!isSending) StartCoroutine(SendEvents());
    }

    private void SaveTelemetryEventLocally(Dictionary<string, object> parameters)
    {
        string json = JsonUtility.ToJson(new SerializationWrapper(parameters));
        try
        {
            File.AppendAllText(telemetryLogPath, json + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save telemetry event locally: {e.Message}");
        }
    }

    private IEnumerator SendEvents()
    {
        isSending = true;

        while(eventQueue.Count > 0)
        {
            Dictionary<string, object> currentEvent = eventQueue.Dequeue();
            string payload= JsonUtility.ToJson(new SerializationWrapper(currentEvent));

            using (UnityWebRequest request = new UnityWebRequest(serverURL, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(payload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                request.SetRequestHeader("Content-Type", "application/json");
                //TODO: Add bearer token --- "bearer 

                yield return request.SendWebRequest();
                if(request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"Error {request.error}")                    ;
                    eventQueue.Enqueue(currentEvent);
                    break;
                }
                else{
                    Debug.Log("request send: " + payload);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        isSending = false;

    }

    [System.Serializable]
    private class SerializationWrapper
    {
        public List<string> keys = new List<string>();
        public List<string> values = new List<string>();
        public SerializationWrapper(Dictionary<string, object> parameters)
        {
            foreach(var kvp in parameters)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value != null ? kvp.Value.ToString() : "null");
            }
        }
    }
}
