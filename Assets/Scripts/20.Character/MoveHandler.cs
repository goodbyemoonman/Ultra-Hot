using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour {
    Rigidbody2D rgbd;
    bool sw = true;
    float speed = 2f;  //1초에 2Unit 이동합니다.
    Vector2 prePos;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        GetComponent<HealthManager>().CharaStateTeller += StateObserver;
        if (gameObject.CompareTag("Enemy"))
            speed = 1.5f;
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
    
    public void LookAt(float angle)
    {
        gameObject.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public void StopMove()
    {
        rgbd.velocity = Vector2.zero;
    }
}
