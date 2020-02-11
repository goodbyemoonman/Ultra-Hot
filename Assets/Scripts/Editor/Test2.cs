using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Test2 : EditorWindow
{
    static MapXmlParser data;
    BrushType brushNow = BrushType.WALL;
    Vector2Int tmpSom;
    Vector2 scrollPos;
    Rect[,] rects;

    readonly Vector2 gridStartPos = new Vector2(55, 60);
    readonly Vector2 scrollStartPos = new Vector2(30, 60);
    readonly Vector2 scrollArea = new Vector2(400, 250);
    readonly Vector2 boxSize = new Vector2Int(25, 25);
    readonly Rect gridRect = new Rect(30 + 25, 60 + 25, 400 - 65, 250 - 65);
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
        data = CreateInstance<MapXmlParser>();
        Test2 window = GetWindow<Test2>("Map Editor");
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
                GenerateMap();
            }
        }
        EditorGUILayout.EndHorizontal();

        DrawMap();
        DrawMouseOverlapBox();
        

        if (Event.current.type == EventType.MouseDown)
        {
            Debug.Log(GetCoordWithPosition(Event.current.mousePosition));
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
    }

    void GenerateMap()
    {
        data.SetMapSize(tmpSom);
        scrollPos = Vector2.zero;
        rects = new Rect[data.source.mapSize.x, data.source.mapSize.y];
        data.source.walls.Clear();
        data.source.enemySpawns.Clear();
        data.SetPlayerSpawn(Vector2Int.zero);
        data.SetMapName("New Map");

        for (int y = 0; y < data.source.mapSize.y; y++)
        {
            for(int x = 0; x < data.source.mapSize.x; x++)
            {
                rects[x, y] = new Rect(
                    25 * x + 25, (data.source.mapSize.y * 25 - 25 * y),
                    25, 25);
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
            new Rect(0, 0, data.source.mapSize.x * 25 + 50, data.source.mapSize.y * 25 + 50));

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

    void DrawMouseOverlapBox()
    {
        Color c = colors[(int)brushNow];
        c.a = 0.5f;
        if (rects == null)
            return;
        IEnumerator<Rect> enumerator = 
            GetRectsInRect(rects, new Rect(gridStartPos, scrollArea)).GetEnumerator();
        while (enumerator.MoveNext())
        {
            DrawColorBox(enumerator.Current, GUIContent.none, c);
        }
    }
    
    IEnumerable<Rect> GetRectsInRect(Rect[,] rectsPool, Rect targetRect)
    {
        foreach (Rect r in rectsPool)
        {
            Rect newR = r;
            newR.position -= scrollPos;
            newR.position += scrollStartPos;
            if (targetRect.Overlaps(newR))
            {
                if (newR.Contains(Event.current.mousePosition))
                {
                    yield return newR;
                }
            }
        }
    }
    
    Vector2Int GetCoordWithPosition(Vector2 pos)
    {
        if (gridRect.Contains(pos) == false)
            return new Vector2Int(-1, -1);
        Vector2 result = pos;
        result += scrollPos;
        result -= gridStartPos;
        result.x = Mathf.FloorToInt(result.x / 25);
        result.y = data.source.mapSize.y - Mathf.FloorToInt(result.y / 25);
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
        Debug.Log(pos + " Set Wall. ");
        return;
    }

    void SetEnemySpawn(Vector2Int pos)
    {
        data.SetEnemySpawn(pos);
        Debug.Log(pos + " Set Enemy Spawn. ");

    }

    void SetPlayerSpawn(Vector2Int pos)
    {
        data.SetPlayerSpawn(pos);            
        Debug.Log(pos + " Set Player Spawn. ");
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
    }

}