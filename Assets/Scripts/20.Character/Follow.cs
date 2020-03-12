using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
    public Transform targetTf;
    public Vector2 adder;

    private void LateUpdate()
    {
        Vector3 newPos = transform.position;
        newPos.x = targetTf.position.x + adder.x;
        newPos.y = targetTf.position.y + adder.y;
        transform.position = newPos;
    }
}
