using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Vector2 target;
    Rigidbody2D rgbd;
    float speed = 2f;
    Vector2 prePos;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CallBackMoveDir(target, "Callback");
    }

    void Callback()
    {
        Debug.Log("Arrive to " + target);
    }

    public void CallBackMoveDir(Vector2 targetPos, string callbackName)
    {
        if (rgbd == null)
            Awake();
        if ((prePos - (Vector2)transform.position).magnitude > 1)
            prePos = transform.position;
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.eulerAngles = new Vector3(0, 0, angle);
        rgbd.velocity = dir * speed;

        if (IsTargetInside(targetPos))
        {
            SendMessage(callbackName);
        }
        else
            prePos = transform.position;
    }

    bool IsTargetInside(Vector2 targetPos)
    {
        float preToNow = (prePos - (Vector2)transform.position).magnitude;
        float preToTarget = (prePos - targetPos).magnitude;
        float targetToNow = (targetPos - (Vector2)transform.position).magnitude;

        if (preToNow < preToTarget)
            return false;
        if (preToNow < targetToNow)
            return false;

        return true;
    }

}

