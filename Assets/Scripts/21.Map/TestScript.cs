using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public int x;
    public Vector3 v;
    public float angle;
    public Vector3 center;

    private void OnEnable()
    {
        Debug.Log(IsOnScreen(v));
    }


    int[] GetY(int x)
    {
        int[] result = new int[1 + (x * 4)];
        for (int i = 0; i < (x * 4) + 1; i++)
        {
            result[i] = i - (x * 2);
        }
        return result;
    }

    Vector3 RotatePosition(Vector3 sp, float degree, Vector3 center)
    {
        float cos = Mathf.Cos(degree * Mathf.Deg2Rad);
        float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
        Debug.Log("Cos >> " + cos + " Sin >> " + sin);
            Vector3 originPos = sp;
            originPos -= center;
            sp.x = originPos.x * cos - originPos.y * sin;
            sp.y = originPos.x * sin + originPos.y * cos;
            sp.z = 0;
            sp += center;

        return sp;
    }
    
    bool IsOnScreen(Vector3 pos)
    {
        if (Mathf.Abs(pos.x) > 20)
            return false;
        if (Mathf.Abs(pos.y) > 12)
            return false;

        return true;
    }
}
