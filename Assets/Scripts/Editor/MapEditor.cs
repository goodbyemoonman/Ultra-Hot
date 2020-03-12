using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditor : EditorWindow
{
    static MapDataManager data;
    BrushType brushNow = BrushType.WALL;
    Vector2Int tmpSom;
    Vector2 scrollPos;
    Rect[,] rects;
    Rect preMouseRect;

    readonly float boxSize = 25;
    readonly Vector2 scrollStartPos = new Vector2(30, 60);
    readonly Vector2 scrollArea = new Vector2(400, 250);
    readonly Vector2 gridStartPos = new Vector2(55, 60);
    readonly Rect gridRect = new Rect(30 + 25, 60 + 25, 400 - 65, 250 - 65);
    readonly Vector2 underScrollArea = new Vector2(0, 340);
    readonly Color[] colors = {
        Color.white,
        new Color(0.5f, 0.5f, 1f, 1),
        new Color(1f, 0.5f, 0.5f, 1),
        new Color(0.5f, 0.5f, 1, 1) };
    enum WallType
    {
        NONE = 0,
        WALL = 1
    }
    enum BrushType { WALL = 1, PLAYERSPAWN = 3, ENEMYSPAWN = 2}

    [MenuItem("Window/Map Editor Window")]
    static void Init()
    {
        data = new MapDataManager();
        MapEditor window = GetWindow<MapEditor>("Map Editor");
        window.Show();
    }

    private void OnGUI()
    {
        if (data == null)
            return;

        EditorGUILayout.BeginHorizontal();
        {
            tmpSom = EditorGUILayout.Vector2IntField("Size of Map", tmpSom, GUILayout.Width(300));
            tmpSom.x = Mathf.Clamp(tmpSom.x, 0, 50);
            tmpSom.y = Mathf.Clamp(tmpSom.y, 0, 50);
            if (GUILayout.Button("Generate", GUILayout.Width(100))){
                GenerateNewMap();
            }
        }
        EditorGUILayout.EndHorizontal();

        DrawMap();

        if (Event.current.type == EventType.MouseDown)
        {
            ClickEvent(GetCoordWithPosition(Event.current.mousePosition));
        }

        GUILayout.BeginHorizontal(GUILayout.Width(400));
        {
            brushNow = (BrushType)GUILayout.Toolbar((int)brushNow - 1,
                new string[] {
                    BrushType.WALL.ToString(),
                    BrushType.ENEMYSPAWN.ToString(),
                    BrushType.PLAYERSPAWN.ToString()
                }) + 1;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(GUILayout.Width(400));
        {
            if (GUI.Button(new Rect(underScrollArea, new Vector2(150, EditorGUIUtility.singleLineHeight)), "Load Map"))
            {
                string path = EditorUtility.OpenFilePanel("Open Map Data", "", "xml");
                if (path.Length != 0)
                {
                    Debug.Log("Load Path >>" + path);
                    data.LoadMapData(path);
                    LoadMap();
                }
            }

            Rect r = new Rect(underScrollArea, new Vector2(150, EditorGUIUtility.singleLineHeight));
            r.position = new Vector2(r.position.x + 150 + 30, r.position.y);

            if(GUI.Button(r, "Save Map"))
            {
                string path = EditorUtility.SaveFilePanel("Save Map Data", "", "New Map","xml");
                if (path.Length != 0)
                {
                    Debug.Log("Save Map Data At >>" + path);
                    data.SaveMapData(path);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
        }
        GUILayout.EndHorizontal();
    }


    void GenerateNewMap()
    {
        data.ClearMapData();
        data.SetMapSize(tmpSom);
        GenerateBoxGrid();
    }

    void LoadMap()
    {
        tmpSom = data.source.mapSize;
        GenerateBoxGrid();
    }

    void GenerateBoxGrid()
    {
        rects = new Rect[data.source.mapSize.x, data.source.mapSize.y];

        for (int y = 0; y < data.source.mapSize.y; y++)
        {
            for (int x = 0; x < data.source.mapSize.x; x++)
            {
                rects[x, y] = new Rect(
                    boxSize * x + boxSize, (data.source.mapSize.y * boxSize - boxSize * y),
                    boxSize, boxSize);
            }
        }
    }

    void DrawMap()
    {
        if (rects == null)
            return;
        if (rects.Length == 0)
            return;

        scrollPos = GUI.BeginScrollView(new Rect(scrollStartPos, scrollArea),
            scrollPos,
            new Rect(0, 0, data.source.mapSize.x * boxSize + 50, data.source.mapSize.y * boxSize + 50));

        for(int y = 0; y < data.source.mapSize.y; y++)
        {
            for(int x = 0; x < data.source.mapSize.x; x++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                string s = x + "," + y;
                if (data.source.playerSpawn == coord)
                    s = "P";
                if (data.source.enemySpawns.Contains(coord))
                    s = "E";
                WallType c = WallType.NONE;
                if (data.source.walls.Contains(coord))
                    c = WallType.WALL;
                DrawColorBox(rects[x, y], s, c);

            }
        }
        GUI.EndScrollView();
    }
    
    Vector2Int GetCoordWithPosition(Vector2 pos)
    {
        if (gridRect.Contains(pos) == false)
            return new Vector2Int(-1, -1);
        Vector2 result = pos;
        result += scrollPos;
        result -= gridStartPos;
        result.x = Mathf.FloorToInt(result.x / boxSize);
        result.y = data.source.mapSize.y - Mathf.FloorToInt(result.y / boxSize);
        return new Vector2Int((int)result.x, (int)result.y);
    }

    void DrawColorBox(Rect r,GUIContent content ,WallType n2c)
    {
        DrawColorBox(r, content, colors[(int)n2c]);
    }

    void DrawColorBox(Rect r, string s, WallType n2c)
    {
        GUIContent content = new GUIContent(s);
        DrawColorBox(r, content, colors[(int)n2c]);
    }

    void DrawColorBox(Rect r, string s, Color c)
    {
        GUIContent content = new GUIContent(s);
        DrawColorBox(r, content, c);
    }

    void DrawColorBox(Rect r, GUIContent content, Color c)
    {
        Color originC = GUI.backgroundColor;
        GUI.backgroundColor = c;
        GUI.Box(r, content);
        GUI.backgroundColor = originC;
    }
    
    void SetWall(Vector2Int pos)
    {
        data.SetWall(pos);
        return;
    }

    void SetEnemySpawn(Vector2Int pos)
    {
        data.SetEnemySpawn(pos);

    }

    void SetPlayerSpawn(Vector2Int pos)
    {
        data.SetPlayerSpawn(pos);            
    }

    void ClickEvent(Vector2Int pos)
    {
        switch (brushNow)
        {
            case BrushType.WALL:
                SetWall(pos);
                break;
            case BrushType.ENEMYSPAWN:
                SetEnemySpawn(pos);
                break;
            case BrushType.PLAYERSPAWN:
                SetPlayerSpawn(pos);
                break;
        }
        
        this.Repaint();
    }

}