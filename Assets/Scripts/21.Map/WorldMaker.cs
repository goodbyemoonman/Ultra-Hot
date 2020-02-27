using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class WorldMaker : Singleton<WorldMaker> {
    MapDataManager mdm;
    public Tilemap obstacleGrid;
    public Tile obstacle;
    string dataFilePath;

    private void Awake()
    {
        mdm = ScriptableObject.CreateInstance<MapDataManager>();
        dataFilePath = Application.dataPath + "/Resources/MapData/";
        //txxt.text = dataFilePath;
        SetTileMap();
    }

    public void SetTileMap()
    {
        mdm.LoadMapData(dataFilePath + "S1.xml");

        for(int i = 0; i < mdm.source.walls.Count; i++)
        {
            Vector3Int pos = new Vector3Int(
                mdm.source.walls[i].x - mdm.source.playerSpawn.x, 
                mdm.source.walls[i].y - mdm.source.playerSpawn.y, 
                0);
            obstacleGrid.SetTile(pos, obstacle);
        }
    }

    public Vector2Int GetTileMapSize()
    {
        return mdm.source.mapSize;
    }

    public bool IsWall(Vector2Int pos)
    {
        Vector3Int input = new Vector3Int(pos.x, pos.y, 0);
        if (obstacleGrid.GetTile(input) == obstacle)
            return true;
        else
            return false;
    }
}
