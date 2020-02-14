using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public float x;
    public GameObject prefab;
    readonly float rotateAngleRad = 1.107f;
    readonly float baseRotateAngleRad = 2.3f;
    List<GameObject> prefabs = new List<GameObject>();

    private void OnEnable()
    {
        Vector3 pos = new Vector3(1, 0, 0);
        Vector3[] poses = GetBaseLinePositions(pos);
        Vector3 pos2 = Vector3.zero;
        Vector3[] pos2s = GetBaseLinePositions(pos2);
        poses = ArrayAppend(poses, pos2s);
        for(int i = 0; i < poses.Length; i++)
        {
            if(prefabs.Count - 1 < i)
            {
                prefabs.Add(Instantiate(prefab));
            }

            prefabs[i].transform.position = poses[i];
        }
    }
    
    Vector3[] ArrayAppend(Vector3[] left, Vector3[] right)
    {
        Vector3[] result = new Vector3[left.Length + right.Length];
        left.CopyTo(result, 0);
        right.CopyTo(result, left.Length);
        return result;
    }


    Vector3[] GetBaseLinePositions(Vector3 startPos)
    {
        //this function will return Vector in y = 2x 
        // 0 <= x <= 6  // 0 <= y <= 12 // number of result = 12 * 4 + 1;
        Vector3[] result = new Vector3[12 * 4 + 1];
        float xx = 0.25f;
        for(int i = 0; i < 49; i++)
        {
            result[i] = new Vector3(xx, xx * 2f, 0);
            result[i] += startPos;
            xx += (6 - 0.25f) / 49f;
        }

        return result;
    }


    int CountFogPositionOnCurve(float distance)
    {
        return Mathf.CeilToInt(distance * 2f);
    }

    Vector3[] GetCurvedFogPositon(float distance, Vector3 basePosition)
    {
        int count = CountFogPositionOnCurve(distance);
        if (count == 0)
            return null;
        float rotateAngle = baseRotateAngleRad / count;
        float cos = Mathf.Cos(-rotateAngle);
        float sin = Mathf.Sin(-rotateAngle);
        Vector3[] result = new Vector3[count];
        result[0] = basePosition;
        result[result.Length - 1] = basePosition;
        result[result.Length - 1].y *= -1;
        for (int i = 1; i < count * 0.5f; i++)
        {
            result[i] = new Vector2(
                cos * result[i - 1].x - sin * result[i - 1].y,
                sin * result[i - 1].x + cos * result[i - 1].y);
            //count가 홀수 일 때 연산을 두번 하게 되지만 큰 상관은 없는것같음. 
            result[result.Length - 1 - i] = new Vector2(result[i].x, -result[i].y);

        }
        return result;
    }

    float GetDistance(float diameter)
    {
        return diameter * Mathf.PI * 0.333f;
    }

    float GetDiameter(Vector2 pos)
    {
        return (pos.x * Mathf.Cos(-rotateAngleRad) - pos.y * Mathf.Sin(-rotateAngleRad)) * 2;
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
