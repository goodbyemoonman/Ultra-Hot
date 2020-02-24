using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

public class ObjectPoolEditor : EditorWindow {
    static ObjPoolRWHelper readWriter;
    Vector2 scrollPos;

    [MenuItem("Window/Object Pool Editor Window")]
    static void Init()
    {
        readWriter = CreateInstance<ObjPoolRWHelper>();
        readWriter.LoadData();
        ObjectPoolEditor window = GetWindow<ObjectPoolEditor>("Object Pool Editor");
        window.Show();
    }

    private void OnGUI()
    {
        if (readWriter == null)
            return;

        EditorGUILayout.BeginVertical(GUILayout.Width(420));
        {
            EditorGUILayout.Space();

            PrintToolBar();

            EditorGUILayout.Space();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(400));
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(380));
                {
                    for (int i = 0; readWriter.data.sources != null && i < readWriter.data.sources.Count; i++)
                    {
                        PrintObjPool(i);
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add Obj", GUILayout.Width(200)))
                    {
                        readWriter.AddData();
                        Repaint();
                    }

                    if (GUILayout.Button("Create Enum List", GUILayout.Width(200)))
                    {
                        MakeEnum();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

        }
        EditorGUILayout.EndVertical();
    }

    void PrintToolBar()
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Load", GUILayout.Width(200)))
            {
                readWriter.LoadData();
                Repaint();
            }
            if (GUILayout.Button("Save", GUILayout.Width(200)))
            {
                readWriter.SaveData();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void PrintObjPool(int idx)
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(360));
        {
            GameObject go = EditorGUILayout.ObjectField(
                "PreFab Object", 
                readWriter.data.sources[idx].GetOrigin(), 
                typeof(GameObject), 
                false) 
                as GameObject;
            if (go != null)
            {
                readWriter.data.sources[idx].prefabPath = PathCutter(AssetDatabase.GetAssetPath(go));
                readWriter.data.LoadPrefab();
            }

            readWriter.data.sources[idx].count = Mathf.Clamp(EditorGUILayout.IntField(
                "Initialize Count", readWriter.data.sources[idx].count), 0, int.MaxValue);

            if(GUILayout.Button("Delete This Obj"))
            {
                readWriter.RemoveData(idx);
                Repaint();
            }
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
    }

    void MakeEnum()
    {
        string path = "Assets/Template/EnumTemplate.txt";
        string enumTemplate = File.ReadAllText(path);
        string objPoolList = "";
        string data = "";
        for(int i = 0; i < readWriter.data.sources.Count; i++)
        {
            data += ("\t" + readWriter.data.sources[i].GetOrigin().name + " = " + i.ToString() + ", \n");
        }
        objPoolList = enumTemplate.Replace("$ENUMNAME$", "ObjectPoolList");
        objPoolList = objPoolList.Replace("$ENUMDATA$", data);

        string savePath = "Assets/Scripts/EnumLists/";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        string fullPath = savePath + "ObjectPoolList.cs";
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        File.WriteAllText(fullPath, objPoolList);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    string PathCutter(string path)
    {
        string result = "";
        string[] tmp = path.Split('/', '.');
        //리소스 전 주소와 확장자 잘라내기
        for(int i = 0; i < tmp.Length - 1; i++)
        {
            if (tmp[i] == "Resources")
                result = "";
            else
                result = Path.Combine(result, tmp[i]);
        }

        return result;
    }
}

