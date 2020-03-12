using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDataSource {
    public Vector2Int mapSize;
    public string mapName;
    public List<Vector2Int> walls;
    public List<Vector2Int> enemySpawns;
    public Vector2Int playerSpawn;

    public MapDataSource()
    {
        walls = new List<Vector2Int>();
        enemySpawns = new List<Vector2Int>();
    }

    public void Clear()
    {
        walls = new List<Vector2Int>();
        enemySpawns = new List<Vector2Int>();
        mapSize = Vector2Int.one;
        mapName = "NewMap";
        playerSpawn = Vector2Int.zero;
    }
}
