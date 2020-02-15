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
    readonly float rotateAngleRad = 1.107f;     //63도 정도?
    readonly float baseRotateAngleRad = 2.3f; //120도 정도?
    RaycastHit2D hit;

    private void Awake()
    {
        GetComponent<GameManager>().GameStateObserver += GameStateObserve;
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
    }

    public void GameStateObserve(GAMESTATE state)
    {
        if (state == GAMESTATE.MAP_READY)
            StartCoroutine(FogUpdate());
    }
    
    IEnumerator FogUpdate()
    {
        while(true)
        {
            SetFog();
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    void SetFog()
    {
        Vector3 center = playerTf.position;
        float angle = playerTf.eulerAngles.z;
        //(0, 0)을 중심으로 베이스 라인 그리기
        Vector3[] basePos = GetBaseLinePositions();
        //베이스라인 회전시키면서 시야 그리기.
        Vector3[] fogpos = new Vector3[0];
        for(int i = 0; i < basePos.Length; i++)
        {
            //현재 점과 캐릭터 사이의 거리를 반지름(half diameter)이라 했을 때
            //시야각만큼 회전시 나온 distance를 갖고 
            //distance의 0.5 마다 시야를 하나씩 배치
            fogpos = ArrayAppend(
                fogpos,
                GetCurvedFogPositon(
                    GetDistance(GetDiameter(basePos[i])), 
                    basePos[i]));
        }
        //만들어진 시야 캐릭터의 방향에 맞게 회전
        fogpos = RotatePosition(fogpos, angle);
        //시야 잘라내기
        int count = 0;
        for (int i = 0; i < fogpos.Length; i++)
        {
            fogpos[i] += center;
            
            if (IsOnScreen(fogpos[i]) && !IsFogByWall(center, fogpos[i]))
            {
                while (count > liveSight.Count - 1)
                    TurnSightOn();
                liveSight[count].transform.position = fogpos[i];
                count++;
            }
        }

        if(count < liveSight.Count)
        {
            TurnSightOff(liveSight.Count - count);
        }

    }

    Vector3[] GetBaseLinePositions()
    {
        //this function will return Vector in y = 2x 
        // 0.25 <= x <= 6  // 0 <= y <= 12 // number of result = 12 * 4 + 1;
        // x가 6일때의 길이와 시야의 길이가 맵에서 가장 길 때의 길이가 같음.
        int maxNum = maxSight * 3 + 1;
        Vector3[] result = new Vector3[maxNum];
        float x = 0.33f;
        for (int i = 0; i < maxNum; i++)
        {
            result[i] = new Vector3(x, 2 * x, 0);
            x += (maxX - 0.033f) / (maxNum);
        }
        return result;
    }

    Vector3[] RotatePosition(Vector3[] pos, float degree)
    {
        Vector3[] result = new Vector3[pos.Length];
        float cos = Mathf.Cos(degree * Mathf.Deg2Rad);
        float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
        for (int i = 0; i < pos.Length; i++)
        {
            result[i].x = pos[i].x * cos - pos[i].y * sin;
            result[i].y = pos[i].x * sin + pos[i].y * cos;
            result[i].z = 0;
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

    float GetDiameter(Vector2 pos)
    {
        return (pos.x * Mathf.Cos(-rotateAngleRad) - pos.y * Mathf.Sin(-rotateAngleRad)) * 2f;
    }

    float GetDistance(float diameter)
    {
        return diameter * Mathf.PI * (2 * Mathf.PI / baseRotateAngleRad);
    }

    Vector3[] GetCurvedFogPositon(float distance, Vector3 baseLinePos)
    {
        int count = CountFogPositionOnCurve(distance);
        if (count == 0)
            return null;
        float rotateAngle = baseRotateAngleRad / count;
        float cos = Mathf.Cos(-rotateAngle);
        float sin = Mathf.Sin(-rotateAngle);
        Vector3[] result = new Vector3[count];
        result[0] = baseLinePos;
        result[result.Length - 1] = baseLinePos;
        result[result.Length - 1].y *= -1;
        for(int i = 1; i < count * 0.5f; i++)
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

    int CountFogPositionOnCurve(float distance)
    {
        return Mathf.CeilToInt(distance * 2f);
    }
}
