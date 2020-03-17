using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldMaker : MonoBehaviour {
    MapDataManager mdm;
    public Tilemap obstacleGrid;
    public Tile obstacle;
    string dataFilePath;
    MapList mapNow;

    enum MapList { T0 = -4, T1 = -3, T2 = -2, T3 = -1, S1 = 0, S2 = 1, S3 = 2, S4 = 3 };

    private void Awake()
    {
        mapNow = MapList.T1;
        mdm = new MapDataManager();
        dataFilePath = Application.dataPath + "/Resources/MapData/";
        StageManager.Instance.GameStateTeller += GameStateObserver;
    }

    void GameStateObserver(GameStateList state)
    {
        switch (state)
        {
            case GameStateList.StageReady:
                InitMap();
                break;
            case GameStateList.Win:
                mapNow = GetNextMap(mapNow);
                break;
        }
    }

    MapList GetNextMap(MapList map)
    {
        MapList result = map;
        if (result >= MapList.S1)
        {
            while(result == map)
                result = (MapList)Random.Range(0, 4);
        }
        else
        {
            result++;
        }
        return result;
    }

    void InitMap()
    {
        obstacleGrid.ClearAllTiles();
        mdm.LoadMapData(dataFilePath + mapNow.ToString() + ".xml");
        SetTileMap(mdm.source.walls, mdm.source.playerSpawn);
        SetEnemySpawn(mdm.source.enemySpawns, mdm.source.playerSpawn);
        SetNumberOfEnemy(mdm.source.numberOfEnemy);
        SetPistol(mdm.source.gunSpawn, mdm.source.playerSpawn);
    }

    void SetTileMap(List<Vector2Int> walls, Vector2Int center)
    {
        for(int i = 0; i < walls.Count; i++)
        {
            Vector3Int pos = new Vector3Int(
                walls[i].x - center.x,
                walls[i].y - center.y, 
                0);
            obstacleGrid.SetTile(pos, obstacle);
        }
    }

    void SetEnemySpawn(List<Vector2Int> enemySpawns, Vector2Int center)
    {
        for(int i = 0; i < enemySpawns.Count; i++)
        {
            Vector3 newPos = new Vector3(enemySpawns[i].x - center.x, enemySpawns[i].y - center.y);
            GameObject newSpawner = ObjPoolManager.Instance.GetObject(ObjectPoolList.EnemySpawner);
            newSpawner.transform.position = newPos;
            newSpawner.SetActive(true);

            StageManager.Instance.AddEnemySpawner(newSpawner);
        }
    }

    void SetNumberOfEnemy(int num)
    {
        StageManager.Instance.SetNumberOfEnemy(num);
    }

    void SetPistol(List<Vector2Int> gunPos, Vector2Int center)
    {
        for(int i = 0; i < gunPos.Count; i++)
        {
            GameObject newGun = ObjPoolManager.Instance.GetObject(ObjectPoolList.Pistol);
            newGun.transform.position = new Vector3(gunPos[i].x - center.x, gunPos[i].y - center.y);
            newGun.SetActive(true);
        }
    }
}
