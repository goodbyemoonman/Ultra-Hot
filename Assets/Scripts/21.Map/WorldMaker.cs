using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class WorldMaker : MonoBehaviour {
    MapDataManager mdm;
    public Tilemap obstacleGrid;
    public Tile obstacle;
    string dataFilePath;
    GameManager gm;

    private void Awake()
    {
        gm = GetComponent<GameManager>();
        mdm = new MapDataManager();
        dataFilePath = Application.dataPath + "/Resources/MapData/";
    }

    private void Start()
    {
        SetTileMap();
        gm.SetGameState(GAMESTATE.MAP_READY);
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
}
