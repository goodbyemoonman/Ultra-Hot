using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour {
    Rigidbody2D rgbd;
    bool sw = true;
    readonly float speed = 2f;  //1초에 2Unit 이동합니다.
    Vector2 prePos;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        GetComponent<HealthManager>().StateTeller += StateObserver;
    }

    private void OnEnable()
    {
        sw = true;
    }

    void StateObserver(HealthManager.CharacterState state)
    {
        if (state == HealthManager.CharacterState.Sturn)
        {
            rgbd.velocity = Vector2.zero;
            sw = false;
        }
        else
        {
            sw = true;
        }
    }


    public void MoveToWorldDirection(Vector2 dir)
    {
        if (sw == false)
            return;

        dir *= speed;

        rgbd.velocity = dir;
    }

    public void MoveForward()
    {
        if (sw == false)
            return;
        if (rgbd == null)
            Awake();
        Vector2 dir = Vector2.right;
        dir *= speed;

        float angle = Mathf.Deg2Rad * transform.eulerAngles.z;

        Vector2 newDir = new Vector2(
            (Mathf.Cos(angle) * dir.x) + (-Mathf.Sin(angle) * dir.y), 
            (Mathf.Sin(angle) * dir.x) + (Mathf.Cos(angle) * dir.y));
        rgbd.velocity = newDir;
    }

    public void MoveToWorldPosition(Vector2 targetPos)
    {
        if (sw == false)
            return;
        if (rgbd == null)
            Awake();

        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        transform.eulerAngles = new Vector3(0, 0, angle);
        rgbd.velocity = dir * speed;
    }

    public void CallBackMoveDir(Vector2 targetPos, string callbackName)
    {
        if ((prePos - (Vector2)transform.position).magnitude > 1)
            prePos = transform.position;
        MoveToWorldPosition(targetPos);

        if (IsArrive(targetPos))
        {
            SendMessage(callbackName);
        }
        else
            prePos = transform.position;
    }

    bool IsArrive(Vector2 targetPos)
    {
        float preToNow = (prePos - (Vector2)transform.position).magnitude;
        float preToTarget = (prePos - targetPos).magnitude;
        float targetToNow = (targetPos - (Vector2)transform.position).magnitude;

        if (targetToNow < 0.01f)
            return true;

        if (preToNow < preToTarget)
            return false;
        if (preToNow < targetToNow)
            return false;

        return true;
    }

    public void LookAt(float angle)
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
    }
}
