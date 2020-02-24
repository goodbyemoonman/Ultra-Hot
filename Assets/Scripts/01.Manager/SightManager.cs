using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightManager : MonoBehaviour
{
    public GameObject circleSightPrefab;
    public GameObject boxSightPrefab;
    public Transform playerTf;
    List<SightData> sight;
    readonly int maxCount = 50;
    readonly float baseRotateAngleRad = 2.3f; //120도 정도?
    readonly Vector3 fogStartPos = new Vector3(5, 10, 0);
    Transform parent;

    private void Awake()
    {
        sight = new List<SightData>();
        parent = new GameObject("Sights").transform;
        parent.SetParent(playerTf);
        parent.localRotation = Quaternion.Euler(0, 0, 0);
        parent.localPosition = Vector3.zero;

        InitSight();
    }

    public void RefreshSight()
    {
        if (sight == null)
            return;
        for (int i = 0; i < sight.Count; i++)
        {
            sight[i].RefreshSight();
        }
    }

    void InitSight()
    {
        Vector3[] fogTargetPos = GetFogTargetPos(fogStartPos);

        for (int i = 0; i < fogTargetPos.Length; i++)
        {
            GameObject newBox = Instantiate(boxSightPrefab, parent);
            GameObject newCircle = Instantiate(circleSightPrefab, parent);
            SightData data = new SightData(newBox, newCircle, fogTargetPos[i]);
            sight.Add(data);
        }
    }


    Vector3[] GetFogTargetPos(Vector3 baseLinePos)
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

}

class SightData
{
    int wallLayer = 1 << LayerMask.NameToLayer("Wall");
    Transform boxTf;
    Transform circleTf;

    //(0, 0)에 위치해 있을때 타겟
    Vector3 targetPos;

    public SightData(GameObject box, GameObject circle, Vector3 target)
    {
        boxTf = box.transform;
        circleTf = circle.transform;
        targetPos = target;
        boxTf.localPosition = Vector3.zero;
        Vector3 newAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, target));
        boxTf.localEulerAngles = newAngles;
        RefreshCirclePosition();
    }

    public void Rotate(float angle)
    {
        boxTf.eulerAngles = new Vector3(0, 0, angle);
    }

    public void RefreshSight()
    {
        RefreshBoxScale();
        RefreshCirclePosition();
    }

    void RefreshBoxScale()
    {
        float cos = Mathf.Cos(boxTf.parent.eulerAngles.z * Mathf.Deg2Rad);
        float sin = Mathf.Sin(boxTf.parent.eulerAngles.z * Mathf.Deg2Rad);
        Vector3 rotatedTarget = new Vector3(
            cos * targetPos.x - sin * targetPos.y,
            sin * targetPos.x + cos * targetPos.y, 0);

        RaycastHit2D hit = Physics2D.Raycast(boxTf.position, rotatedTarget,
            targetPos.magnitude, wallLayer);

        Vector3 newScale = boxTf.localScale;

        if (hit.collider == null)
            newScale.x = (rotatedTarget - boxTf.position).magnitude;
        else
            newScale.x = (hit.point - (Vector2)boxTf.position).magnitude;
        
        boxTf.localScale = newScale;
    }

    void RefreshCirclePosition()
    {
        float cos = Mathf.Cos(boxTf.eulerAngles.z * Mathf.Deg2Rad);
        float sin = Mathf.Sin(boxTf.eulerAngles.z * Mathf.Deg2Rad);
        float distance = boxTf.localScale.x;
        Vector3 newPos = new Vector3(cos * distance, sin * distance, 0);

        circleTf.position = boxTf.position + newPos;
    }
}
