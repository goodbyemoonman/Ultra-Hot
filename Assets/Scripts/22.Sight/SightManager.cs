using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightManager : MonoBehaviour
{
    int wallLayer;
    public GameObject sightPrefab;
    public Transform playerTf;
    public Transform cameraTf;
    List<GameObject> deadSight;
    List<GameObject> liveSight;
    readonly int maxWidth = 10;
    readonly int maxX = 6;
    readonly int maxY = 12;
    readonly int maxSight = 12;
    readonly int maxHeight = 6;
    RaycastHit2D hit;

    private void Awake()
    {
        wallLayer = 1 << LayerMask.NameToLayer("Wall");
        deadSight = new List<GameObject>();
        liveSight = new List<GameObject>();
        cameraTf = Camera.main.transform;
        for (int i = 0; i < maxWidth * maxHeight; i++)
        {
            GameObject go = Instantiate(sightPrefab, transform);
            go.SetActive(false);
            deadSight.Add(go);
        }
        StartCoroutine(FogUpdate());
    }
    
    IEnumerator FogUpdate()
    {
        while(true)
        {
            SetFog();
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    void SetFog()
    {
        Vector3 center = playerTf.position;
        float angle = playerTf.eulerAngles.z;
        int count = 0;
        for(float f = 0; f < maxSight; f += 0.5f)
        {
            Vector3[] newPos = GetSightPosition(f, center);
            newPos = RotatePosition(newPos, angle, center);
            for(int j = 0; j < newPos.Length; j++)
            {
                if (IsOnScreen(newPos[j]) && !IsFogByWall(center, newPos[j]))
                {
                    while (count > liveSight.Count - 1)
                        TurnSightOn();
                    liveSight[count].transform.position = newPos[j];
                    count++;
                }
            }
        }

        //할당된 시야가 살아있는 시야보다 많다면 남은 살아있는 시야를 죽임.
        if(count < liveSight.Count)
        {
            TurnSightOff(liveSight.Count - count);
        }
    }

    Vector3[] GetSightPosition(float x, Vector3 center)
    {
        //this function will return y in "y = 2|x - center.x| + center.y" 
        Vector3[] result = new Vector3[1 + (int)(x * 4 * 2)];
        for (int i = 0; i < (x * 4 * 2) + 1; i++)
        {
            result[i] = new Vector3(x, i * 0.5f - (x * 2), 0);
            result[i] += center;
        }
        return result;
    }

    Vector3[] GetBaseLinePositions(Vector3 startPos)
    {
        //this function will return Vector in y = 2x 
        // 0 <= x <= 6  // 0 <= y <= 12 // number of result = 12 * 4 + 1;
        Vector3[] result = new Vector3[maxSight * 4 + 1];
        int count = 0;
        for(float x = 0; x <= maxX; x += (6 / 48))
        {
            result[count] = new Vector3(x, x * 2f, 0);
            result[count] += startPos;
            count++;
        }
        return result;
    }

    Vector3[] RotatePosition(Vector3[] pos, float degree, Vector3 center)
    {
        Vector3[] result = new Vector3[pos.Length];
        float cos = Mathf.Cos(degree * Mathf.Deg2Rad);
        float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] -= center;
            result[i].x = pos[i].x * cos - pos[i].y * sin;
            result[i].y = pos[i].x * sin + pos[i].y * cos;
            result[i].z = 0;
            result[i] += center;
        }

        return result;
    }

    Vector3[] ArrayAppend(Vector3[] left, Vector3[] right)
    {
        Vector3[] result = new Vector3[left.Length + right.Length];
        left.CopyTo(result, 0);
        right.CopyTo(result, left.Length);
        return result;
    }

    bool IsOnScreen(Vector3 pos)
    {
        if (Mathf.Abs(pos.x - cameraTf.position.x) > maxWidth)
            return false;
        if (Mathf.Abs(pos.y - cameraTf.position.y) > maxHeight)
            return false;

        return true;
    }

    void TurnSightOn()
    {
        if (deadSight.Count != 0)
        {
            liveSight.Add(deadSight[deadSight.Count - 1]);
            liveSight[liveSight.Count - 1].SetActive(true);
            deadSight.RemoveAt(deadSight.Count - 1);
        }
        else
        {
            liveSight.Add(Instantiate(sightPrefab, transform));
            liveSight[liveSight.Count - 1].SetActive(true);
        }
    }

    void TurnSightOff(int count)
    {
        for(int i = 0;i < count; i++)
        {
            liveSight[liveSight.Count - 1].SetActive(false);
            deadSight.Add(liveSight[liveSight.Count - 1]);
            liveSight.RemoveAt(liveSight.Count - 1);
        }
    }

    bool IsFogByWall(Vector3 pos, Vector3 pos2)
    {
        float lv = (pos2 - pos).x;
        float rv = (pos2 - pos).y;
        float distance = Mathf.Sqrt(lv * lv + rv * rv);
        hit = Physics2D.Raycast(pos, pos2 - pos, distance, wallLayer);
        if (hit.collider == null)
            return false;
        else
            return true;
    }
}
