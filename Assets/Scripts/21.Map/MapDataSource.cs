using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDataSource {
    public Vector2Int mapSize;
    public string mapName;
    public List<Vector2Int> walls;
    public List<Vector2Int> enemySpawns;
    public List<Vector2Int> gunSpawn;
    public Vector2Int playerSpawn;
    public int numberOfEnemy;

    public MapDataSource()
    {
        walls = new List<Vector2Int>();
        enemySpawns = new List<Vector2Int>();
        gunSpawn = new List<Vector2Int>();
    }

    public void Clear()
    {
        walls.Clear();
        enemySpawns.Clear();
        gunSpawn.Clear();
        mapSize = Vector2Int.one;
        mapName = "NewMap";
        playerSpawn = Vector2Int.zero;
        numberOfEnemy = 0;
    }
}
