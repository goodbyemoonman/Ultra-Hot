using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataSource {
    public Vector2Int mapSize;
    public string mapName;
    public List<Vector2Int> walls = new List<Vector2Int>();
    public List<Vector2Int> enemySpawns = new List<Vector2Int>();
    public Vector2Int playerSpawn;

    public void Clear()
    {
        mapSize = Vector2Int.one;
        mapName = "New Map";
        walls.Clear();
        enemySpawns.Clear();
        playerSpawn = Vector2Int.one;
    }
}
