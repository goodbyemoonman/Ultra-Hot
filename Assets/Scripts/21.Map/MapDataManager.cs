using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class MapDataManager {
    public MapDataSource source;

    public MapDataManager()
    {
        source = new MapDataSource();
    }

    private void OnEnable()
    {
        source = new MapDataSource();
    }

    public void SaveMapData(string mapPath)
    {
        using (StreamWriter sw = new StreamWriter(mapPath))
        {
            XmlSerializer xs = new XmlSerializer(typeof(MapDataSource));
            xs.Serialize(sw, source);
        }
    }

    public void LoadMapData(string mapPath)
    {
        string[] tmp = mapPath.Split('/');
        string path = "";
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i] == "Resources")
                path = "";
            else
                path = Path.Combine(path, tmp[i]);
        }

        if (path.Contains(".xml"))
            path = path.Remove(path.Length - ".xml".Length);

        TextAsset ta = Resources.Load<TextAsset>(path);
        StringReader sr = new StringReader(ta.text);
        
        XmlSerializer x = new XmlSerializer(typeof(MapDataSource));
        source = x.Deserialize(sr) as MapDataSource;
        
    }

    public Vector2Int Str2Vec2(string str)
    {
        str.Trim();
        str = str.Substring(1, str.Length - 2);
        string[] num = str.Split(',');
        for (int i = 0; i < num.Length; i++)
        {
            Debug.Log(i + " : " + num[i]);
        }
        Vector2Int result = new Vector2Int(int.Parse(num[0]), int.Parse(num[1]));

        return result;
    }

    public void SetMapName(string name)
    {
        name.Trim();
        if (name.Contains(".xml"))
        {
            name.Remove(name.Length - ".xml".Length);
        }

        Debug.Log("Set Map Name to >> " + name);
        source.mapName = name;
    }

    public void SetWall(Vector2Int coord)
    {
        if (CanSet(coord) == false ||
            CheckCollision(coord, source.playerSpawn) ||
            CheckCollision(coord, source.gunSpawn) ||
            CheckCollision(coord, source.enemySpawns))
            return;

        if (source.walls.Contains(coord))
        {
            source.walls.Remove(coord);
            Debug.Log("Unset Wall to " + coord);
        }
        else
        {
            source.walls.Add(coord);
            Debug.Log("Set Wall to " + coord);
        }
    }

    public void SetPlayerSpawn(Vector2Int coord)
    {
        if (CanSet(coord) == false || 
            CheckCollision(coord, source.walls) ||
            CheckCollision(coord, source.gunSpawn) ||
            CheckCollision(coord, source.enemySpawns))
            return;

        source.playerSpawn = coord;
        Debug.Log("Set Player Spawn at " + coord);
    }

    public void SetEnemySpawn(Vector2Int coord)
    {
        if (CanSet(coord) == false ||
            CheckCollision(coord, source.playerSpawn) ||
            CheckCollision(coord, source.gunSpawn) ||
            CheckCollision(coord, source.walls))
            return;

        if (source.enemySpawns.Contains(coord))
        {
            source.enemySpawns.Remove(coord);
            Debug.Log("Unset Enemy spawn at " + coord);
        }
        else
        {
            source.enemySpawns.Add(coord);
            Debug.Log("Set Eenmy spawn at " + coord);
        }
    }

    public void SetGunSpawn(Vector2Int coord)
    {
        if (CanSet(coord) == false ||
            CheckCollision(coord, source.playerSpawn) ||
            CheckCollision(coord, source.walls) ||
            CheckCollision(coord, source.enemySpawns))
            return;

        if (source.gunSpawn.Contains(coord))
        {
            source.gunSpawn.Remove(coord);
            Debug.Log("Unset Pistol Spawn at " + coord);
        }
        else
        {
            source.gunSpawn.Add(coord);
            Debug.Log("Set Pistol Spawn at " + coord);
        }
    }

    public void SetMapSize(Vector2Int size)
    {
        if (size.x < 0 || size.y < 0)
            return;
        Debug.Log("Set Map Size to " + size);
        source.mapSize = size;
    }

    public void SetNumberOfEnemy(int input)
    {
        source.numberOfEnemy = Mathf.Clamp(input, 0, 20);
        Debug.Log("Number of Enemy Set : " + source.numberOfEnemy);
    }

    public void ClearMapData()
    {
        source.Clear();
    }

    bool CanSet(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= source.mapSize.x)
            return false;
        if (coord.y < 0 || coord.y >= source.mapSize.y)
            return false;
        return true;
    }

    bool CheckCollision(Vector2Int coord, Vector2Int coord2)
    {
        if (coord == coord2)
            return true;
        return false;
    }

    bool CheckCollision(Vector2Int coord, List<Vector2Int> coord2)
    {
        if (coord2.Contains(coord))
            return true;
        return false;
    }
}
