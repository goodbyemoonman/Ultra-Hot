using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Test2 : EditorWindow
{
    int tmp;
    Vector2Int sizeOfMap;
    Vector2Int tmpSom;
    readonly float boxSize = 23f;
    Vector2 scrollPos;
    Event e;
    Texture2D texture;
    GUIStyle style;

    [MenuItem("Window/Test2 Window")]
    static void Init()
    {
        Test2 window = GetWindow<Test2>();
        window.Show();
    }
    //
    //private void OnGUI()
    //{
    //    Rect workSpace = GUILayoutUtility.GetRect(1600, 1000);
    //
    //    sizeOfMap = EditorGUI.Vector2IntField(
    //        new Rect(0, 0, 200, EditorGUIUtility.singleLineHeight),
    //        new GUIContent("맵의 크기 : "),
    //        sizeOfMap);
    //    sizeOfMap.x = Mathf.Clamp(sizeOfMap.x, 0, 100);
    //    sizeOfMap.y = Mathf.Clamp(sizeOfMap.y, 0, 100);
    //
    //    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true,
    //        GUILayout.Width(250), GUILayout.Height(250));
    //    for (int y = 0; y < sizeOfMap.y; y++)
    //    {
    //        {
    //            for (int x = 0; x < sizeOfMap.x; x++)
    //            {
    //                EditorGUI.DrawRect(
    //                    new Rect(x * (boxSize + 2) + 30,
    //                    EditorGUIUtility.singleLineHeight * 2f +
    //                    y * (boxSize + 2) + 30,
    //                    boxSize + 2,
    //                    boxSize + 2),
    //                    Color.black);
    //                EditorGUI.DrawRect(
    //                    new Rect(x * (boxSize + 2) + 1 + 30,
    //                    EditorGUIUtility.singleLineHeight * 2f +
    //                    y * (boxSize + 2) + 1 + 30,
    //                    boxSize,
    //                    boxSize),
    //                    Color.gray);
    //                //GUILayout.Button("", GUILayout.MaxWidth(EditorGUIUtility.singleLineHeight));
    //
    //            }
    //        }
    //    }
    //    EditorGUILayout.EndScrollView();
    //    if (Event.current.type == EventType.DragExited)
    //    {
    //        Debug.Log("DragExit event, mousePos:" + Event.current.mousePosition +
    //            "window pos:" + position);
    //    }
    //    GUILayout.EndArea();
    //
    //}
    //
    Rect[,] rects;
    Color originColor;

    private void OnGUI()
    {
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
        FindHoverBox();

        //if (Event.current.type == EventType.MouseDown)
        //    TestFunc1(e);
        //
        //if (Event.current.type == EventType.MouseDrag)
        //    TestFunc1(e);
        //
        //if (Event.current.type == EventType.MouseUp)
        //    Debug.Log("mouseUp");
        //
        //TestFunc2(e);
    }

    void TestFunc1(Event e)
    {
        for(int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (rects[x, y].Contains(e.mousePosition))
                {
                    Debug.Log("[" + x + "," + y + "]");
                    return;
                }
            }
        }
    }

    void TestFunc2(Event e)
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (rects[x, y].Contains(e.mousePosition))
                {
                    GUI.backgroundColor = Color.black;
                    GUI.Box(rects[x, y], "");
                    GUI.backgroundColor = originColor;
                    return;
                }
            }
        }
    }

    void GenerateMap()
    {
        sizeOfMap = tmpSom;
        e = Event.current;
        originColor = Color.grey;
        texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        style = new GUIStyle { normal = new GUIStyleState { background = texture } };
        scrollPos = Vector2.zero;
        rects = new Rect[sizeOfMap.x, sizeOfMap.y];

        for (int y = 0; y < sizeOfMap.y; y++)
        {
            for(int x = 0; x < sizeOfMap.x; x++)
            {
                rects[x, y] = new Rect(
                    25 * x, (sizeOfMap.y * 25 - 25 * y),
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

        scrollPos = GUI.BeginScrollView(new Rect(30, 60, 400, 250),
            scrollPos,
            new Rect(0, 0, sizeOfMap.x * 25, sizeOfMap.y * 25));

        for(int y = 0; y < sizeOfMap.y; y++)
        {
            for(int x = 0; x < sizeOfMap.x; x++)
            {
                GUI.Box(rects[x, y], x + "," + y);
            }
        }
        GUI.EndScrollView();
    }

    void FindHoverBox()
    {
        if (rects == null)
            return;
        foreach(Rect r in rects)
        {
            Vector2 startPos = new Vector2(30, 60);
            Rect newR = r;
            newR.position -= scrollPos;
            newR.position += startPos;
            if ((new Rect(30, 60, 360, 210)).Overlaps(newR))
            {
                if (newR.Contains(Event.current.mousePosition))
                {
                    style.normal.textColor = Color.yellow;
                    GUI.Box(newR, "", style);
                }
            }
        }
    }
}
