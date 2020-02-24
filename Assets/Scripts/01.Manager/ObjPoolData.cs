using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjPoolSource : ScriptableObject
{
    GameObject originObj;
    Transform parent;
    public int count;
    public string prefabPath;
    List<GameObject> objs;

    private void OnEnable()
    {
        LoadPrefab();
        objs = new List<GameObject>();
    }

    public void LoadPrefab()
    {
        if (originObj != null)
            return;
        originObj = Resources.Load<GameObject>(prefabPath);
        if (originObj == null)
            Debug.Log("can not found " + prefabPath);
    }

    public void SetParent(Transform inputParent)
    {
        parent = inputParent;
    }

    void AddObj()
    {
        GameObject go = Instantiate(originObj, parent);
        go.name = originObj.name;
        go.SetActive(false);
        objs.Add(go);
    }

    public GameObject GetObj()
    {
        GameObject go;
        if (objs.Count == 0)
        {
            AddObj();
        }

        go = objs[objs.Count - 1];
        objs.RemoveAt(objs.Count - 1);

        return go;
    }

    public void ReturnObj(GameObject go)
    {
        if (go.name != originObj.name)
        {
            return;
        }
        go.transform.SetParent(parent);
        go.SetActive(false);
        objs.Add(go);
    }

    public bool IsThisPoolObj(GameObject go)
    {
        if (go.name != originObj.name)
            return false;
        return true;
    }

    public void Initialize()
    {
        if (originObj == null)
            LoadPrefab();
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(originObj, parent);
            go.name = originObj.name;
            objs.Add(go);
        }
    }

    public GameObject GetOrigin()
    {
        return originObj;
    }
}

[System.Serializable]
public class ObjPool
{
    public List<ObjPoolSource> sources;

    public void LoadPrefab()
    {
        if (sources == null)
            sources = new List<ObjPoolSource>();
        for (int i = 0; i < sources.Count; i++)
            sources[i].LoadPrefab();
    }

    public void ReturnObj(GameObject go)
    {
        for (int i = 0; i < sources.Count; i++)
        {
            if (go.name == sources[i].GetOrigin().name)
            {
                sources[i].ReturnObj(go);
                return;
            }
        }
        Debug.Log("Can not find Obj Pool : " + go.name);
        go.SetActive(false);
    }
}