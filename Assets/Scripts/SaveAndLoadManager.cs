using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class HouseSaveData
{
    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> rotations = new List<Vector3>();
}

public class SaveAndLoadManager : MonoBehaviour
{
    public HouseManager houseManager; 
    
    [Header("Save/Load Input Fields")]
    public TMP_InputField saveUsernameInputField;
    public TMP_InputField loadUsernameInputField;
    
    public Button loadButton;
    public Button saveButton;
    private string SaveDirectory => Path.Combine(Application.persistentDataPath, "SavedWorlds");

    void Start()
    {
        Directory.CreateDirectory(SaveDirectory);
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGameFromButton);
        }
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadGameFromButton);
        }
    }
    public void SaveGameFromButton()
    {
        string username = saveUsernameInputField.text;
        SaveGame(username);
    }
    public void LoadGameFromButton()
    {
        string username = loadUsernameInputField.text;
        LoadGame(username);
    }
    public void SaveGame(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("Username cannot be empty");
            return;
        }
        HouseSaveData saveData = new HouseSaveData();
        GameObject[] placedHouses = GameObject.FindGameObjectsWithTag("PlacedHouse");
        foreach (GameObject house in placedHouses)
        {
            saveData.positions.Add(house.transform.position);
            saveData.rotations.Add(house.transform.rotation.eulerAngles);
        }
        string json = JsonUtility.ToJson(saveData);
        string filename = Path.Combine(SaveDirectory, $"{username}_save.json");
        File.WriteAllText(filename, json);
        Debug.Log($"Game saved for user: {username}");
        
        if (saveUsernameInputField != null)
        {
            saveUsernameInputField.text = string.Empty;
        }
    }

    public void LoadGame(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogError("Username cannot be empty");
            return;
        }
        string filename = Path.Combine(SaveDirectory, $"{username}_save.json");
        
        if (!File.Exists(filename))
        {
            Debug.LogError($"No save file found for user: {username}");
            return;
        }
        string json = File.ReadAllText(filename);
        HouseSaveData loadedData = JsonUtility.FromJson<HouseSaveData>(json);
        GameObject[] existingHouses = GameObject.FindGameObjectsWithTag("PlacedHouse");
        foreach (GameObject house in existingHouses)
        {
            Destroy(house);
        }
        for (int i = 0; i < loadedData.positions.Count; i++)
        {
            GameObject newHouse = Instantiate(houseManager.placementPrefab, 
                loadedData.positions[i], 
                Quaternion.Euler(loadedData.rotations[i]));
            
            newHouse.tag = "PlacedHouse";
        }
        Debug.Log($"Game loaded for user: {username}");
        if (loadUsernameInputField != null)
        {
            loadUsernameInputField.text = string.Empty;
        }
    }
}
