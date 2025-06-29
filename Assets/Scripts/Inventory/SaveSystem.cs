using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [Header("Save Settings")]
    public string saveFileName = "gamesave.json";

    [Header("References")]
    [SerializeField] private HotBar hotbarReference;

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    void Start()
    {
        // Try to load save data when the game starts
        LoadGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame();
        }
    }

    private void SaveGame()
    {
        try
        {
            GameSaveData saveData = CreateSaveData();

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, json);

            Debug.Log($"Game saved succesfully to: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to save game: {e.Message}");
        }
        
    }

    private void LoadGame()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                ApplySaveData(saveData);
                Debug.Log($"Game loaded succesfully from {SavePath}");
            }
            else
            {
                Debug.Log("No save file found, starting fresh game");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    private GameSaveData CreateSaveData()
    {
        GameSaveData saveData = new GameSaveData();

        // === PLAYER DATA ===
        GameObject playerObj = GameObject.FindWithTag("Player");
        Camera playerCamera = playerObj.GetComponentInChildren<Camera>();
        Transform cameraRoot = playerCamera.transform.parent;

        if (playerObj != null)
        {
            saveData.player.playerPosition = playerObj.transform.position;
            saveData.player.playerCameraRotationVertical = playerCamera.transform.eulerAngles;
            saveData.player.playerCameraRotationHorizontal = cameraRoot.transform.eulerAngles;
        }

        // === INVENTORY DATA ===

        if (hotbarReference != null)
        {
            saveData.inventory.currentHotbarSlot = hotbarReference.currentSlot;
            saveData.inventory.hotbarItems = hotbarReference.GetHotbarItems();
        }

        //// === WORLD DATA ===
        if (WorldItemManager.Instance != null)
        {
            saveData.world.worldItems = WorldItemManager.Instance.getAllWorldItemData();
            Debug.Log($"Saving {saveData.world.worldItems.Count} world items");
        }

        return saveData;
    }



    private void ApplySaveData(GameSaveData saveData)
    {
        // === PLAYER DATA ===
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // Set player position
            playerObj.transform.position = saveData.player.playerPosition;

            // Set player camera rotation using the PlayerCamera script
            PlayerCamera playerCameraScript = playerObj.GetComponentInChildren<PlayerCamera>();
            if (playerCameraScript != null)
            {
                try
                {
                    playerCameraScript.SetRotationFromSave(saveData.player.playerCameraRotationVertical, saveData.player.playerCameraRotationHorizontal);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in SetRotationFromSave: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("PlayerCamera component not found in children of player!");
            }
        }
        Debug.Log("Successfully loaded the saved game");

        // === INVENTORY DATA ===
        if (hotbarReference != null)
        {
            hotbarReference.currentSlot = saveData.inventory.currentHotbarSlot;
            hotbarReference.UpdateSelection();
        }

        // === WORLD DATA ===
        if (WorldItemManager.Instance != null && saveData.world.worldItems != null)
        {
            WorldItemManager.Instance.LoadWorldItem(saveData.world.worldItems);
            Debug.Log($"Loaded {saveData.world.worldItems.Count} world items");
        }

        Debug.Log("Succesfully loaded the save game");
    }

    // Utility methods
    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted");
        }
    }
}
