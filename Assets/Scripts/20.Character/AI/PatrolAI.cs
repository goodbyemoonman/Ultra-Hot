using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : AbstractAI
{
    public PatrolAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject w)
    {
        bca = b;
        sa = s;
        who = w;
        moveHandler = who.GetComponent<MoveHandler>();
        path = new List<Vector2>();
    }

    public void Initialize()
    {
        path.Clear();
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
        //Debug.DrawLine(who.transform.position, goal, Color.blue, 0.5f);
        path = sa.GetPath(who.transform.position, goal);
    }

    public override bool CheckAIChange(AIHolder aiHolder)
    {
        //바운더리에 오브젝트가 있을 때 추적
        if (bca.GetObjList().Count != 0)
        {
            bca.SetTarget(bca.GetObjList()[0]);
            if (bca.GetObjList()[0].CompareTag("Player"))
                aiHolder.SetAI(AIHolder.AIList.ChasePlayer);
            else
                aiHolder.SetAI(AIHolder.AIList.ChaseEquip);
            return false;
        }

        return true;
    }

    public override bool Check(AIHolder aiHolder)
    {
        if (CheckAIChange(aiHolder) == false)
            return false;

        while(path.Count == 0)
        {
            GetRandomPath();
        }

        if (path.Count > 0 &&
            ((Vector2)who.transform.position - path[0]).magnitude < 0.1f)
        {
            path.RemoveAt(0);
        }

        return true;
    }

    public override void Do()
    {
        if (path.Count == 0)
            return;

        moveHandler.MoveToWorldPosition(path[0]);
    }
}
