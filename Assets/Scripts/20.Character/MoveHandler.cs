using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour {
    Rigidbody2D rgbd;
    readonly float speed = 4f;

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
    }


    public void MoveToWorldDir(Vector2 dir)
    {
        dir *= speed;

        rgbd.velocity = dir;
    }

    public void MoveToSelfDir(Vector2 dir)
    {
        dir *= speed;

        float angle = Mathf.Deg2Rad * transform.eulerAngles.z;

        Vector2 newDir = new Vector2(
            (Mathf.Cos(angle) * dir.x) + (-Mathf.Sin(angle) * dir.y), 
            (Mathf.Sin(angle) * dir.x) + (Mathf.Cos(angle) * dir.y));
        rgbd.velocity = newDir;
    }
}
