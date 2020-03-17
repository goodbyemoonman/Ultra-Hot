using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : iAI
{
    SeekAlgorithm sa;
    GameObject who;
    MoveHandler mh;
    List<Vector2> path;

    public PatrolAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject w)
    {
        sa = s;
        who = w;
        mh = who.GetComponent<MoveHandler>();
        path = new List<Vector2>();
    }

    void GetRandomPath()
    {
        int radius = Random.Range(1, 5) + Random.Range(1, 5);
        float angle = Random.Range(-5, 5) * 36 * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        Vector3 goal = new Vector2(radius * cos, radius * sin);
        goal.x = Utility.V3ToV2I(goal).x;
        goal.y = Utility.V3ToV2I(goal).y;
        Debug.DrawLine(who.transform.position, goal, Color.blue, 0.5f);
        path = sa.GetPath(who.transform.position, goal);
    }

    public void Check()
    {
        while(path.Count == 0)
        {
            GetRandomPath();
        }

        if (path.Count > 0 &&
            ((Vector2)who.transform.position - path[0]).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }
    }

    public void Do()
    {
        mh.MoveToWorldPosition(path[0]);
    }
}
