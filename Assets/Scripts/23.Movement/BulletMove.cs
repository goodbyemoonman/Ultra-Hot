using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour {
    TrailRenderer tr;
    readonly int dmg = 3;
    readonly int speed = 6;
    readonly float deadSpeed = 0.2f;
    bool isCrashed = false;
    float t = 0;
    
    private void OnEnable()
    {
        t = 0;
        tr = GetComponent<TrailRenderer>();
        isCrashed = false;
        tr.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (isCrashed)
            return;
        isCrashed = true;

        if (collision.gameObject.layer != LayerMask.NameToLayer("Wall"))
            collision.SendMessage("GetDamaged", dmg);
    }

    // Update is called once per frame
    void Update () {
        if (isCrashed == false)
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        else
        {
            transform.Translate(Vector3.right * Time.deltaTime * deadSpeed);
            t += Time.deltaTime * 5;
        }

        if (t > 1)
            ObjPoolManager.Instance.ReturnObject(gameObject);
    }
}
