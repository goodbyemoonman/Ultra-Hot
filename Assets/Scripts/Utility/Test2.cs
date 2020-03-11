using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {
    private void OnEnable()
    {
        Debug.Log(Utility.V3ToV2I(transform.position) + " : " + IsWall(Utility.V3ToV2I(transform.position)));
    }

    bool IsWall(Vector2Int crd)
    {
        Collider2D col = Physics2D.OverlapCircle(crd, 0.4f, Utility.WallLayer);
        if (col != null)
            return true;
        return
            false;
    }
}
