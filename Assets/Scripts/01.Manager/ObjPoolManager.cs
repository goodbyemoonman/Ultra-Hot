using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPoolManager : Singleton<ObjPoolManager> {
    ObjPoolRWHelper reader;

    private void OnEnable()
    {
        reader = ScriptableObject.CreateInstance<ObjPoolRWHelper>();
        reader.LoadData();
        for(int i = 0; i < reader.data.sources.Count; i++)
        {
            GameObject parent = new GameObject(reader.data.sources[i].GetOrigin().name + " parent");
            reader.data.sources[i].SetParent(parent.transform);
            reader.data.sources[i].Initialize();
        }
    }
    
    public GameObject GetObject(ObjectPoolList list)
    {
        return reader.data.sources[(int)list].GetObj();
    }

    public void ReturnObject(GameObject go)
    {
        reader.data.ReturnObj(go);
    }
}
