using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class MapDataManager : ScriptableObject {
    enum TYPE { WALLS, ENEMYSPAWNS }
    const string MAP = "map";
    const string NAME = "name";
    const string SIZE = "size";
    const string WALLS = "walls";
    const string ENEMYSPAWNS = "enemySpawns";
    const string PLAYERSPAWN = "playerSpawn";
    const string VECTOR2 = "vector2";

    public MapDataSource source = new MapDataSource();

    #region do not use this code anymore
    /*
    public void SaveMapData(string mapPath)
    {
        string[] tmpStr = mapPath.Split('/');
        SetMapName(tmpStr[tmpStr.Length - 1].Substring(0, tmpStr[tmpStr.Length - 1].Length - ".xml".Length));

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = System.Text.Encoding.Unicode;
        using (XmlWriter writer = XmlWriter.Create(mapPath, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(MAP);
            {
                writer.WriteElementString(NAME, source.mapName);
                writer.WriteElementString(SIZE, source.mapSize.ToString());
                writer.WriteElementString(PLAYERSPAWN, source.playerSpawn.ToString());
                writer.WriteStartElement(WALLS);
                {
                    for (int i = 0; i < source.walls.Count; i++)
                    {
                        writer.WriteElementString(VECTOR2, source.walls[i].ToString());
                    }
                }
                writer.WriteEndElement();

                writer.WriteStartElement(ENEMYSPAWNS);
                {
                    for (int i = 0; i < source.enemySpawns.Count; i++)
                    {
                        writer.WriteElementString(VECTOR2, source.enemySpawns[i].ToString());
                    }
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }

    public void LoadMapData(string mapPath)
    {
        string[] tmpStr = mapPath.Split('.');
        tmpStr = tmpStr[0].Split('/');
        string newPath = tmpStr[0];
        for (int i = 1; i < tmpStr.Length; i++)

        {
            if (tmpStr[i] == "Resources")
                newPath = "";
            else
                newPath = Path.Combine(newPath, tmpStr[i]);
        }
        Debug.Log("Load Path >> " + newPath);
        TextAsset textAsset = Resources.Load<TextAsset>(newPath);
        Debug.Log("textAsset text >> " + textAsset.text);
        if (textAsset == null || textAsset.text == "")
            return;

        source.Clear();
        using (XmlReader reader = XmlReader.Create(new StringReader(textAsset.text)))
        {
            TYPE t = TYPE.WALLS;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case SIZE:
                            SetMapSize(Str2Vec2(reader.ReadString()));
                            break;
                        case NAME:
                            SetMapName(reader.ReadString());
                            break;
                        case PLAYERSPAWN:
                            SetPlayerSpawn(Str2Vec2(reader.ReadString()));
                            break;
                        case WALLS:
                            t = TYPE.WALLS;
                            break;
                        case ENEMYSPAWNS:
                            t = TYPE.ENEMYSPAWNS;
                            break;
                        case VECTOR2:
                            if (t == TYPE.WALLS)
                                SetWall(Str2Vec2(reader.ReadString()));
                            else
                                SetEnemySpawn(Str2Vec2(reader.ReadString()));
                            break;
                    }
                }
            }
        }
    }*/
    #endregion

    public void SaveMapData(string mapPath)
    {
        using (StreamWriter sw = new StreamWriter(mapPath))
        {
            XmlSerializer xs = new XmlSerializer(typeof(MapDataSource));
            xs.Serialize(sw, source);
            Debug.Log(sw.ToString());
        }
    }

    public void LoadMapData(string mapPath)
    {
        using (StreamReader sr = new StreamReader(mapPath))
        {
            XmlSerializer xs = new XmlSerializer(typeof(MapDataSource));
            source = xs.Deserialize(sr) as MapDataSource;
        }
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
            CheckCollision(coord, source.enemySpawns))
            return;

        source.playerSpawn = coord;
        Debug.Log("Set Player Spawn at " + coord);
    }

    public void SetEnemySpawn(Vector2Int coord)
    {
        if (CanSet(coord) == false ||
            CheckCollision(coord, source.playerSpawn) ||
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

    public void SetMapSize(Vector2Int size)
    {
        if (size.x < 0 || size.y < 0)
            return;
        Debug.Log("Set Map Size to " + size);
        source.mapSize = size;
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
