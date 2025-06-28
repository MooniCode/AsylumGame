
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public Vector3 playerPosition;
    public Vector3 playerCameraRotationVertical;
    public Vector3 playerCameraRotationHorizontal;

    public PlayerSaveData()
    {
        playerPosition = Vector3.zero;
        playerCameraRotationVertical = Vector3.zero;
        playerCameraRotationHorizontal = Vector3.zero;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public List<string> hotbarItems;
    public int currentHotbarSlot;

    public InventorySaveData()
    {
        hotbarItems = new List<string>();
        currentHotbarSlot = 0;
    }
}

[System.Serializable]
public class WorldSaveData
{
    public List<ItemSpawn> worldItems;
    public List<EnemySpawn> enemies;

    public WorldSaveData()
    {
        worldItems = new List<ItemSpawn>();
        enemies = new List<EnemySpawn>();
    }
}

public class ProgressSaveData
{
    public Dictionary<string, float> gameValues;

    public ProgressSaveData()
    {
        gameValues = new Dictionary<string, float>();
    }
}

public class GameMetaSaveData
{
    public float totalPlayTime;
    public System.DateTime lastSaveTime;
    public string gameVersion;
    public int saveVersion;

    public GameMetaSaveData()
    {
        totalPlayTime = 0f;
        lastSaveTime = System.DateTime.Now;
        gameVersion = Application.version;
        saveVersion = 1;
    }
}

[System.Serializable]
public class ItemSpawn
{
    public string itemId;
    public Vector3 position;
    public Vector3 rotation;

    public ItemSpawn(string id, Vector3 pos, Vector3 rot)
    {
        itemId = id;
        position = pos;
        rotation = rot;
    }
}

[System.Serializable]
public class EnemySpawn
{
    public string enemyType;
    public Vector3 position;
    public Vector3 rotation;
    public float health;
    public bool isAlive;

    public EnemySpawn(string type, Vector3 pos, Vector3 rot, float hp, bool alive)
    {
        enemyType = type;
        position = pos;
        rotation = rot;
        health = hp;
        isAlive = alive;
    }
}

[System.Serializable]
public class GameSaveData
{
    public PlayerSaveData player;
    public InventorySaveData inventory;
    public WorldSaveData world;
    public ProgressSaveData progress;
    public GameMetaSaveData meta;

    public GameSaveData()
    {
        player = new PlayerSaveData();
        inventory = new InventorySaveData();
        world = new WorldSaveData();
        progress = new ProgressSaveData();
        meta = new GameMetaSaveData();
    }
}
