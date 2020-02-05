using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour {

    public void MoveTo(Vector2 dir)
    {
        transform.Translate(dir * Time.deltaTime * 5, Space.World);
    }

    public void MoveRelativeRotation(Vector2 dir)
    {
        transform.Translate(dir * Time.deltaTime * 5, Space.Self);
    }
}
