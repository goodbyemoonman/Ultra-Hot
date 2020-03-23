using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEditor;


public class ObjPoolRWHelper : ScriptableObject
{
    string path = "Resources/ObjectPoolData/";
    public ObjPool data;

    public void SaveData()
    {
        string fullPath = Path.Combine(Application.dataPath, path);
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);
        fullPath += "/ObjectPoolData.xml";
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        using (StreamWriter sw = new StreamWriter(fullPath))
        {
            XmlSerializer xs = new XmlSerializer(data.GetType());
            xs.Serialize(sw, data);
        }
    }

    public void LoadData()
    {
        TextAsset text = Resources.Load<TextAsset>("ObjectPoolData/ObjectPoolData");
        if (text == null || text.text == null)
        {
            //Debug.Log("can not found ObjectPoolData.xml");
            data = new ObjPool();
            return;
        }
        StringReader sr = new StringReader(text.text);
        XmlSerializer xs = new XmlSerializer(typeof(ObjPool));
        data = xs.Deserialize(sr) as ObjPool;
        sr.Close();

        if (data == null)
            data = new ObjPool();

        data.LoadPrefab();
        return;
    }

    public void AddData()
    {
        if (data.sources == null)
            data.sources = new List<ObjPoolSource>();
        ObjPoolSource source = new ObjPoolSource();
        data.sources.Add(source);
    }

    public void RemoveData(int idx)
    {
        if (idx < 0 || data.sources.Count - 1 < idx)
            return;

        data.sources.RemoveAt(idx);
    }

    public void ClearData()
    {
        data.sources.Clear();
    }
}