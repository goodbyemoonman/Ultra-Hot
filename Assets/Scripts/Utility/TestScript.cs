using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public Transform start;
    public Transform end;

    SeekAlgorithm sa = new SeekAlgorithm();
    List<Vector2> path = new List<Vector2>();

    [ContextMenu("Find Path")]
    void FindPath()
    {
        path = sa.GetPath(start.position, end.position, false);
    }

    [ContextMenu("Pretty Path")]
    void PrettyPath()
    {
        path = sa.GetPath(start.position, end.position, true);
    }

    private void OnDrawGizmosSelected()
    {
        if (path.Count == 0)
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, path[0]);
        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(path[i], path[i + 1]);
        }
    }
}

