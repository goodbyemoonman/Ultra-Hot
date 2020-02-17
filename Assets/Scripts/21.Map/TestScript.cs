using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    public Transform testTf;
    public Vector2 origin;
    public Vector2 target;

    private void OnEnable()
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, target, (target - origin).magnitude);
        Debug.Log(hit.point);
    }
}
