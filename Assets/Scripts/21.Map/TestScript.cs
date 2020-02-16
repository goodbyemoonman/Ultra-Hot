using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public float x;
    public GameObject prefab;
    readonly float rotateAngleRad = 1.107f;
    readonly float baseRotateAngleRad = 2.3f;
    List<GameObject> prefabs = new List<GameObject>();
    public Vector3 center;
    public Vector3 target;

    private void OnEnable()
    {
        Debug.Log(Vector3.SignedAngle(Vector3.right, target - center, -Vector3.right));
    }

    Vector3 GetRayHitPos(Vector3 center, Vector3 target)
    {
        RaycastHit2D hit;
        int wallLayer = 1 << LayerMask.NameToLayer("Wall");
        //(5, 10) 을 원점 기준으로 회전시켰기 때문에 모든 점의 거리는 동일하다
        float distance = 11.18f;
        hit = Physics2D.Raycast(center, target - center, distance, wallLayer);
        if (hit.collider != null)
        {
            return hit.point;
        }
        else
            return target;
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

    Vector3[] GetCurvedFogPositon(Vector3 baseLinePos)
    {
        float distance = GetDistance(GetDiameter(baseLinePos));
        int count = CountFogPositionOnCurve(distance);
        Debug.Log("Count : " + count.ToString());
        if (count == 0)
            return null;
        float rotateAngle = baseRotateAngleRad / count;
        float cos = Mathf.Cos(-rotateAngle);
        float sin = Mathf.Sin(-rotateAngle);
        Vector3[] result = new Vector3[count];
        result[0] = baseLinePos;
        result[result.Length - 1] = baseLinePos;
        result[result.Length - 1].y *= -1;
        for (int i = 1; i < count * 0.5f; i++)
        {
            result[i] = new Vector3(
                cos * result[i - 1].x - sin * result[i - 1].y,
                sin * result[i - 1].x + cos * result[i - 1].y);
            //count가 홀수 일 때 연산을 두번 하게 되지만 큰 상관은 없는것같음. 
            result[result.Length - 1 - i] = new Vector3(
                result[i].x, -result[i].y);

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
