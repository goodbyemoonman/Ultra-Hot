using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject who;
    public GameObject target;

    private void OnEnable()
    {
        Debug.Log(IsLookAt(who, target));
    }


    bool IsLookAt(GameObject who, GameObject target)
    {
        Vector3 pos = target.transform.position - who.transform.position;
        Vector2 dir = Vector2.right;
        float sin = Mathf.Sin(who.transform.eulerAngles.z * Mathf.Deg2Rad);
        float cos = Mathf.Cos(who.transform.eulerAngles.z * Mathf.Deg2Rad);
        dir.x = Vector2.right.x * cos - Vector2.right.y * sin;
        dir.y = Vector2.right.x * sin + Vector2.right.y * cos;

        float angle = Vector2.SignedAngle(dir, (Vector2)pos);
        Debug.Log("angle = " + angle.ToString());
        if (Mathf.Cos(angle * Mathf.Deg2Rad) > 0)
            return true;
        else
            return false;
    }

}

