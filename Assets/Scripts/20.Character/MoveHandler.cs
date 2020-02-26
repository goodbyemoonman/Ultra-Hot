using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour {
    Rigidbody2D rgbd;
    bool sw = true;
    readonly float speed = 2f;

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


    public void MoveToWorldDir(Vector2 dir)
    {
        if (sw == false)
            return;

        dir *= speed;

        rgbd.velocity = dir;
    }

    public void MoveToSelfDir(Vector2 dir)
    {
        if (sw == false)
            return;
        if (rgbd == null)
            Awake();

        dir *= speed;

        float angle = Mathf.Deg2Rad * transform.eulerAngles.z;

        Vector2 newDir = new Vector2(
            (Mathf.Cos(angle) * dir.x) + (-Mathf.Sin(angle) * dir.y), 
            (Mathf.Sin(angle) * dir.x) + (Mathf.Cos(angle) * dir.y));
        rgbd.velocity = newDir;
    }
}
