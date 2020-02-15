using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
    public Transform targetTf;

    private void LateUpdate()
    {
        Vector3 newPos = transform.position;
        newPos.x = targetTf.position.x;
        newPos.y = targetTf.position.y;
        transform.position = newPos;
    }
}
