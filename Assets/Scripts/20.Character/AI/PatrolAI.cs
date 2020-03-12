using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : iAI
{
    BoundaryCheckAlgorithm bca;
    SeekAlgorithm sa;
    GameObject who;
    ActHandler actHandler;
    MoveHandler mh;
    AIHolder ah;
    List<Vector2> path;

    public PatrolAI(BoundaryCheckAlgorithm b, SeekAlgorithm s, GameObject w)
    {
        bca = b;
        sa = s;
        who = w;
        actHandler = who.GetComponent<ActHandler>();
        ah = who.GetComponent<AIHolder>();
        mh = who.GetComponent<MoveHandler>();
        path = new List<Vector2>();
    }

    public bool Check()
    {
        int targetLayer;
        if (actHandler.IsEquipSomething())
            targetLayer = Utility.PlayerLayer;
        else
            targetLayer = Utility.PlayerLayer | Utility.EquipmentLayer;

        List<GameObject> objs = bca.GetObjListInSight(who, 6f, targetLayer);

        if(objs.Count > 0)
        {
            if (objs[0].CompareTag("Player"))
                ah.SetAI(AIHolder.AIList.ChasePlayer);
            else
                ah.SetAI(AIHolder.AIList.ChaseEquip);
            return false;
        }

        if (path.Count != 0 &&
            ((Vector2)who.transform.position - path[0]).magnitude <= 0.1f)
        {
            path.RemoveAt(0);
        }

        while(path.Count == 0)
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


        return true;
    }

    public void Do()
    {
        mh.MoveToWorldPosition(path[0]);
    }
}
