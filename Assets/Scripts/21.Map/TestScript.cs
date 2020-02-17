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
}
