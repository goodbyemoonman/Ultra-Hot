using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightManager : MonoBehaviour
{
    int wallLayer;
    public GameObject circleSightPrefab;
    public GameObject boxSightPrefab;
    public Transform playerTf;
    Transform cameraTf;
    List<GameObject> boxSights;
    List<GameObject> circleSights;
    readonly int maxCount = 25;
    readonly float baseRotateAngleRad = 2.3f; //120도 정도?
    RaycastHit2D hit;
    readonly Vector3 baseFogStartPos = new Vector3(5, 10, 0);
    Transform parent;

    private void Awake()
    {
        GetComponent<GameManager>().GameStateObserver += GameStateObserve;
        wallLayer = 1 << LayerMask.NameToLayer("Wall");
        boxSights = new List<GameObject>();
        circleSights = new List<GameObject>();
        cameraTf = Camera.main.transform;
        parent = new GameObject("Sights").transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            AddMoreCircleSight();
            AddMoreBoxSight();
        }
    }

    public void GameStateObserve(GAMESTATE state)
    {
        if (state == GAMESTATE.MAP_READY)
            StartCoroutine(FogUpdate());
    }
    
    IEnumerator FogUpdate()
    {
        yield return new WaitForSecondsRealtime(1.0f);
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
        Vector3[] baseFogPos = GetCurvedFogPositon(baseFogStartPos);
        baseFogPos = RotatePosition(baseFogPos, angle);
        for(int i = 0;i < baseFogPos.Length; i++)
        {
            baseFogPos[i] += center;
        }
        
        for (int i = 0; i < baseFogPos.Length; i++)
        {
            while (i > circleSights.Count - 1)
                AddMoreCircleSight();
            while (i > boxSights.Count - 1)
                AddMoreBoxSight();

            baseFogPos[i] = GetRayHitPos(center, baseFogPos[i]);
            PutBoxSight(center, baseFogPos[i], boxSights[i]);
            circleSights[i].transform.position = baseFogPos[i];
        }
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

    Vector3[] GetCurvedFogPositon(Vector3 baseLinePos)
    {
        int count = maxCount;
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

    Vector3 GetRayHitPos(Vector3 center, Vector3 target)
    {
        float distance = (target - center).magnitude;
        hit = Physics2D.Raycast(center, target - center, distance, wallLayer);
        if (hit.collider != null)
        {
            return hit.point;
        }
        else
            return target;
    }

    void PutBoxSight(Vector3 center, Vector3 target, GameObject go)
    {
        Vector3 scale = go.transform.localScale;
        scale.x = (target - center).magnitude;
        go.transform.localScale = scale;

        Vector3 rotation = Vector3.zero;
        rotation.z = GetAngleDegree(center, target, AngleType.DEG);
        go.transform.localEulerAngles = rotation;

        go.transform.position = center;
    }

    enum AngleType { DEG, RAD}

    float GetAngleDegree(Vector3 center, Vector3 target, AngleType type)
    {
        float result = Vector3.SignedAngle(Vector3.right, target - center, Vector3.up);
        if ((target - center).y < 0)
            result *= -1f;

        if (type == AngleType.RAD)
            result *= Mathf.Deg2Rad;

        return result;
    }

    void AddMoreBoxSight()
    {
        GameObject box = Instantiate(boxSightPrefab, parent);
        box.SetActive(true);
        boxSights.Add(box);
    }

    void AddMoreCircleSight()
    {
        GameObject circle = Instantiate(circleSightPrefab, parent);
        circle.SetActive(true);
        circleSights.Add(circle);
    }
}
